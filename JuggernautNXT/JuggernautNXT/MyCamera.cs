using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RobotControlNew.UDP;
using static Android.Hardware.Camera;

namespace JuggernautNXT
{
    public class MyCamera : Activity, IPreviewCallback
    {
        TextureView image;

        public int fragmentation = 8;

        bool originalStart = true;

        int noFrames = 0;

        Android.Hardware.Camera cam;
        public MyCamera(TextureView image)
        {
            this.image = image;
        }

        public Action<byte[][]> FrameRefreshed;

        public void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera)
        {
            try
            {
                noFrames = 0;
                YuvImage yi = new YuvImage(data, camera.GetParameters().PreviewFormat, camera.GetParameters().PreviewSize.Width, camera.GetParameters().PreviewSize.Height, null);

                byte[][] frames = new byte[fragmentation][];

                using MemoryStream ms = new MemoryStream();

                yi.CompressToJpeg(new Rect(0, 0, yi.Width, yi.Height), 20, ms);

                byte[] jpegBytes = ms.ToArray();
                UdpFrame frame = new UdpFrame(jpegBytes);

                FrameRefreshed?.Invoke(frame.packages);
            }
            catch(Exception e)
            {
                start();
            }
        }

        void reStarter()
        {
            while (true)
            {
                noFrames++;
                if (noFrames > 15)
                {
                    try
                    {
                        cam.StopPreview();
                        cam.Release();
                    }
                    catch (Exception e)
                    { }
                    start();
                }
                Thread.Sleep(100);
            }
        }

        public void start()
        {
            if (originalStart)
            {
                originalStart = false;
                Thread t = new Thread(new ThreadStart(reStarter));
                t.Start();
            }
            try
            {
                cam = Android.Hardware.Camera.Open();
                cam.GetParameters().PreviewFormat = ImageFormatType.Rgb565;
                var param = cam.GetParameters();
                param.SetPreviewSize(320, 240);
                cam.SetParameters(param);
                var x = cam.GetParameters().SupportedPreviewSizes;
                cam.SetDisplayOrientation(90);
                cam.SetPreviewCallback(this);
                cam.SetPreviewTexture(image.SurfaceTexture);
                cam.StartPreview();
            }
            catch (Exception e) { }
        }
    }
}