using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace JuggernautControlAndroid
{
    public class UDPCommunication
    {
        UdpClient client;
        Action<MemoryStream> receivedFrame;

        public UDPCommunication(string ip, int port, Action<MemoryStream> receivedFrame)
        {
            this.receivedFrame = receivedFrame;
            client = new UdpClient(ip, port);
        }

        public void write(byte[] data, IPEndPoint ep)
        {
            client.Send(data, data.Length);
            var x = client.Receive(ref ep);
            receivedFrame(new MemoryStream(x));
        }
    }
}