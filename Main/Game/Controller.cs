using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Valorant_Instalocker.Main.Game
{
    internal class Controller
    {
        private static async Task LockAgent()
        {
            await AppStateManager.initiator.Endpoints.PreGameEndpoints.LockCharacterAsync(AppStateManager.SelectedAgent);
        }

        private static async Task SelectAgent()
        {
            await AppStateManager.initiator.Endpoints.PreGameEndpoints.SelectCharacterAsync(AppStateManager.SelectedAgent);
        }
        public static async void GameControlHandler(string MatchID)
        {
            try
            {
                if (AppStateManager.OnlySelect)
                { 
                    await SelectAgent();
                }
                else
                {
                    await LockAgent();
                }
            }
            catch (Exception ex) {
            
            }

        }
    }
}
