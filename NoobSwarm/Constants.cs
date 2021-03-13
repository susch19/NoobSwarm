using System;
using System.IO;

namespace NoobSwarm
{
    public static class Constants
    {
        public static readonly DirectoryInfo RootDirectory;
        public static readonly DirectoryInfo LogDirectory;
        public static readonly DirectoryInfo SettingsDirectory;
        public static readonly string LightServiceSettings;
        public static readonly string HotKeyManagerSettings;

        static Constants()
        {
            var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NoobSwarm");
            RootDirectory = new(root);
            RootDirectory.Create();

            LogDirectory = RootDirectory.CreateSubdirectory("logs");
            SettingsDirectory = RootDirectory.CreateSubdirectory("settings");

            LightServiceSettings = Path.Combine(SettingsDirectory.FullName, "LightService.save");
            HotKeyManagerSettings = Path.Combine(SettingsDirectory.FullName, "Makros.save");
        }
    }
}
