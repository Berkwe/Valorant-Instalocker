using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Valorant_Instalocker.Main;
using Valorant_Instalocker.Utils.Configs;
using Valorant_Instalocker.Views;
namespace Valorant_Instalocker
{
    public partial class MainWindow : Window
    {
        private string CurrentTab;
        public MainWindow()
        {
            InitializeComponent();
            AppStateManager.Initialize();
            MainContent.Content = new Views.DashboardView();
            CurrentTab = "BtnNavDashboard";

        }

        private void SwitchTab_Click(object sender, RoutedEventArgs e)
        {


            if (CurrentTab == "BtnNavSettings")
            {
                UserSettings savedUserSettings = ConfigManager.LoadSettings();

                if (savedUserSettings != AppStateManager.CurrentUserSettings)
                {
                    var MsgBox = new CustomMessageBox("Kaydedilmeyen değişiklikler var, çıkmak istediğinizden emin misiniz?", "Dikkat!");

                    MsgBox.Owner = Application.Current.MainWindow;

                    MsgBox.ShowDialog();

                    if (!MsgBox.Result) return;

                    SettingsView settingsView = new SettingsView();
                    AppStateManager.CurrentUserSettings = savedUserSettings;
                    settingsView.ApplySettings(savedUserSettings);
                }

            }

            var clickedButton = sender as Button;
            if (clickedButton == null) return;

            BtnNavDashboard.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            BtnNavDashboard.BorderBrush = new SolidColorBrush(Colors.Transparent);
            BtnNavMacros.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            BtnNavMacros.BorderBrush = new SolidColorBrush(Colors.Transparent);
            BtnNavSettings.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            BtnNavSettings.BorderBrush = new SolidColorBrush(Colors.Transparent);
            BtnNavAbout.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            BtnNavAbout.BorderBrush = new SolidColorBrush(Colors.Transparent);

            clickedButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
            clickedButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));

            CurrentTab = clickedButton.Name;
            switch (clickedButton.Name)
            {
                case "BtnNavDashboard":
                    MainContent.Content = new Views.DashboardView();
                    break;
                case "BtnNavMacros":
                    MainContent.Content = new Views.MacrosView();
                    break;
                case "BtnNavSettings":
                    MainContent.Content = new Views.SettingsView();
                    if (MainContent.Content is SettingsView settingsView) settingsView.ApplySettings();
                    break;
                case "BtnNavAbout":
                    MainContent.Content = new Views.AboutView();
                    break;
                default:
                    break;
            }

        }
    }
}