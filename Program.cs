using NetCoreAlarmServer.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using System.Linq;

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

            var filters = LoadFilters(configFilename);
            if (filters.Count == 0)
            {
                Console.WriteLine("No filters found on config file");
                return;
            }

            var t = new Thread(delegate ()
            {
                // replace the IP with your system IP Address...
                Server myserver = new Server(ip, port, filters);
            });
            t.Start();

            Console.WriteLine($"Alarm Server Started (on {ip}:{port})");
        }

        private static List<Filter> LoadFilters(string configFilename)
        {
            const string NameSpaceActionPrefix = "NetCoreAlarmServer.Settings.";

            var lst = new List<Filter>();

            using (var file = File.OpenRead(configFilename))
            {
                var xml = XDocument.Load(file);
                foreach (var filter in xml.Descendants("Filters").First().Elements("Filter"))
                {
                    var f = new Filter();
                    f.MinInterval = Convert.ToInt32(filter.Attribute("MinInterval")?.Value ?? "15");
                    f.Items = new Dictionary<string, string>();
                    foreach (var item in filter.Element("Items").Elements())
                    {
                        f.Items[item.Name.LocalName] = item.Value;
                    }
                    f.Actions = new List<AlarmAction>();
                    foreach (var action in filter.Elements("Actions").Elements())
                    {
                        var act = (AlarmAction)Activator.CreateInstance(Type.GetType(NameSpaceActionPrefix + action.Name.LocalName));
                        foreach (var attr in action.Attributes())
                        {
                            act.GetType().GetProperty(attr.Name.LocalName).SetValue(act, attr.Value);
                        }
                        f.Actions.Add(act);
                    }
                    lst.Add(f);
                }
            }

            return lst;
        }
    }
}
