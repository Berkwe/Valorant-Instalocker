using RadiantConnect.Network.PartyEndpoints.DataTypes;
using Serilog;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Valorant_Instalocker.Views;

namespace Valorant_Instalocker.Utils.Configs
{
    class ConfigManager
    {
        public static string filePath = "settings.json";

        public static void SaveSettings(UserSettings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true  });
            File.WriteAllText(filePath, json);
        }

        public static UserSettings LoadSettings() {

            if (!File.Exists(filePath)) { 
                return new UserSettings();
            }
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();

        }

        public static UserSettings? InitializeSettings()
        {
            try {
                var CurrentUserSettings = ConfigManager.LoadSettings();
                SettingsView settingsView = new SettingsView();
                settingsView.ApplySettings(CurrentUserSettings);
                Log.Information("Kullanıcı ayarları çekildi : {UserSettings}", CurrentUserSettings);
                return CurrentUserSettings;
            }
            catch (Exception ex) {
                Log.Error(ex, "Ayarlar Yüklenirken Bir Hata Oluştu.");
                return null;
            }
        }
    }
}
