using RadiantConnect.Methods;
using RadiantConnect.Services;
using RadiantConnect.ValorantApi;
using Serilog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Valorant_Instalocker.Main;
using Valorant_Instalocker.Main.API;
using Valorant_Instalocker.Main.Game;
using Valorant_Instalocker.Main.Helpers;
using static RadiantConnect.ValorantApi.Agents;
namespace Valorant_Instalocker.Views
{
    public partial class DashboardView : UserControl
    {
        private string _selectedAgent = "";
        private Border _activeCard = null;
        private readonly Brush _selectColor = (Brush)new BrushConverter().ConvertFrom("#EF4444");
        private static string GetPhaseString(string key) =>
            Application.Current.FindResource(key) as string ?? key;

        public static string TranslatePhase(string phase) => GetPhaseString(phase.ToLower() switch
        {
            "ıdle" or "menus" or "default" => "PhaseIdle",
            "pregame" => "PhasePregame",
            "ingame" or "gameplay" => "PhaseIngame",
            _ => "PhaseUnknown"
            
        });


        public static readonly IReadOnlyDictionary<string, ValorantTables.Agent> NameToAgent =
            ValorantTables.AgentToId.ToDictionary(
                pair => pair.Key.ToString(),
                pair => pair.Key,
                StringComparer.OrdinalIgnoreCase
            );




        public DashboardView()
        {
            InitializeComponent();
            LoadAgents();
            UpdateUIState(null, null);
        }

        private async void LoadAgents()
        {
            try
            {
                await AgentFetcher.UpdateAgents();
                var style = (Style)FindResource("AgentCardStyle");

                foreach (var agent in AppStateManager.Agents)
                {
                    var card = AgentUIHelper.CreateAgentCard(agent.Value, agent.Key.ToString(), AgentCard_Click, style);
                    AgentsContainer.Children.Add(card);
                }
            }
            catch (Exception ex)
            {

                Log.Error(ex.ToString());

            }
        }

        private void AgentCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is Border clickedCard)) return;

            if (_activeCard != null) _activeCard.BorderBrush = Brushes.Transparent;

            if (_activeCard == clickedCard)
            {
                _activeCard = null;
                _selectedAgent = null;
                if (ActionBtn.IsEnabled) ActionBtn.IsEnabled = false;
            }
            else
            {
                if (AppStateManager.Instance.IsClientLoggedIn) ActionBtn.IsEnabled = true;
                _activeCard = clickedCard;
                _activeCard.BorderBrush = _selectColor;
                _selectedAgent = _activeCard.Tag.ToString();
            }

            UpdateUIState(null, null);
        }

        private void UpdateUIState(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedAgent))
            {
                ActionBtn.Content = "AJAN SEÇİN";
                return;
            }

            string name = _selectedAgent.ToUpper();
            string mode = OnlySelectCheckBox.IsChecked == true ? "SEÇ" : "KİLİTLE";
            ActionBtn.Content = $"{name} {mode}";
        }






        private void OnlySelectCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (ActionBtn == null) return;

            if (OnlySelectCheckBox.IsChecked == true)
            {
                AppStateManager.OnlySelect = true;
                ActionBtn.Content = "SADECE SEÇ";
            }
            else
            {
                AppStateManager.OnlySelect = false;
                ActionBtn.Content = "KİLİTLE";
            }
        }


        private void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            var SelectedAgent = NameToAgent[_selectedAgent];
            var SelectedAgentName = _selectedAgent;
            var SelectedAgentImageURL = AppStateManager.Agents[SelectedAgent].ToString();
            Controller.CreateInstalockGameHandler(SelectedAgent, SelectedAgentName, SelectedAgentImageURL);
        }

        private void CancelInstalockAction_Click(object sender, RoutedEventArgs e)
        {
            Controller.CancelControlHandler();
        }
    }
}