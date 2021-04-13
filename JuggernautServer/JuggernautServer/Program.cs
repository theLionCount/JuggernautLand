using RealtimeRobotControll;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace JuggernautServer
{
    class Program
    {
        static Random r = new Random();

        static void Main(string[] args)
        { 
            Console.WriteLine("Juggernaut server started");
            Console.WriteLine("Sending ip");
            IPSender.refreshIP();
            Console.WriteLine("ip sent");
            Console.WriteLine("do you want logging? (y/n)");
            var s = Console.ReadLine();
            jgLogger.log = s == "y";
            Console.WriteLine("LS: ");
            s = Console.ReadLine();
            int LS = Convert.ToInt32(s);
            Console.WriteLine("RS: ");
            s = Console.ReadLine();
            int RS = Convert.ToInt32(s); ;
            FixedSpeedRobotController rc = new FixedSpeedRobotController(receiveFrame, LS, RS);
            rc.start();
            rc.RemoteControlled = true;
        }
        static void receiveFrame(MemoryStream ms)
        {
        }

    }
}
