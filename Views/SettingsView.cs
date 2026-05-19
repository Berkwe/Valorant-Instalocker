using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Valorant_Instalocker.Utils.Constants;
using Valorant_Instalocker.Main;
using Valorant_Instalocker.Utils.Configs;

namespace Valorant_Instalocker.Views
{
    public partial class SettingsView : UserControl
    {
        private bool isLoaded = false;
        
        public SettingsView()
        {
            InitializeComponent();
            isLoaded = true;
        }

        public void ApplySettings(UserSettings? SettingsToApply = null)
        {
            SettingsToApply ??= AppStateManager.CurrentUserSettings;

            ChangeLanguage(SettingsToApply.Language);
            ChangeTheme(SettingsToApply.Theme);
        }


        public void ChangeTheme(string ThemeCode)
        {

            var dicts = Application.Current.Resources.MergedDictionaries;
            var newTheme = new ResourceDictionary
            {
                Source = new Uri($"/Views/Themes/Theme_{ThemeCode}.xaml", UriKind.Relative)
            };
            foreach (ComboBoxItem item in ThemeComboBox.Items)
            {
                if (item.Tag?.ToString() == ThemeCode) ThemeComboBox.SelectedItem = item;
            }

            dicts.Add(newTheme);

            for (int i = 0; i < dicts.Count; i++)
            {
                if (dicts[i].Source.OriginalString.Contains("Theme_") && dicts[i] != newTheme)
                {
                    dicts.RemoveAt(i);
                    i--;
                }
            }

        }

        public void ChangeLanguage(string langCode)
        {
            var dicts = Application.Current.Resources.MergedDictionaries;
            var newLang = new ResourceDictionary
            {
                Source = new Uri($"Assets/Langs/Lang_{langCode}.xaml", UriKind.Relative)
            };
            foreach  (ComboBoxItem item in LanguageComboBox.Items)
            {
                if (item.Tag?.ToString() == langCode) LanguageComboBox.SelectedItem = item;
            }
            
            dicts.Add(newLang);

            for (int i = 0; i < dicts.Count; i++)
            {
                if (dicts[i].Source.OriginalString.Contains("Langs") && dicts[i] != newLang)
                {
                    dicts.RemoveAt(i);
                    i--;
                }
            }

        }

        private void LanguageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isLoaded) return;
            var selected = LanguageComboBox.SelectedItem as ComboBoxItem;

            if (selected == null) return;

            string selectedLanguage = selected.Tag.ToString();

            if (string.IsNullOrEmpty(selectedLanguage) || !Languages.AvailableLanguages.Contains(selectedLanguage)) return;

            AppStateManager.CurrentUserSettings.Language = selectedLanguage;

            ApplySettings();
        }

        private void SettingsSaveBtn_Click(object sender, EventArgs e)
        {
            ConfigManager.SaveSettings(AppStateManager.CurrentUserSettings);
        }

        private void ThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!isLoaded) return;
            var selected = ThemeComboBox.SelectedItem as ComboBoxItem;

            if (selected == null) return;

            string selectedTheme = selected.Tag.ToString();

            if (string.IsNullOrEmpty(selectedTheme)) return;

            AppStateManager.CurrentUserSettings.Theme = selectedTheme;

            ApplySettings();


        }

    }

}
