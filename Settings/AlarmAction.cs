using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreAlarmServer.Settings
{
    public abstract class AlarmAction
    {
        public string Description { get; set; }

        public abstract void Execute();        
    }
}
