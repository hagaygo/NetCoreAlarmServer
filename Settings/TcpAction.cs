using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NetCoreAlarmServer.Settings
{
    public class TcpAction : AlarmAction
    {
        public string Port { get; set; }
        public string Address { get; set; }

        public override void Execute()
        {
            using (var tcp = new TcpClient())
                try
                {
                    tcp.Connect(Address, Convert.ToInt32(Port));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on TcpAction ({0}:{1}) , {2}", Address, Port, ex.Message);
                }
        }
    }
}