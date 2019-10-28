﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreAlarmServer.Settings
{
    public class Filter
    {
        public int MinInterval { get; set; }
        public List<AlarmAction> Actions { get; set; }
        public Dictionary<string, string> Items { get; set; }
    }
}
