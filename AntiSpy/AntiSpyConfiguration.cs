using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntiSpy
{
    public class AntiSpyConfiguration : IRocketPluginConfiguration
    {
        public string Key;
        public int TimeToRedoChecks;

        public void LoadDefaults()
        {
            Key = "Key-Here";
            TimeToRedoChecks = 5;
        }
    }
}
