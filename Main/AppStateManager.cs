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

        partial void OnIsGameBreakedChanged(bool oldValue, bool newValue)
        {
            Log.Information($"Break game değişti :{oldValue} : {newValue}");
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
            Log.Information("───────────────────────────────────────────");
            Log.Information("  Yeni Oturum Başladı » {Time}", DateTime.Now);
            Log.Information("───────────────────────────────────────────");


        }
        public static async void Initialize()
        {
            Log.Information("[AppStateManager.Initialize] Uygulama başlatılıyor.");
            try
            {
                InitializeLogger();

                try
                {
                    CurrentUserSettings = ConfigManager.InitializeSettings();
                    Log.Information("[AppStateManager.Initialize] Kullanıcı ayarları yüklendi.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[AppStateManager.Initialize] Kullanıcı ayarları yüklenirken hata oluştu. Varsayılan ayarlar kullanılıyor.");
                    CurrentUserSettings = new UserSettings();
                }

                try
                {
                    GreetingHelper.StartToGreetingTimer();
                    Log.Debug("[AppStateManager.Initialize] Selamlaştırma timer başlatıldı.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[AppStateManager.Initialize] Selamlaştırma timer başlatılırken hata oluştu");
                }

                await Connection.ConnectValorant();
                if (Instance.IsClientLoggedIn)
                {
                    try
                    {
                        // Kullanıcı adı alma
                        Instance.UserName = await Connection.GetUserName();
                        Log.Information("[AppStateManager.Initialize] Kullanıcı adı alındı: {UserName}", Instance.UserName);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "[AppStateManager.Initialize] Kullanıcı adı alınırken hata oluştu");
                        Instance.UserName = "Bilinmiyor";
                    }

                    try
                    {
                        // Phase alma ve evente ekleme
                        var CurrentPhase = await Connection.GetCurrentPhase();
                        var TranslatedPhase = DashboardView.TranslatePhase(CurrentPhase);
                        Instance.CurrentPhase = CurrentPhase.ToLower();
                        Instance.CurrentPhaseTranslated = TranslatedPhase;
                        Log.Information("[AppStateManager.Initialize] Oyun aşaması alındı: {CurrentPhase}", CurrentPhase);

                        initiator.TcpEvents.OnGameStateChanged += (string phase) =>
                        {
                            try
                            {
                                var EventTranslatedPhase = DashboardView.TranslatePhase(phase);
                                Instance.CurrentPhase = phase.ToLower();
                                Instance.CurrentPhaseTranslated = EventTranslatedPhase;

                                if (phase.ToLower() != "pregame") AppStateManager.Instance.IsAgentLocked = false;
                                Log.Debug("[AppStateManager.OnGameStateChanged] Oyun aşaması değişti: {Phase}", phase);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "[AppStateManager.OnGameStateChanged] Oyun aşaması event'i işlenirken hata oluştu");
                            }
                        };
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "[AppStateManager.Initialize] Oyun aşaması alınırken hata oluştu");
                        Instance.CurrentPhase = "none";
                    }

                    try
                    {
                        // Seviye alma
                        Instance.CurrentPlayerLevel = await Connection.GetCurrentPlayerLevel();
                        Log.Information("[AppStateManager.Initialize] Oyuncu seviyesi alındı: {Level}", Instance.CurrentPlayerLevel);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "[AppStateManager.Initialize] Oyuncu seviyesi alınırken hata oluştu");
                        Instance.CurrentPlayerLevel = 0;
                    }
                }
                else
                {
                    Log.Warning("[AppStateManager.Initialize] Valorant istemcisine bağlanılamadı. Uygulamayı çalıştırmaya devam ediyorum.");
                }

                Log.Information("[AppStateManager.Initialize] Uygulama başlatılması başarıyla tamamlandı.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[AppStateManager.Initialize] Uygulama başlatılması sırasında kritik hata oluştu");
            }
        }

    }
}
