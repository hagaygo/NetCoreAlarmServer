using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetCoreAlarmServer.Settings
{
    public class HttpGetAction : AlarmAction
    {
        public string Url { get; set; }

        public override void Execute()
        {
            var wr = WebRequest.Create(Url);
            try
            {
                wr.GetResponse();                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on HttpGetAction (Url {0}) , {1}", Url, ex.Message);
            }
        }
    }
}
