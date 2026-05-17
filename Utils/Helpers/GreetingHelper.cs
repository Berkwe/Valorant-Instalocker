using RadiantConnect.Network.PVPEndpoints.DataTypes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Documents;
using Valorant_Instalocker.Main;
namespace Valorant_Instalocker.Utils.Helpers
{
    public static class GreetingHelper
    {
        static string CurrentDictLang;
        static public ResourceDictionary GreetingDict;
        static public List<string> GreetingDictKeys = new List<string>();
        static Random rn = new Random();

        static private System.Timers.Timer _timer;

        static private void GetGreetingDict()
        {
            Log.Information("[GreetingHelper.GetGreetingDict] Selamlaştırma sözlüğü yükleniyor. Dil: {Language}", AppStateManager.CurrentUserSettings.Language);
            try
            {
                CurrentDictLang = AppStateManager.CurrentUserSettings.Language;

                var uri = new Uri($"/Langs/GreetingTexts/Greeting_{CurrentDictLang}.xaml", UriKind.Relative);
                GreetingDict = (ResourceDictionary)Application.LoadComponent(uri);

                if (GreetingDict == null || GreetingDict.Count == 0)
                {
                    Log.Warning("[GreetingHelper.GetGreetingDict] Selamlaştırma sözlüğü yüklenemedi veya boş: {Language}", CurrentDictLang);
                    return;
                }

                GreetingDictKeys = GreetingDict.Keys.Cast<object>()
                    .Select(k => k.ToString())
                    .ToList();

                Log.Information("[GreetingHelper.GetGreetingDict] Selamlaştırma sözlüğü başarıyla yüklendi. Toplam anahtar: {KeyCount}", GreetingDictKeys.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GreetingHelper.GetGreetingDict] Selamlaştırma sözlüğü yüklenirken hata oluştu. Dil: {Language}", CurrentDictLang);
                GreetingDictKeys = new List<string>();
                GreetingDict = null;
            }
        }


        static public void StartToGreetingTimer()
        {
            Log.Information("[GreetingHelper.StartToGreetingTimer] Selamlaştırma timer başlatılıyor.");
            try
            {
                GreetingHelper.SetRandomGreetingMessage(null, null);
                _timer = new System.Timers.Timer(AppStateManager.CurrentUserSettings.GreetingDurationSeconds * 1000);

                _timer.Elapsed += SetRandomGreetingMessage;

                _timer.AutoReset = true;

                _timer.Start();
                Log.Information("[GreetingHelper.StartToGreetingTimer] Timer başarıyla başlatıldı. Interval: {Interval}ms", AppStateManager.CurrentUserSettings.GreetingDurationSeconds * 1000);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GreetingHelper.StartToGreetingTimer] Timer başlatılırken hata oluştu");
            }
        }


        static private void SetRandomGreetingMessage(object sender, ElapsedEventArgs e)
        {
            Log.Debug("[GreetingHelper.SetRandomGreetingMessage] Rastgele selamlaştırma mesajı ayarlanıyor.");
            try
            {
                if (GreetingDict == null || GreetingDict.Count == 0 || AppStateManager.CurrentUserSettings.Language != CurrentDictLang)
                {
                    Log.Debug("[GreetingHelper.SetRandomGreetingMessage] Sözlük yeniden yükleniyor.");
                    GetGreetingDict();
                }

                if (GreetingDictKeys == null || GreetingDictKeys.Count == 0)
                {
                    Log.Warning("[GreetingHelper.SetRandomGreetingMessage] Kullanılabilir selamlaştırma anahtarı yok");
                    return;
                }

                int RandomIndex = rn.Next(GreetingDictKeys.Count);
                var SelectedGreetingKey = GreetingDictKeys[RandomIndex];
                AppStateManager.Instance.ActiveGreetingMessage = GreetingDict[SelectedGreetingKey].ToString();
                Log.Debug("[GreetingHelper.SetRandomGreetingMessage] Selamlaştırma mesajı başarıyla ayarlandı. Anahtar: {Key}", SelectedGreetingKey);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[GreetingHelper.SetRandomGreetingMessage] Selamlaştırma mesajı ayarlanırken hata oluştu");
            }
        }
    }
}
