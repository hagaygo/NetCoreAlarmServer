using NetCoreAlarmServer.Settings;
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
        TcpListener _server;
        List<Filter> _filters;

        public Server(string ip, int port, List<Filter> filters)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            _filters = filters;
            _server = new TcpListener(localAddr, port);
            _server.Start();
            StartListener();
        }

        public void StartListener()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    var client = _server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    var t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                _server.Stop();
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
                    if (line.Contains("{"))
                    {
                        line = line.Substring(line.IndexOf("{")).Trim();
                        var json = Newtonsoft.Json.Linq.JObject.Parse(line);                        
                        foreach (var filter in _filters)
                        {
                            var fits = true;
                            foreach (var key in filter.Items.Keys)
                                if (json.Value<string>(key) != filter.Items[key])
                                {
                                    fits = false;
                                    break;
                                }
                            if (fits)
                            {
                                Console.WriteLine("Alarm fits filter , executing actions...");
                                PerformAction(filter);
                            }
                        }
                    }  // else ignore for now
                }
                Console.WriteLine("Disconnected");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }

        private void PerformAction(Filter filter)
        {
            if (filter.LastOccurrence.HasValue && DateTime.Now - filter.LastOccurrence < TimeSpan.FromSeconds(filter.MinInterval))
            {
                Console.WriteLine("Skipping action because of minimum interval");
                return;
            }
            filter.LastOccurrence = DateTime.Now;
            foreach (var act in filter.Actions)
            {
                act.Execute();
            }
        }
    }
}
