using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TCPCommunicator
    {
        TcpClient clientSocket;
        string ip;
        int port;
        public TCPCommunicator(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        public void connect()
        {
            clientSocket = new TcpClient();
            clientSocket.ReceiveBufferSize = 4096;
            clientSocket.Connect(ip, port);
        }

        public byte[] communicate(byte[] data)
        {
            NetworkStream serverStream = clientSocket.GetStream();
            byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
            serverStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
            serverStream.Write(data, 0, data.Length);
            serverStream.Flush();
            return bytesFrom;
        }
    }
}