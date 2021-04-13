using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace JuggernautControlAndroid
{
    public class RobotController
    {
        private byte _L;

        public byte L
        {
            get { return _L; }
            set
            {
                if (value <= 180 && value > 0) _L = value;
            }
        }

        private byte _R;

        public byte R
        {
            get { return _R; }
            set
            {
                if (value <= 180 && value > 0) _R = value;
            }
        }

        public int tcpPort, udpPort;

        public RobotController(int UDPPort = 4815, int TCPPort = 4816)
        {
            tcpPort = TCPPort;
            udpPort = UDPPort;
            L = 90;
            R = 90;
        }

        public Action<MemoryStream> receivedFrame;

        public Thread startUdpServer()
        {
            Thread t = new Thread(x => startUdpServerInternal());
            t.Start();
            return t;
        }

        public Thread startTcpServer()
        {
            Thread t = new Thread(x => startTcpServerInternal());
            t.Start();
            return t;
        }

        public virtual byte[] getTcpBytes()
        {
            return new byte[] { L, R };
        }

        private void startUdpServerInternal()
        {
            Console.WriteLine("UDP server started");
            UdpClient udp = new UdpClient(4818);
            Console.WriteLine("UDP connection ok");
            // recieve data from any ip address and any port
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {

                byte[] byte_Of_Frame = udp.Receive(ref remote);
                MemoryStream ms = new MemoryStream(byte_Of_Frame);
                receivedFrame?.Invoke(ms);
            }
        }

        private void startTcpServerInternal()
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 4816);
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" TCP Server Started");

            while ((true))
            {
                clientSocket = serverSocket.AcceptTcpClient();
                clientSocket.ReceiveBufferSize = 4096;
                Console.WriteLine(" >> Accept connection from client");
                NetworkStream networkStream = clientSocket.GetStream();
                var ob = getTcpBytes();
                networkStream.Write(getTcpBytes(), 0, ob.Length);
                networkStream.Flush();
                while (clientSocket.Connected)
                {
                    try
                    {
                        networkStream = clientSocket.GetStream();
                        byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                        networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                        ob = getTcpBytes();
                        networkStream.Write(ob, 0, ob.Length);
                        networkStream.Flush();
                        Console.WriteLine("TCP received: " + Encoding.ASCII.GetString(bytesFrom));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                clientSocket.Close();
            }
        }
    }
}
