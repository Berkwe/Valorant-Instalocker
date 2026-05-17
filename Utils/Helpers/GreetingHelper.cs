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
            CurrentDictLang = AppStateManager.CurrentUserSettings.Language;

            var uri = new Uri($"/Langs/GreetingTexts/Greeting_{CurrentDictLang}.xaml", UriKind.Relative);
            GreetingDict = (ResourceDictionary)Application.LoadComponent(uri);

            if (GreetingDict == null || GreetingDict.Count == 0)
            {
                Log.Warning($"Greeting dictionary yüklenemedi: {CurrentDictLang}");
                return;
            }

            GreetingDictKeys = GreetingDict.Keys.Cast<object>()
                .Select(k => k.ToString())
                .ToList();

            Log.Information($"Greeting dict yüklendi, {GreetingDictKeys.Count} key.");
        }


        static public void StartToGreetingTimer()
        {
            GreetingHelper.SetRandomGreetingMessage(null, null);
            _timer = new System.Timers.Timer(AppStateManager.CurrentUserSettings.GreetingDurationSeconds * 1000);

            _timer.Elapsed += SetRandomGreetingMessage;

            _timer.AutoReset = true;

            _timer.Start();
        }


        static private void SetRandomGreetingMessage(object sender, ElapsedEventArgs e)
        {
            if (GreetingDict == null || GreetingDict.Count == 0 || AppStateManager.CurrentUserSettings.Language != CurrentDictLang)
            {
                GetGreetingDict();
            }
            int RandomIndex = rn.Next(GreetingDictKeys.Count);
            var SelectedGreetingKey = GreetingDictKeys[RandomIndex];
            AppStateManager.Instance.ActiveGreetingMessage = GreetingDict[SelectedGreetingKey].ToString();
        }
    }
}
