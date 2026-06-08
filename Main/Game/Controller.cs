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
        // Ensures we only lock/select once per pregame (agent select) session.
        // Reset when the game leaves the pregame phase, so every new match locks again.
        private static bool _hasLockedThisPregame = false;

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

            // Skip if we already locked in this pregame (both the log and presence events can
            // fire around the same time; this prevents a double lock).
            if (_hasLockedThisPregame)
            {
                Log.Debug("[GameControlHandler] Bu pregame için kilitleme zaten yapıldı, atlanıyor.");
                return;
            }
            _hasLockedThisPregame = true;

            try
            {
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

                AppStateManager.Instance.IsInstalockRunning = false;
                Log.Information("[GameControlHandler] İşlem döngüsü sorunsuz tamamlandı. IsInstalockRunning: false");
            }
            catch (Exception ex)
            {
                // Lock failed (e.g. pregame not ready yet). Reset the flag so another event
                // (presence/log) within the same pregame can retry.
                _hasLockedThisPregame = false;
                Log.Error(ex, "[GameControlHandler] Ajan seçimi veya kilitlenmesi sırasında kritik bir API/Sistem hatası oluştu. MatchID: {MatchID}", MatchID);
            }
        }


        public static void CreateInstalockGameHandler(ValorantTables.Agent SelectedAgent, string SelectedAgentName, string SelectedAgentImageURL)
        {
            Log.Information("[CreateInstalockGameHandler] Kurulum başlatıldı. Seçilen Ajan: {AgentName}", SelectedAgentName);

            AppStateManager.Instance.SelectedAgentName = SelectedAgentName;
            AppStateManager.Instance.SelectedAgentImage = SelectedAgentImageURL;
            AppStateManager.SelectedAgent = SelectedAgent;
            AppStateManager.Instance.IsGameBreaked = false;

            // New setup: allow locking again for this pregame (the agent may have changed).
            _hasLockedThisPregame = false;

            // We trigger locking from two independent sources:
            //  1) OnPreGameMatchLoaded  -> log-based, fast but one-shot/can be missed.
            //  2) OnGameStateChanged    -> presence/websocket-based, fires reliably every match.
            // Subscriptions are persistent, so once set up every match auto-locks.
            // Remove then re-add first to avoid duplicate subscriptions.
            AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded -= Controller.GameControlHandler;
            AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded += Controller.GameControlHandler;
            AppStateManager.initiator.TcpEvents.OnGameStateChanged -= Controller.GameStateWatcher;
            AppStateManager.initiator.TcpEvents.OnGameStateChanged += Controller.GameStateWatcher;

            AppStateManager.Instance.IsInstalockRunning = true;

            Log.Debug("[CreateInstalockGameHandler] State Manager güncellendi. Mevcut Oyun Aşaması: {CurrentPhase}", AppStateManager.Instance.CurrentPhase);

            // If we're already in agent select, lock right away without waiting for events.
            if (AppStateManager.Instance.CurrentPhase.ToLower() == "pregame")
            {
                Log.Information("[CreateInstalockGameHandler] Oyun halihazırda pregame aşamasında. GameControlHandler anında tetikleniyor.");
                Controller.GameControlHandler();
                return;
            }

            Log.Information("[CreateInstalockGameHandler] Bekleme moduna geçildi. Pregame olayları dinleniyor.");
        }


        public static async void CancelControlHandler()
        {
            Log.Information("[CancelControlHandler] İptal işlemi tetiklendi.");

            if (!AppStateManager.Instance.IsInstalockRunning)
            {
                Log.Debug("[CancelControlHandler] İptal edilecek aktif bir instalock işlemi bulunmuyor. Metottan çıkılıyor.");
                return;
            }

            Log.Debug("[CancelControlHandler] Removing all pregame subscriptions and resetting AppStateManager values to default.");
            AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded -= Controller.GameControlHandler;
            AppStateManager.initiator.TcpEvents.OnGameStateChanged -= Controller.GameStateWatcher;

            _hasLockedThisPregame = false;
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


        // Presence/websocket-based game phase watcher. Unlike the log-based OnPreGameMatchLoaded
        // event, this fires reliably every match, so it guarantees locking works on both the
        // first match and all subsequent ones. It also covers break protection (falling back
        // into agent select).
        public static void GameStateWatcher(string phase)
        {
            try
            {
                string currentPhase = phase.ToLower();
                Log.Information("[GameStateWatcher] Oyun aşaması: {Phase}", phase);

                if (currentPhase == "pregame")
                {
                    // Entered agent select (or fell back into it after a break) -> lock.
                    GameControlHandler();
                }
                else
                {
                    // Left agent select (ingame/menus/idle). Allow locking again for the
                    // next pregame.
                    _hasLockedThisPregame = false;

                    if (currentPhase == "ingame" || currentPhase == "ıngame")
                        AppStateManager.Instance.IsGameBreaked = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}