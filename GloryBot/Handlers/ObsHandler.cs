using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ObsStrawket;
using ObsStrawket.DataTypes.Predefineds;

namespace GloryBot.Handlers
{
    public class ObsHandler
    {
        private ObsClientSocket _obsClient { get; init; } = new();
        public bool ObsConnected { get; private set; } = false;


        public ObsHandler()
        {
            _obsClient = new();
            _obsClient.Connected += OnConnected;
        }

        private void OnConnected(Uri obj)
        {
            Console.WriteLine("Obs Connected");
            Log("Obs connected\r\n", LogTypes.StreamTool);

            // ShowAlert("test", "test");


        }

        public async void ConnectObs()
        {
            try
            {
                var url = new Uri($"ws://{DashboardInstance.SettingsModel.ObsUrl}");
                var password = DashboardInstance.SettingsModel.ObsPassword;
                await _obsClient.ConnectAsync(url, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't Connect to Obs: " + ex.Message);
            }
        }

        /*
            Shows Alerts for .... duration in seconds
        */
        public async void ShowAlert(string scene, string sceneItem, int duration = 3)
        {
            try
            {
                //Gets items in group


                var sceneRes = await _obsClient.GetSceneItemIdAsync(scene, sceneItem);
                var sceneItemId = sceneRes.SceneItemId;


                await _obsClient.SetSceneItemEnabledAsync(scene, sceneItemId, true);
                await Task.Delay(900).ContinueWith(async _ =>
                           {
                EnableGroupItems(sceneItem);
                            });
                await Task.Delay(duration * 1000).ContinueWith(async _ =>
                           {
                               await _obsClient.SetSceneItemEnabledAsync(scene, sceneItemId, false);
                               DisableGroupItems(sceneItem);
                           });
                // SetTimeout(async () =>
                // {
                //     await _obsClient.SetSceneItemEnabledAsync(sceneItem, sceneItemId, false);
                //     // DisableGroupItems(sceneItem);
                // }, duration * 1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async void EnableGroupItems(string sceneItem)
        {
            try
            {
                var res = await _obsClient.GetGroupSceneItemListAsync(sceneItem);
                var groupItems = res.SceneItems;
                foreach (var item in groupItems)
                {
                    var groupItemId = item["sceneItemId"].ToString().ToInt();
                    await _obsClient.SetSceneItemEnabledAsync(sceneItem, groupItemId, true);
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void DisableGroupItems(string sceneItem)
        {
            try
            {
                var res = await _obsClient.GetGroupSceneItemListAsync(sceneItem);
                var groupItems = res.SceneItems;
                foreach (var item in groupItems)
                {
                    var groupItemId = item["sceneItemId"].ToString().ToInt();
                    await _obsClient.SetSceneItemEnabledAsync(sceneItem, groupItemId, false);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}