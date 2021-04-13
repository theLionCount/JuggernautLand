using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace JuggernautNXT
{
    public class UDPCommunication
    {
        UdpClient client;

        public UDPCommunication(string ip, int port)
        {
            client = new UdpClient(ip, port);
        }

        public void write(byte[] data)
        {
            client.Send(data, data.Length);
        }
    }
}