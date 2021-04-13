using System;
using System.Net.Sockets;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Hoho.Android.UsbSerial.Driver;

[assembly: UsesFeature("android.hardware.usb.host")]
namespace JuggernautNXT
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    [IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached })]
    [MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter")]
    public class MainActivity : AppCompatActivity
    {
        MyCamera mc;
        
        TCPCommunicator tcp;
        UDPCommunication udp;
        bool serialAttached;
        string ip;

        EditText text;
        TextView t1, t2;
        TextureView image;

        UsbManager usbManager;
        IUsbSerialDriver driver;
        IUsbSerialPort port;
        UsbManager manager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            PowerManager pm = (PowerManager)GetSystemService(Context.PowerService);
            PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.ScreenBright, "My Tag");
            wl.Acquire();
            this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            text = FindViewById<EditText>(Resource.Id.editText1);
            text.Text = getIP();
            t1 = FindViewById<TextView>(Resource.Id.textView2);
            t2 = FindViewById<TextView>(Resource.Id.textView3);

            var client = new TcpClient();

            Button button1 = FindViewById<Button>(Resource.Id.button1);
            button1.Click += StartBotClicked;

            Button button2 = FindViewById<Button>(Resource.Id.button2);
            button2.Click += NetworkTestClicked;

            usbManager = GetSystemService(Context.UsbService) as UsbManager;

            image = FindViewById<TextureView>(Resource.Id.imageView1);
            mc = new MyCamera(image);
            mc.FrameRefreshed = sendFrame;
        }


        private void StartBotClicked(object sender, EventArgs e)
        {
            serialAttached = true;
            manager = (UsbManager)GetSystemService(Context.UsbService);
            driver = UsbSerialProber.DefaultProber.FindAllDrivers(manager)[0];
            port = driver.Ports[0];
            var connection = manager.OpenDevice(driver.Device);
            port.Open(connection);
            port.SetParameters(9600, 8, StopBits.One, Parity.None);

            ip = text.Text;

            Thread t = new Thread(x => tcpCommunication());
            t.Start();
            udp = new UDPCommunication(ip, 4815);
            mc.start();
        }

        private void sendFrame(byte[][] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                udp.write(data[i]);
            }
        }

        public static string jsIPID = "306fc0a4-9ae4-11eb-ad57-0242ac110002";
        public static string jsBin = "ccfbc521785e";
        public static string getIP()
        {
            using (var client = new System.Net.WebClient())
            {
                client.Headers["Security-key"] = jsIPID;
                return System.Text.Encoding.Default.GetString(client.DownloadData("https://json.extendsclass.com/bin/" + jsBin)).Split(':')[1].Replace("\"", "").Replace("}", "").Replace(" ","");
            }
        }

        private void NetworkTestClicked(object sender, EventArgs e)
        {
            serialAttached = false;
            ip = text.Text;

            Thread t = new Thread(x => tcpCommunication());
            t.Start();
            udp = new UDPCommunication(ip, 4815);
            mc.start();
        }

        private async void tcpCommunication()
        {
            while (true)
            {
                byte L, R;
                try
                {
                    tcp = new TCPCommunicator(ip, 4816);
                    tcp.connect();
                    byte[] xoxo = new byte[] { (byte)'X', (byte)'O', (byte)'X', (byte)'O' };
                    try
                    {
                        while (true)
                        {
                            var commands = tcp.communicate(xoxo);
                            if (commands.Length >= 2)
                            {
                                L = commands[0];
                                R = commands[1];

                                try
                                {
                                    if (serialAttached) port.Write(new byte[] { L, R }, 2);
                                }
                                catch
                                {
                                }

                                this.RunOnUiThread(() =>
                                {
                                    t1.Text = L.ToString();
                                    t2.Text = R.ToString();
                                });

                            }
                        }
                    }
                    catch (Exception e)
                    { }
                    finally
                    { tcp.close(); }
                    L = 90;
                    R = 90;
                    try
                    {
                        if (serialAttached) port.Write(new byte[] { L, R }, 2);
                    }
                    catch
                    {
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception e) { }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

	}
}
