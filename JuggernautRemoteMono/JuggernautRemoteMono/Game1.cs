using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using RobotControlNew.UDP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace JuggernautControlAndroid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D t2d, dummyTexture;
        FixedSpeedRobotController rc;
        GamePad gp;
        TCPCommunicator tcp;
        UDPCommunication udp;
        byte L, R;

        public UdpFrameManager frameManager = new UdpFrameManager();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            

            dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });

            gp = new GamePad(new Rectangle(1540*2/3, 540 * 2 / 3, 260 * 2 / 3, 540 * 2 / 3), new Rectangle(30 * 2 / 3, 800 * 2 / 3, 700 * 2 / 3, 270 * 2 / 3), dummyTexture);

            //   IPSender.refreshIP();
            //rc = new FixedSpeedRobotController(ReceivedFrame, 90, 24);
            //rc.start();
            tcp = new TCPCommunicator(IPSender.getIP(), 4817);
            udp = new UDPCommunication(IPSender.getIP(), 4818, ReceivedFrame);
            Thread t = new Thread(x => tcpCommunication());
            t.Start();
            Thread t2 = new Thread(x => udpCommunication());
            t2.Start();
        }

        public void ReceivedFrame(MemoryStream ms)
        {
            t2d = Texture2D.FromStream(GraphicsDevice, ms);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            gp.refresh();

            R = 90;
            L = 90;

            if (gp.up)
            {
                L = 1;
                R = 180;
            }
            if (gp.down)
            {
                L = 180;
                R = 1;
            }
            if (gp.left)
            {
                L = 140;
                R = 140;
            }
            if (gp.right)
            {
                L = 40;
                R = 40;
            }
            if (gp.up && gp.right)
            {
                L = 61;
                R = 180;
            }
            if (gp.up && gp.right)
            {
                L = 1;
                R = 120;
            }
            if (gp.down && gp.left)
            {
                L = 120;
                R = 1;
            }
            if (gp.down && gp.right)
            {
                L = 180;
                R = 60;
            }

            // TODO: Add your update logic here

            var frame = frameManager.getFrame();
            if (frame != null)
            {
                MemoryStream ms = new MemoryStream(frame.getData());
                t2d = Texture2D.FromStream(GraphicsDevice, ms);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (t2d != null)
                spriteBatch.Draw(t2d, new Rectangle(60, 60, 1366, 748), Color.White);
            gp.draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }


        private async void tcpCommunication()
        {
            byte[] xoxo = new byte[] { (byte)'X', (byte)'O', (byte)'X', (byte)'O' };
            tcp.connect();
            while (true)
            {
                var commands = tcp.communicate(new byte[] { L, R });
            }
        }

        List<byte[]> frameBuffer = new List<byte[]>();
        object frameBufferLock = new object();

        private async void udpCommunication()
        {
            string ip = IPSender.getIP();
            UdpClient udpw = new UdpClient();
            Console.WriteLine("UDP connection ok");
            // recieve data from any ip address and any port
            IPEndPoint remotew = new IPEndPoint(IPAddress.Any, 4819);//= new IPEndPoint(IPAddress.Parse(ip), 4818);
            udpw.Connect(ip, 4819);
            udpw.Send(new byte[] { 100, 101 }, 2);
            Thread.Sleep(1000);
            Console.WriteLine("UDP server started");
            UdpClient udp = new UdpClient();
            Console.WriteLine("UDP connection ok");
            // recieve data from any ip address and any port
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 4818);//= new IPEndPoint(IPAddress.Parse(ip), 4818);
            udp.Connect(ip, 4818);
            udp.Send(new byte[] { 100, 101 }, 2);

            //UdpClient udp = new UdpClient(4818);
            //IPEndPoint remote = new IPEndPoint(IPAddress.Any, 4818);

           

            while (true)
            {
                byte[] byte_Of_Frame = udp.Receive(ref remote);

             //   Console.WriteLine("received goodies" + byte_Of_Frame[0].ToString() + "." + byte_Of_Frame[1].ToString() + " in " + sw.ElapsedMilliseconds);
                frameManager.receivePackage(byte_Of_Frame);


            }
        }
    }
}
