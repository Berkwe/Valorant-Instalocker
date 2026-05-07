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

            public static async Task ConnectValorant(IProgress<string> progress = null)
            {

                Log.Information("Valorant istemcisine bağlanılmaya çalışılıyor...");
                progress?.Report("Valorant bekleniyor...");

            try
            {
                AppStateManager.initiator = await Task.Run(() =>
                {
                    return new Initiator(ignoreVpn: true);
                });
                AppStateManager.Instance.IsClientLoggedIn = true;
                progress?.Report("Bağlantı başarılı!");
                Log.Information("RadiantConnect başarıyla başlatıldı.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Initiator başlatılırken hata oluştu.");
                progress?.Report("Hata: İstemci bulunamadı.");
                throw;

            }
        }
    }
}
