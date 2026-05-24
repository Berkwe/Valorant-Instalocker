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

        public static string GithubUrl = "https://github.com/Berkwe/Valorant-Instalocker/";
        public static string DiscordUrl = "https://discord.gg/YMbKCawDzy";
        public static string GithubInstalockerTUIURL = "https://github.com/Berkwe/Valorant-instalocker-TUI";


        static string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static string ValorantPath = Path.Combine(localAppData, "VALORANT");
        public static string InstalockerPath = Path.Combine(ValorantPath, "Instalocker-gui");


        public static string LogsPath = Path.Combine(InstalockerPath, "Logs");
        public static string LogPath = Path.Combine(LogsPath, "Instalocker.log");

        public static string CachePath = Path.Combine(InstalockerPath, "Cache");
        public static string CacheImagesPath = Path.Combine(CachePath, "Images");

        public static string SettingsPath = Path.Combine(InstalockerPath, "settings.json");


        

    }
}
