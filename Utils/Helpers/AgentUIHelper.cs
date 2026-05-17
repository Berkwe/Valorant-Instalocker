using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Serilog;

namespace Valorant_Instalocker.Main.Helpers
{
    public static class AgentUIHelper
    {
        public static Border CreateAgentCard(string url, string name, MouseButtonEventHandler clickHandler, Style cardStyle)
        {
            Log.Information("[AgentUIHelper.CreateAgentCard] Ajan kartı oluşturuluyor. Ajan: {AgentName}, URL: {URL}", name, url);
            try
            {
                Border card = new Border
                {
                    Style = cardStyle,
                    Tag = name
                };

                BitmapImage bitmap = new BitmapImage();

                try
                {
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
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "[AgentUIHelper.CreateAgentCard] Görüntü yüklenirken hata oluştu. Ajan: {AgentName}, URL: {URL}", name, url);
                    // Hata durumunda boş bir bitmap kullanıyoruz
                    bitmap = new BitmapImage();
                }

                Image img = new Image
                {
                    Source = bitmap,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(2)
                };

                card.Child = img;
                card.MouseDown += clickHandler;

                Log.Debug("[AgentUIHelper.CreateAgentCard] Ajan kartı başarıyla oluşturuldu. Ajan: {AgentName}", name);
                return card;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[AgentUIHelper.CreateAgentCard] Ajan kartı oluşturulurken kritik hata oluştu. Ajan: {AgentName}", name);
                // Hata durumunda boş bir border döneriz
                return new Border { Tag = name, Style = cardStyle };
            }
        }
    }
}