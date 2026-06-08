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
        // Aynı pregame (ajan seçimi) içinde yalnızca bir kez kilitleme/seçim yapılmasını sağlar.
        // Oyun pregame aşamasından çıktığında sıfırlanır, böylece her yeni maç tekrar kilitlenir.
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

            // Bu pregame içinde zaten kilitlediysek tekrar deneme (hem log hem presence olayı
            // aynı anda tetikleyebilir; çift kilitlemeyi önler).
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
                // Kilitleme başarısız oldu (ör. pregame henüz hazır değil). Aynı pregame içinde
                // başka bir olay (presence/log) tekrar denesin diye bayrağı geri al.
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

            // Yeni bir kurulum: bu pregame için kilitlemeye yeniden izin ver (ajan değiştirilmiş olabilir).
            _hasLockedThisPregame = false;

            // Kilitlemeyi iki bağımsız kaynaktan tetikliyoruz:
            //  1) OnPreGameMatchLoaded  -> log tabanlı, hızlı ama tek seferlik/atlanabilir.
            //  2) OnGameStateChanged    -> presence/websocket tabanlı, her maçta güvenilir şekilde tetiklenir.
            // Abonelikler kalıcıdır; böylece bir kez kurulduktan sonra her maçta otomatik kilitlenir.
            // Önce çift abonelikten kaçınmak için kaldırıp tekrar ekliyoruz.
            AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded -= Controller.GameControlHandler;
            AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded += Controller.GameControlHandler;
            AppStateManager.initiator.TcpEvents.OnGameStateChanged -= Controller.GameStateWatcher;
            AppStateManager.initiator.TcpEvents.OnGameStateChanged += Controller.GameStateWatcher;

            AppStateManager.Instance.IsInstalockRunning = true;

            Log.Debug("[CreateInstalockGameHandler] State Manager güncellendi. Mevcut Oyun Aşaması: {CurrentPhase}", AppStateManager.Instance.CurrentPhase);

            // Halihazırda ajan seçimindeysek olayları beklemeden hemen kilitle.
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

            Log.Debug("[CancelControlHandler] Tüm pregame abonelikleri kaldırılıyor ve AppStateManager değerleri default olarak sıfırlanıyor.");
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


        // Presence/websocket tabanlı oyun aşaması izleyicisi. Log tabanlı OnPreGameMatchLoaded
        // olayının aksine her maçta güvenilir şekilde tetiklenir; bu yüzden hem ilk maçta hem de
        // sonraki tüm maçlarda kilitlemenin çalışmasını garanti eder. Ayrıca bozulma korumasını
        // (ajan seçimine geri düşme) da kapsar.
        public static void GameStateWatcher(string phase)
        {
            try
            {
                string currentPhase = phase.ToLower();
                Log.Information("[GameStateWatcher] Oyun aşaması: {Phase}", phase);

                if (currentPhase == "pregame")
                {
                    // Ajan seçimine girildi (veya bozulma sonrası geri dönüldü) -> kilitle.
                    GameControlHandler();
                }
                else
                {
                    // Ajan seçiminden çıkıldı (ingame/menus/idle). Bir sonraki pregame için
                    // kilitlemeye tekrar izin ver.
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