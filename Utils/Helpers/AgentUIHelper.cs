using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Valorant_Instalocker.Main.Helpers
{
    public static class AgentUIHelper
    {
        public static Border CreateAgentCard(string url, string name, MouseButtonEventHandler clickHandler, Style cardStyle)
        {
            Border card = new Border
            {
                Style = cardStyle,
                Tag = name
            };

            BitmapImage bitmap = new BitmapImage();

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
            {
                if (uriResult.IsFile)
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = uriResult;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                else
                {
                    bitmap = new BitmapImage(uriResult);
                }
            }
            else
            {
                bitmap = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
            }

            Image img = new Image
            {
                Source = bitmap,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(2)
            };

            card.Child = img;
            card.MouseDown += clickHandler;

            return card;
        }
    }
}