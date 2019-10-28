using System;
using System.Threading;

namespace NetCoreAlarmServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Missing parameters : ");
                Console.WriteLine("1.Address to bind/listen");
                Console.WriteLine("2.TCP Port");
                Console.WriteLine("3.config.xml path");
                return;
            }
            var ip = args[0];
            var port = Convert.ToInt32(args[1]);
            var configFilename = args[2];

            var t = new Thread(delegate ()
            {
                // replace the IP with your system IP Address...
                Server myserver = new Server(ip, port);
            });
            t.Start();

            Console.WriteLine($"Alarm Server Started (on {ip}:{port})");
        }
    }
}
