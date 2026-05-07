using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valorant_Instalocker.Utils.Configs;
using Valorant_Instalocker.Views;
using RadiantConnect.Methods;
using RadiantConnect;
using Valorant_Instalocker.Main.Game;
using Serilog;
using Valorant_Instalocker.Utils.Constants;
namespace Valorant_Instalocker.Main
{
    public static class AppStateManager
    {
        public static Initiator initiator;
        public static UserSettings CurrentUserSettings { get; set; } = new Utils.Configs.UserSettings();
        public static Dictionary<ValorantTables.Agent, string> Agents = new Dictionary<ValorantTables.Agent, string>();
        public static ValorantTables.Agent SelectedAgent;
        public static bool OnlySelect = false;
        
        private static void InitializeLogger()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                Constants.LogPath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:MM/d:HH:mm:ss}] - [{SourceContext}]:[{Level:u3}] : {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();
            Log.Information("Instalocker Başladı..");

        }
        public static void Initialize()
        {

            InitializeLogger();

            CurrentUserSettings = ConfigManager.InitializeSettings();
            initiator = Connection.ConnectValorant();
        }
    
    }
}
