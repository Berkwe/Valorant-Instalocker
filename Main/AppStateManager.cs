using System;
using CommunityToolkit.Mvvm.ComponentModel;
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
using System.Windows;
namespace Valorant_Instalocker.Main
{
    public partial class AppStateManager : ObservableObject
    {
        public static Initiator initiator;
        public static AppStateManager Instance { get; } = new AppStateManager();
        public static UserSettings CurrentUserSettings { get; set; } = new Utils.Configs.UserSettings();
        public static Dictionary<ValorantTables.Agent, string> Agents = new Dictionary<ValorantTables.Agent, string>();
        public static ValorantTables.Agent SelectedAgent;
        public static bool OnlySelect = false;
        
        [ObservableProperty] private bool isClientLoggedIn = false;
        
        [ObservableProperty] private string userName = "Player";

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
        public static async void Initialize()
        {

            InitializeLogger();
            CurrentUserSettings = ConfigManager.InitializeSettings();
            await Connection.ConnectValorant();
            if (Instance.IsClientLoggedIn)
            {
                Instance.UserName = await Connection.GetUserName();


            }
        }

    }
}
