using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Valorant_Instalocker.Utils.Constants;
using Valorant_Instalocker.Main;
using System.Windows;
using RadiantConnect.Methods;
using RadiantConnect.ValorantApi;
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

        public static async Task UpdateAgents()
        {
            try
            {
                AppStateManager.Agents.Clear();
                foreach (ValorantTables.Agent agent in Enum.GetValues(typeof(ValorantTables.Agent)))
                {
                    string FullUrl = Constants.AgentApiUrl + ValorantTables.AgentToId[agent];
                    HttpResponseMessage response = await _client.GetAsync(FullUrl);
                    if (!response.IsSuccessStatusCode) return;

                    string jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<AgentApiResponse>(jsonString, options);

                    if (result?.Data != null)
                    {
                        var API = result.Data;

                        AppStateManager.Agents.Add(agent, API.DisplayIconURL);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}