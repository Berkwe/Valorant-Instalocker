using RadiantConnect;
using System.Configuration;
using System.Data;
using System.Windows;
using Velopack;
namespace Valorant_Instalocker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            VelopackApp.Build().Run();
            base.OnStartup(e);
        }
    }

}
