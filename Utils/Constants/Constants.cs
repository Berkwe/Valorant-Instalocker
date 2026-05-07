using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valorant_Instalocker.Utils.Constants
{
    internal class Constants
    {
        public static string AgentApiUrl = "https://valorant-api.com/v1/agents/";
        static string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static string ValorantPath = Path.Combine(localAppData, "VALORANT");
        public static string InstalockerPath = Path.Combine(ValorantPath, "Instalocker-gui");
        public static string LogPath = Path.Combine(InstalockerPath, "Instalocker.log");
        public static string SettingsPath = Path.Combine(InstalockerPath, "settings.json");
        

    }
}
