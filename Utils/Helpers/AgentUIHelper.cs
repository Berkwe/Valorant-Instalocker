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
    }
}