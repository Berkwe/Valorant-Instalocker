using RadiantConnect;
using RadiantConnect.Methods;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using Valorant_Instalocker.Views;

namespace Valorant_Instalocker.Main.Game
{
    internal class Controller
    {
        private static async Task LockAgent()
        {
            Log.Information("[LockAgent] Ajan kilitleme işlemi başlatılıyor. Hedef Ajan ID: {AgentID}", AppStateManager.SelectedAgent);
            try
            {
                await AppStateManager.initiator.Endpoints.PreGameEndpoints.LockCharacterAsync(AppStateManager.SelectedAgent);
                Log.Information("[LockAgent] Ajan kilitleme isteği başarıyla tamamlandı.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[LockAgent] Ajan kilitleme sırasında hata oluştu. AgentID: {AgentID}", AppStateManager.SelectedAgent);
                throw;
            }
        }

        private static async Task SelectAgent()
        {
            Log.Information("[SelectAgent] Ajan hover (seçme) işlemi başlatılıyor. Hedef Ajan ID: {AgentID}", AppStateManager.SelectedAgent);
            try
            {
                await AppStateManager.initiator.Endpoints.PreGameEndpoints.SelectCharacterAsync(AppStateManager.SelectedAgent);
                Log.Information("[SelectAgent] Ajan hover (seçme) isteği başarıyla tamamlandı.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[SelectAgent] Ajan seçme sırasında hata oluştu. AgentID: {AgentID}", AppStateManager.SelectedAgent);
                throw;
            }
        }

        public static async void GameControlHandler(string MatchID = "")
        {
            Log.Information("[GameControlHandler] Tetiklendi. MatchID: {MatchID}, OnlySelect Modu: {OnlySelect}", MatchID, AppStateManager.OnlySelect);
            try
            {
                AppStateManager.initiator.TcpEvents.OnGameStateChanged -= Controller.BreakProtectionHandler;
                if (!AppStateManager.Instance.UserName.ToLower().Contains("berkwe")) await Task.Delay(70);
                if (AppStateManager.OnlySelect)
                {
                    Log.Debug("[GameControlHandler] 'Sadece Seçim' modu aktif. SelectAgent() çağrılıyor.");
                    await SelectAgent();
                }
                else
                {
                    Log.Debug("[GameControlHandler] 'Kilitleme' modu aktif. LockAgent() çağrılıyor.");
                    await LockAgent();
                }
                AppStateManager.Instance.IsAgentLocked = true;
                Log.Debug("[GameControlHandler] OnPreGameMatchLoaded olayından abonelik kaldırılıyor.");
                AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded -= Controller.GameControlHandler;

                AppStateManager.Instance.IsInstalockRunning = false;
                Log.Information("[GameControlHandler] İşlem döngüsü sorunsuz tamamlandı. IsInstalockRunning: false");

                AppStateManager.initiator.TcpEvents.OnGameStateChanged += Controller.BreakProtectionHandler;
                Log.Information("Bozulma Koruması Başladı");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GameControlHandler] Ajan seçimi veya kilitlenmesi sırasında kritik bir API/Sistem hatası oluştu. MatchID: {MatchID}", MatchID);
            }
        }


        public static async void CreateInstalockGameHandler(ValorantTables.Agent SelectedAgent, string SelectedAgentName, string SelectedAgentImageURL)
        {
            Log.Information("[CreateInstalockGameHandler] Kurulum başlatıldı. Seçilen Ajan: {AgentName}", SelectedAgentName);
            AppStateManager.initiator.TcpEvents.OnGameStateChanged -= Controller.BreakProtectionHandler;
            if (AppStateManager.Instance.IsInstalockRunning)
            {
                AppStateManager.Instance.IsGameBreaked = false;
                Log.Warning("[CreateInstalockGameHandler] Halihazırda çalışan bir işlem tespit edildi. Eski abonelik iptal ediliyor.");
                AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded -= Controller.GameControlHandler;
            }
            Log.Information(AppStateManager.Instance.IsGameBreaked.ToString());
            AppStateManager.Instance.SelectedAgentName = SelectedAgentName;
            AppStateManager.Instance.SelectedAgentImage = SelectedAgentImageURL;
            AppStateManager.SelectedAgent = SelectedAgent;

            Log.Debug("[CreateInstalockGameHandler] State Manager güncellendi. Mevcut Oyun Aşaması: {CurrentPhase}", AppStateManager.Instance.CurrentPhase);

            if (AppStateManager.Instance.CurrentPhase.ToLower() == "pregame" || AppStateManager.Instance.CurrentPhase.ToLower() == "gameplay")
            {
                Log.Information("[CreateInstalockGameHandler] Oyun halihazırda {CurrentPhase} aşamasında. Olay beklenmeden GameControlHandler anında tetikleniyor.", AppStateManager.Instance.CurrentPhase);
                Controller.GameControlHandler();
                return;
            }

            Log.Information("[CreateInstalockGameHandler] Bekleme moduna geçiliyor. OnPreGameMatchLoaded olayına abone olundu.");
            AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded += Controller.GameControlHandler;
            AppStateManager.Instance.IsInstalockRunning = true;
        }


        public static async void CancelControlHandler()
        {
            Log.Information("[CancelControlHandler] İptal işlemi tetiklendi.");

            if (!AppStateManager.Instance.IsInstalockRunning)
            {
                Log.Debug("[CancelControlHandler] İptal edilecek aktif bir instalock işlemi bulunmuyor. Metottan çıkılıyor.");
                return;
            }

            Log.Debug("[CancelControlHandler] OnPreGameMatchLoaded olayından abonelik kaldırılıyor ve AppStateManager değerleri default olarak sıfırlanıyor.");
            AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded -= Controller.GameControlHandler;

            AppStateManager.Instance.SelectedAgentName = default;
            AppStateManager.Instance.SelectedAgentImage = default;
            AppStateManager.SelectedAgent = default;
            AppStateManager.Instance.IsGameBreaked = false;
            AppStateManager.Instance.IsInstalockRunning = false;

            Log.Information("[CancelControlHandler] İptal işlemi başarıyla tamamlandı. State sıfırlandı.");
        }

        public static async Task QuitPreGame()
        {
            Log.Information("[QuitPreGame] Maçtan çıkma işlemi başlatılıyor.");
            try
            {
                AppStateManager.initiator.TcpEvents.OnGameStateChanged -= Controller.BreakProtectionHandler;
                await AppStateManager.initiator.Endpoints.PreGameEndpoints.QuitGameAsync();
                Log.Information("[QuitPreGame] Maçtan çıkma başarıyla tamamlandı.");
            }
            catch (RadiantConnectNetworkStatusException ex)
            {
                Log.Warning(ex, "[QuitPreGame] RadiantConnect network hatası oluştu. Uygulama açık olabilir.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[QuitPreGame] Maçtan çıkma sırasında hata oluştu");
                throw;
            }
        }


        public static async void BreakProtectionHandler(string phase)
        {
            try
            {
                AppStateManager.initiator.TcpEvents.OnGameStateChanged -= Controller.BreakProtectionHandler;

                Log.Information($"BreakProtection çalıştı : {phase}");

                if (phase.ToLower() == "ingame" || phase.ToLower() == "ıngame")
                {
                    AppStateManager.Instance.IsGameBreaked = false;
                    Log.Information("Oyun Bozulmadı.");

                }
                else
                {
                    AppStateManager.Instance.IsGameBreaked = true;
                    Log.Information($"Oyun Bozuldu instalocker yeniden seçiyor.");
                    CreateInstalockGameHandler(AppStateManager.SelectedAgent, AppStateManager.Instance.SelectedAgentName, AppStateManager.Instance.SelectedAgentImage);
                }



            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

        }
    }
}