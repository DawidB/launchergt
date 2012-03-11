using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LauncherGTLib
{
    public enum UserSettingsMode { Full, SQL, InsNavireo, InsNavireoIQA, InsSubiektGT, InsGestorGT, InsRewizorGT, InsGratyfikantGT };

    public enum DisplayMode { Tabs, HorizontalGroupBoxScroll, VerticalGroupBoxScroll, }

    public struct SettingsDefinition
    {
        public UserSettingsMode Mode {get; set;}
        public int Id {get; set;}
        public string TabKey { get; set; }
        public string TabText { get; set; }
    }

    public class SettingsPackage
    {
        private UserSettings[] _settings;
        public UserSettings[] Settings { get { return _settings; } set { _settings = value; } }


        public SettingsPackage()
        {
            _settings = new UserSettings[1];
        }

        public SettingsPackage(int _count)
        {
            _settings = new UserSettings[_count];
        }
    }
}
