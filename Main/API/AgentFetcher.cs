using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Valorant_Instalocker.Utils.Constants;
using Valorant_Instalocker.Main;
using System.Windows;
using RadiantConnect.Methods;
using RadiantConnect.ValorantApi;
using Serilog;
using System.IO;
namespace Valorant_Instalocker.Main.API
{
    public class AgentFetcher
    {
        private static readonly HttpClient _client = new HttpClient();

        public record Agent(
            [property: JsonPropertyName("displayIcon")] string DisplayIconURL
        );

        public record AgentApiResponse(
            [property: JsonPropertyName("data")] Agent Data
        );

        private static async Task SaveAgentsToCache() {
            try
            {
                Log.Information("SaveAgentsToCache çalıştı.");

                if (AppStateManager.Agents.Count == 0)
                {
                    Log.Information("Agent daha çekilmemiş Çıkılıyor.");
                    return;
                }
                Directory.CreateDirectory(Constants.CacheImagesPath);

                foreach (var (Agent, AgentURL) in AppStateManager.Agents)
                {
                    using HttpResponseMessage response = await _client.GetAsync(AgentURL, HttpCompletionOption.ResponseHeadersRead);

                    response.EnsureSuccessStatusCode();


                    using Stream internetStream = await response.Content.ReadAsStreamAsync();
                    string SavePath = Path.Combine(Constants.CacheImagesPath, Agent.ToString()+".png");

                    using FileStream fileStream = new FileStream(SavePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);

                    await internetStream.CopyToAsync(fileStream);
                    AppStateManager.Agents[Agent] = SavePath;
                    Log.Information($"{Agent.ToString()}, Başarıyla Cachelendi.");

                }


            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
                Log.Error(ex, $"Ajan resimleri çekilemedi : Ajan reimleri sunucuda bulunamadı.");


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Hata oluştu");


            }


        }

        public static async Task UpdateAgents(bool UpdateOnline = false)
        {
            Log.Information("[UpdateAgents] Ajan güncelleme başlatılıyor. UpdateOnline: {UpdateOnline}", UpdateOnline);
            try
            {
                AppStateManager.Agents.Clear();

                if (UpdateOnline)
                {
                    foreach (ValorantTables.Agent agent in Enum.GetValues(typeof(ValorantTables.Agent)))
                    {
                        try
                        {
                            string FullUrl = Constants.AgentApiUrl + ValorantTables.AgentToId[agent];
                            HttpResponseMessage response = await _client.GetAsync(FullUrl);
                            if (!response.IsSuccessStatusCode)
                            {
                                Log.Warning("[UpdateAgents] API yanıtı başarısız. Ajan: {Agent}, Status: {Status}", agent, response.StatusCode);
                                continue;
                            }

                            string jsonString = await response.Content.ReadAsStringAsync();
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var result = JsonSerializer.Deserialize<AgentApiResponse>(jsonString, options);

                            if (result?.Data != null)
                            {
                                var API = result.Data;
                                AppStateManager.Agents.Add(agent, API.DisplayIconURL);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "[UpdateAgents] Ajan verisi çekilirken hata oluştu. Ajan: {Agent}", agent);
                        }
                    }
                    await SaveAgentsToCache();
                }
                else
                {
                    foreach (ValorantTables.Agent agent in Enum.GetValues(typeof(ValorantTables.Agent)))
                    {
                        try
                        {
                            string AgentPath = Path.Combine(Constants.CacheImagesPath, agent.ToString() + ".png");
                            if (!Path.Exists(AgentPath))
                            {
                                Log.Information("[UpdateAgents] Cache dosyası bulunamadı. Online güncelleme yapılıyor.");
                                await UpdateAgents(true);
                                return;
                            }
                            AppStateManager.Agents.Add(agent, AgentPath);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "[UpdateAgents] Cache dosyası işlenirken hata oluştu. Ajan: {Agent}", agent);
                        }
                    }
                }
                Log.Information("[UpdateAgents] Ajan güncelleme başarıyla tamamlandı. {AgentCount} ajan yüklendi.", AppStateManager.Agents.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[UpdateAgents] Ajan güncelleme kritik hatası");
                throw;
            }
        }
    }
}