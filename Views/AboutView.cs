using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Valorant_Instalocker.Utils.Constants;

namespace Valorant_Instalocker.Views
{
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void DevLink_Click(object sender, RoutedEventArgs e)
        {
            OpenURL(Constants.GithubUrl);
        }

        private void DiscordLink_Click(object sender, RoutedEventArgs e)
        {
            OpenURL(Constants.DiscordUrl);
        }

        private void TuiLink_Click(object sender, RoutedEventArgs e)
        {
            OpenURL(Constants.GithubInstalockerTUIURL);
        }

        private void OpenURL(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Bağlantı açılırken hata oluştu: {ex.Message}");
            }
        }
    }
}