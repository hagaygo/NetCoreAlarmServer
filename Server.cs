using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetCoreAlarmServer
{
    class Server
    {
        TcpListener server = null;
        public Server(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            StartListener();
        }

        public void StartListener()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    var t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void HandleDeivce(Object obj)
        {
            var client = (TcpClient)obj;
            var stream = client.GetStream();
            
            try
            {
                var sr = new StreamReader(stream);
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    Console.WriteLine("Got " + line);
                    if (line.Contains("{"))
                    {
                        line = line.Substring(line.IndexOf("{")).Trim();
                        Console.WriteLine("line after trim " + line);
                    }                    
                }
                Console.WriteLine("Disconnected");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }
    }
}
