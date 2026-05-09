using RadiantConnect;
using Newtonsoft.Json.Linq;
using RadiantConnect.RConnect;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Valorant_Instalocker.Utils.Configs;
using System.Text.Json.Nodes;
namespace Valorant_Instalocker.Main.Game
{

    internal class Connection
    {

            public static async Task<string> GetUserName() {
                dynamic? RSOInfo = await AppStateManager.initiator.Endpoints.LocalEndpoints.GetRSOInfoAsync();
                var outerJson = JObject.Parse(RSOInfo.ToString());
                var innerJson = JObject.Parse(outerJson["userInfo"].ToString());
                string username = innerJson["preferred_username"]?.ToString();
                return username ?? "Bilinmiyor";


            }

            

            public static async Task<string> GetCurrentPhase() {
                var phase = "none";
                var sessions = await AppStateManager.initiator.Endpoints.LocalEndpoints.GetLocalSessionsAsync();
                var json = JsonSerializer.Serialize(sessions);
                var doc = JsonDocument.Parse(json);
                
                foreach (var session in doc.RootElement.EnumerateObject())
                {
                    if (!session.Value.TryGetProperty("productId", out JsonElement productId) || productId.GetString() != "valorant") continue;

                    phase = session.Value.GetProperty("phase").ToString();
                }

                return phase;

        }

        public static async Task<int> GetCurrentPlayerLevel()
        {
            var data = await AppStateManager.initiator.Endpoints.PvpEndpoints.FetchAccountXPAsync();
            var json = JsonSerializer.Serialize(data);
            var doc = JsonDocument.Parse(json);
            var level = doc.RootElement.GetProperty("Progress").GetProperty("Level").ToString();
            Log.Information("Kullanıcı seviyesi çekildi : ", level);
            return int.Parse(level);
        }


        public static async Task ConnectValorant()
            {

                Log.Information("Valorant istemcisine bağlanılmaya çalışılıyor...");

            try
            {
                AppStateManager.initiator = await Task.Run(() =>
                {
                    return new Initiator(ignoreVpn: true);
                });
                AppStateManager.Instance.IsClientLoggedIn = true;
                Log.Information("RadiantConnect başarıyla başlatıldı.");
            }
            catch (System.TimeoutException ex)
            {
                Log.Error(ex, "Giriş yapılırken hata: Valorant açık değil.");
            }
            catch (Exception ex)
            {
                Log.Error("Giriş yapılırken Bir Hata Oluştu : ", ex);
                throw;
            }
        }
    }
}
