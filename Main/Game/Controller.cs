using RadiantConnect.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Serilog;

namespace Valorant_Instalocker.Main.Game
{
    internal class Controller
    {
        private static async Task LockAgent()
        {
            Log.Information("[LockAgent] Ajan kilitleme işlemi başlatılıyor. Hedef Ajan ID: {AgentID}", AppStateManager.SelectedAgent);
            await AppStateManager.initiator.Endpoints.PreGameEndpoints.LockCharacterAsync(AppStateManager.SelectedAgent);
            Log.Information("[LockAgent] Ajan kilitleme isteği başarıyla tamamlandı.");
        }

        private static async Task SelectAgent()
        {
            Log.Information("[SelectAgent] Ajan hover (seçme) işlemi başlatılıyor. Hedef Ajan ID: {AgentID}", AppStateManager.SelectedAgent);
            await AppStateManager.initiator.Endpoints.PreGameEndpoints.SelectCharacterAsync(AppStateManager.SelectedAgent);
            Log.Information("[SelectAgent] Ajan hover (seçme) isteği başarıyla tamamlandı.");
        }

        public static async void GameControlHandler(string MatchID = "")
        {
            Log.Information("[GameControlHandler] Tetiklendi. MatchID: {MatchID}, OnlySelect Modu: {OnlySelect}", MatchID, AppStateManager.OnlySelect);
            try
            {
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

                Log.Debug("[GameControlHandler] OnPreGameMatchLoaded olayından abonelik kaldırılıyor.");
                AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded -= Controller.GameControlHandler;

                AppStateManager.Instance.IsInstalockRunning = false;
                Log.Information("[GameControlHandler] İşlem döngüsü sorunsuz tamamlandı. IsInstalockRunning: false");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GameControlHandler] Ajan seçimi veya kilitlenmesi sırasında kritik bir API/Sistem hatası oluştu. MatchID: {MatchID}", MatchID);
            }
        }


        public static async void CreateInstalockGameHandler(ValorantTables.Agent SelectedAgent, string SelectedAgentName, string SelectedAgentImageURL)
        {
            Log.Information("[CreateInstalockGameHandler] Kurulum başlatıldı. Seçilen Ajan: {AgentName}", SelectedAgentName);

            if (AppStateManager.Instance.IsInstalockRunning)
            {
                Log.Warning("[CreateInstalockGameHandler] Halihazırda çalışan bir işlem tespit edildi. Eski abonelik iptal ediliyor.");
                AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded -= Controller.GameControlHandler;
            }

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
            AppStateManager.Instance.IsInstalockRunning = false;

            Log.Information("[CancelControlHandler] İptal işlemi başarıyla tamamlandı. State sıfırlandı.");
        }
    }
}