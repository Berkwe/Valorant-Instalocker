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

        private static int FailureTimes = 0;
        public static async Task<string> GetUserName()
        {
            Log.Information("GetUserName fonksiyonu çalışmaya başladı.");
            try
            {
                var data = await AppStateManager.initiator.Endpoints.LocalEndpoints.GetAliasInfoAsync();
                Log.Information(data.ToString());
                var json = JsonSerializer.Serialize(data);
                var doc = JsonDocument.Parse(json);
                var username = doc.RootElement.GetProperty("game_name").ToString();

                var result = username ?? "Bilinmiyor";
                Log.Information("GetUserName fonksiyonu başarıyla tamamlandı. Döndürülen değer: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetUserName fonksiyonunda kritik bir hata oluştu.");
                throw;
            }
        }

        public static async Task<string> GetCurrentPhase()
        {
            Log.Information("GetCurrentPhase fonksiyonu çalışmaya başladı.");
            try
            {
                var phase = "none";
                var sessions = await AppStateManager.initiator.Endpoints.LocalEndpoints.GetLocalSessionsAsync();
                var json = JsonSerializer.Serialize(sessions);
                var doc = JsonDocument.Parse(json);

                foreach (var session in doc.RootElement.EnumerateObject())
                {
                    if (!session.Value.TryGetProperty("productId", out JsonElement productId) || productId.GetString() != "valorant") continue;

                    phase = session.Value.GetProperty("phase").ToString();
                }

                Log.Information("GetCurrentPhase fonksiyonu başarıyla tamamlandı. Döndürülen değer: {Phase}", phase);
                return phase;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetCurrentPhase fonksiyonunda kritik bir hata oluştu.");
                throw;
            }
        }

        public static async Task<int> GetCurrentPlayerLevel()
        {
            Log.Information("GetCurrentPlayerLevel fonksiyonu çalışmaya başladı.");
            try
            {
                var data = await AppStateManager.initiator.Endpoints.PvpEndpoints.FetchAccountXPAsync();
                var json = JsonSerializer.Serialize(data);
                var doc = JsonDocument.Parse(json);
                var level = doc.RootElement.GetProperty("Progress").GetProperty("Level").ToString();
                Log.Information("Kullanıcı seviyesi çekildi : {Level}", level);

                var result = int.Parse(level);
                Log.Information("GetCurrentPlayerLevel fonksiyonu başarıyla tamamlandı. Döndürülen değer: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetCurrentPlayerLevel fonksiyonunda kritik bir hata oluştu.");
                throw;
            }
        }

        public static async Task ConnectValorant()
        {
            Log.Information("ConnectValorant fonksiyonu çalışmaya başladı.");
            Log.Information("Valorant istemcisine bağlanılmaya çalışılıyor...");

            try
            {
                AppStateManager.initiator = await Task.Run(() =>
                {
                    return new Initiator(ignoreVpn: true);
                });
                AppStateManager.Instance.IsClientLoggedIn = true;
                Log.Information("RadiantConnect başarıyla başlatıldı.");
                Log.Information("ConnectValorant fonksiyonu başarıyla tamamlandı.");
            }
            catch (System.TimeoutException ex)
            {
                if (FailureTimes > 5)
                {
                    Log.Error(ex, "Giriş yapılırken hata: Valorant açık değil, 5 kez denendi. ConnectValorant fonksiyonu başarısız oldu.");
                    return;
                }
                FailureTimes++;
                Log.Information($"Giriş başarısız, Tekrar deneniyor. Deneme sayısı : {FailureTimes.ToString()}");
                await ConnectValorant();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Giriş yapılırken Bir Hata Oluştu. ConnectValorant fonksiyonu kritik bir hata ile sonlandı.");
                throw;
            }
        }
    }
}