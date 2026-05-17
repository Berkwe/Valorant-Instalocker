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
using System.Net;
using Valorant_Instalocker.Utils.Helpers;
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


        public Visibility AgentPickTableVisibility =>
            (!Instance.IsInstalockRunning && (Instance.CurrentPhase == "pregame" || Instance.CurrentPhase == "gameplay"))
                ? Visibility.Visible : Visibility.Collapsed;

        [ObservableProperty] private bool isGameBreaked = false;
        [ObservableProperty] private string activeGreetingMessage = "";
        [ObservableProperty] private bool isAgentLocked = false;
        [ObservableProperty] private string currentPhase = "";

        [ObservableProperty] public bool quitPreGameBtnBool = false;

        [ObservableProperty] private bool isInstalockRunning = false;

        [ObservableProperty] private string selectedAgentName = "";
        [ObservableProperty] private string selectedAgentImage = "";
        [ObservableProperty] private int currentPlayerLevel = 0;

        [ObservableProperty] private bool isClientLoggedIn = false;
        
        [ObservableProperty] private string userName = "Player";
        [ObservableProperty] private string currentPhaseTranslated = "";


        partial void OnIsInstalockRunningChanged(bool value)
        {
            OnPropertyChanged(nameof(AgentPickTableVisibility));
        }

        partial void OnCurrentPhaseChanged(string value)
        {
            OnPropertyChanged(nameof(AgentPickTableVisibility));
            Instance.QuitPreGameBtnBool = true;
        }



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
            GreetingHelper.StartToGreetingTimer();

            await Connection.ConnectValorant();
            if (Instance.IsClientLoggedIn)
            {
                // AD alnma ksımı
                Instance.UserName = await Connection.GetUserName();
                // phase alma ve evente ekleme
                var CurrentPhase = await Connection.GetCurrentPhase();
                var TranslatedPhase = DashboardView.TranslatePhase(CurrentPhase);
                Instance.CurrentPhase = CurrentPhase.ToLower();
                Instance.CurrentPhaseTranslated = TranslatedPhase;
                initiator.TcpEvents.OnGameStateChanged += (string phase) =>
                {
                    var EventTranslatedPhase = DashboardView.TranslatePhase(phase);
                    Instance.CurrentPhase = phase.ToLower();
                    Instance.CurrentPhaseTranslated = EventTranslatedPhase;
                    
                    if (phase.ToLower() != "pregame") AppStateManager.Instance.IsAgentLocked = false;

                };

                // Seviye kısmı
                Instance.CurrentPlayerLevel = await Connection.GetCurrentPlayerLevel();

            }



        }

    }
}
