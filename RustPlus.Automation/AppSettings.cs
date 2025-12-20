using System;
using System.Collections.Generic;
using System.Text;

namespace RustPlus_Automation
{
    public class AppSettings
    {
        public string RustPlusConfigPath { get; set; }
        public string ServerIP { get; set; }
        public int RustPlusPort { get; set; }
        public ulong SteamId { get; set; }
        public int PlayerToken { get; set; }
        public float BaseLocationX { get; set; }
        public float BaseLocationY { get; set; }
        public float Radius { get; set; }
        public uint SmartSwitchId { get; set; }
        public bool SmartSwitchStateToSet { get; set; }
    }
}
