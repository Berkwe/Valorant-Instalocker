using RadiantConnect;
using RadiantConnect.Methods;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Valorant_Instalocker.Main;
using Valorant_Instalocker.Main.API;
using Valorant_Instalocker.Main.Game;
namespace Valorant_Instalocker.Views
{
    public partial class DashboardView : UserControl
    {
        private string _selectedAgent = "";
        public static readonly IReadOnlyDictionary<string, ValorantTables.Agent> IdToAgent = ValorantTables.AgentToId.ToDictionary(pair => pair.Value, pair => pair.Key);
        public DashboardView()
        {
            InitializeComponent();
            InitializeDashboardScreen();

            ActionBtn.IsEnabled = false;

            Application.Current.Dispatcher.Invoke(() => {
                AppStateManager.OnlySelect = OnlySelectCheckBox.IsChecked ?? false;
            });
        }

        private async void InitializeDashboardScreen()
        {
            try
            {
                await AgentFetcher.UpdateAgents();
                foreach (ValorantTables.Agent agent in AppStateManager.Agents.Keys)
                {
                    AddAgentImageToDashboard(AppStateManager.Agents[agent], ValorantTables.AgentToId[agent]);
                }

            }
            catch (Exception ex)
            {
            }
        }




        private void AddAgentImageToDashboard(string url, string name)
        {
            Border card = new Border { Style = (Style)FindResource("AgentCardStyle") };
            Image img = new Image
            {
                Source = new BitmapImage(new Uri(url, UriKind.Absolute)),
                Stretch = System.Windows.Media.Stretch.UniformToFill,
                Margin = new Thickness(2)
            };

            card.Child = img;
            card.Tag = name;
            card.MouseDown += AgentCard_Click;

            AgentsContainer.Children.Add(card);
        }

        private void AgentCard_Click(object sender, MouseButtonEventArgs e)
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
            ActionBtn.IsEnabled = true;
            var selectedName = ValorantTables.AgentIdToAgent[_selectedAgent].ToString();
            ActionBtn.Content = OnlySelectCheckBox.IsChecked == true ? $"{selectedName} SEÇ!" : $"{selectedName} KİLİTLE";
        }




        private void OnlySelectCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (ActionBtn == null) return;

            if (OnlySelectCheckBox.IsChecked == true)
            {
                ActionBtn.Content = "SADECE SEÇ";
            }
            else
            {
                ActionBtn.Content = "KİLİTLE";
            }
        }

        private void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            AppStateManager.SelectedAgent = IdToAgent[_selectedAgent];
            AppStateManager.initiator.GameEvents.PreGame.OnPreGameMatchLoaded += Controller.GameControlHandler;
        }

    }
}