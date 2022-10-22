using Microsoft.AspNetCore.Mvc;

namespace GloryBot.Controllers
{
    public class UpdateController : Controller
    {
        private string versionNumber { get; set; } = "";
        public IActionResult Index()
        {
            
                CheckForUpdate();
            
            return View();
        }
        private int trigger = 0;


        private async Task<string> CheckForUpdate()
        {
            var autoUpdater = Electron.AutoUpdater;
            autoUpdater.AutoDownload = false;
            autoUpdater.OnUpdateAvailable += OnUpdateIsAvailable;
            await autoUpdater.CheckForUpdatesAsync();
            return versionNumber;
        }

        private void OnUpdateIsAvailable(UpdateInfo obj)
        {
            if (trigger == 0)
            {
                var dict = new Dictionary<string, string>{
                { "version", obj.Version.ToString() },
                {"releaseName", obj.ReleaseName }
                 };

                Electron.IpcMain.Send(MainWindow, "update-updater-data", JsonConvert.SerializeObject(dict, Formatting.Indented));

                StartDownload();
                trigger = 0;
            }
        }
        private async void StartDownload()
        {
            try
            {
                var autoUpdater = Electron.AutoUpdater;
                autoUpdater.OnDownloadProgress += OnDownload;
                autoUpdater.OnUpdateDownloaded += OnDownloadFinished;
                autoUpdater.AutoDownload = false;
                await autoUpdater.DownloadUpdateAsync();
            } catch(Exception ex)
            {
                Log(ex.Message, LogTypes.Error);
            }
        }

        private void OnDownloadFinished(UpdateInfo obj)
        {

            Electron.AutoUpdater.QuitAndInstall(false, true);

        }

        private void OnDownload(ProgressInfo obj)
        {
            var dict = new Dictionary<string, string>
            {
                {"downloadProgress", obj.Percent }
            };
            Electron.IpcMain.Send(MainWindow, "update-download", JsonConvert.SerializeObject(dict, Formatting.Indented));
        }
    }
}
