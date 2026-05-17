using RadiantConnect.Network.PartyEndpoints.DataTypes;
using Serilog;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Valorant_Instalocker.Main;
using Valorant_Instalocker.Views;
using Valorant_Instalocker.Utils.Constants;
namespace Valorant_Instalocker.Utils.Configs
{
    class ConfigManager
    {
                

        public static void SaveSettings(UserSettings settings)
        {
            Log.Information("[ConfigManager.SaveSettings] Kullanıcı ayarları kaydediliyor.");
            try
            {
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true  });
                File.WriteAllText(Constants.Constants.SettingsPath, json);
                Log.Information("[ConfigManager.SaveSettings] Ayarlar başarıyla kaydedildi. Dosya: {Constants.Constants.SettingsPath}", Constants.Constants.SettingsPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error(ex, "[ConfigManager.SaveSettings] Dosya erişim hatası. Dosya korumalı olabilir: {Constants.Constants.SettingsPath}", Constants.Constants.SettingsPath);
                throw;
            }
            catch (System.IO.IOException ex)
            {
                Log.Error(ex, "[ConfigManager.SaveSettings] Dosya yazma hatası: {Constants.Constants.SettingsPath}", Constants.Constants.SettingsPath);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ConfigManager.SaveSettings] Ayarlar kaydedilirken kritik hata oluştu");
                throw;
            }
        }

        public static UserSettings LoadSettings()
        {
            Log.Information("[ConfigManager.LoadSettings] Kullanıcı ayarları yükleniyor. Dosya: {Constants.Constants.SettingsPath}", Constants.Constants.SettingsPath);
            try
            {
                if (!File.Exists(Constants.Constants.SettingsPath))
                {
                    Log.Warning("[ConfigManager.LoadSettings] Ayarlar dosyası bulunamadı. Varsayılan ayarlar kullanılıyor: {Constants.Constants.SettingsPath}", Constants.Constants.SettingsPath);
                    return new UserSettings();
                }

                string json = File.ReadAllText(Constants.Constants.SettingsPath);
                var settings = JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
                Log.Information("[ConfigManager.LoadSettings] Ayarlar başarıyla yüklendi.");
                return settings;
            }
            catch (System.IO.IOException ex)
            {
                Log.Error(ex, "[ConfigManager.LoadSettings] Dosya okuma hatası: {Constants.Constants.SettingsPath}", Constants.Constants.SettingsPath);
                return new UserSettings();
            }
            catch (System.Text.Json.JsonException ex)
            {
                Log.Error(ex, "[ConfigManager.LoadSettings] JSON formatı hatalı. Dosya: {Constants.Constants.SettingsPath}. Varsayılan ayarlar kullanılıyor.", Constants.Constants.SettingsPath);
                return new UserSettings();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ConfigManager.LoadSettings] Ayarlar yüklenirken kritik hata oluştu");
                return new UserSettings();
            }
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
