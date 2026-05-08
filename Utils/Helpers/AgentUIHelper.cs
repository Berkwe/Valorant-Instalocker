using RadiantConnect.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Valorant_Instalocker.Main;

namespace Valorant_Instalocker.Utils.Helpers
{
    public static class AgentUIHelper
    {
        public static Border CreateAgentCard(string url, string name, MouseButtonEventHandler clickHandler, Style cardStyle)
        {
            Border card = new Border { Style = cardStyle, Tag = name };
            Image img = new Image
            {
                Source = new BitmapImage(new Uri(url, UriKind.Absolute)),
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(2)
            };
            card.Child = img;
            card.MouseDown += clickHandler;
            return card;
        }

        private static void AgentCard_Click(object sender, MouseButtonEventArgs e) // EN SON BU SOKTUĞUMUNUN DASHBOARDINI TEMZİLEYİP HELPERE ATIYODUN
        {
            Border selected = sender as Border;
            foreach (Border b in AgentsContainer.Children)
            {
                b.BorderBrush = System.Windows.Media.Brushes.Transparent;
                b.Effect = null;
                b.RenderTransform = new ScaleTransform(1, 1);
            }
            if (selected.Tag.ToString() == _selectedAgent)
            {
                _selectedAgent = "";
                ActionBtn.IsEnabled = false;
                ActionBtn.Content = OnlySelectCheckBox.IsChecked == true ? $"SEÇ!" : $"KİLİTLE";
                return;
            }
            selected.BorderBrush = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#EF4444");
            selected.RenderTransform = new ScaleTransform(1.15, 1.15);
            selected.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = System.Windows.Media.Color.FromRgb(239, 68, 68),
                BlurRadius = 20,
                ShadowDepth = 0,
                Opacity = 0.8
            };

            _selectedAgent = selected.Tag.ToString();
            if (AppStateManager.Instance.IsClientLoggedIn) ActionBtn.IsEnabled = true;
            var selectedName = ValorantTables.AgentIdToAgent[_selectedAgent].ToString();
            ActionBtn.Content = OnlySelectCheckBox.IsChecked == true ? $"{selectedName} SEÇ!" : $"{selectedName} KİLİTLE";
        }
    }
}
