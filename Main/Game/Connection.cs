using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiantConnect;
namespace Valorant_Instalocker.Main.Game
{

    internal class Connection
    {
        public static Initiator ConnectValorant() {
            return new Initiator();
        }
    }
}
