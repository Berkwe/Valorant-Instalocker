using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace Valorant_Instalocker.Main.API
{
    internal class UpdateApplication
    {


        public static async Task CheckForUpdates()
        {
            try
            {
                var mgr = new UpdateManager(new GithubSource(Utils.Constants.Constants.GithubUrl, null, false));
                var newVersion = await mgr.CheckForUpdatesAsync();

                if (newVersion == null)
                {
                    Log.Information("Güncelleme yok.");
                    return;
                }

                Log.Information($"Yeni sürüm bulundu: {newVersion.TargetFullRelease.Version}");
                await mgr.DownloadUpdatesAsync(newVersion);
                mgr.ApplyUpdatesAndRestart(newVersion);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Güncelleme kontrolü başarısız.");
            }
        }
    }
}
