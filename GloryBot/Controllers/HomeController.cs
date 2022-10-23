using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using GloryBot.Models;
using Chat = TrovoCore.Chat;
using Microsoft.AspNetCore.SignalR;
using GloryBot.Hubs;
using GloryBot.Interface;
using ElectronNET.API;
using Microsoft.AspNetCore.Mvc;
using GloryBot.Models.SaveModels;

namespace GloryBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<HomeHub, IHomeHub> _hub;

        public HomeController(ILogger<HomeController> logger, IHubContext<HomeHub, IHomeHub> hub)
        {
            _logger = logger;
            this._hub = hub;
        }
        private int trigger = 0;
        public IActionResult Index()
        {

            if (!IsServer)
            {
                SetChatState(false);
                IsHomeActive = true;
                streamInfo = new();
                if (Trovo.HasAccessData("Bot") && !Chat.Connected && !ChatInstance.ChatOpen)
                {
                    Thread.Sleep(2000);
                    ChatInstance.ChatHandle.Connect();
                }
                RemoveListeners();

                Electron.IpcMain.On("GoToStream", (object url) =>
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = url.ToString(),
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new Exception();
                    }
                });
                if (BotAuthorized && StreamerAuthorized && ChatInstance.ChatDataSet)
                {
                    var sInfo = GetStreamInfo().Result;
                    if (sInfo != null)
                    {
                        streamInfo = sInfo;
                        Log(JsonConvert.SerializeObject(sInfo, Formatting.Indented), LogTypes.StreamInfo);

                        ViewData["streamInfo"] = sInfo;
                        ViewData["categorys"] = DashboardInstance.GameCategorys;
                        DateTime dateNow = DateTime.Now;
                       
                        if (sInfo.is_live && DiscordInstance.DiscordState == false && !string.IsNullOrEmpty(DashboardInstance.SettingsModel.DiscordClientId))
                        {
                            DiscordInstance.StartDiscord(DashboardInstance.SettingsModel.DiscordClientId);
                            DiscordInstance.SetInfo(DashboardInstance.SettingsModel.DiscordLargeImage, DashboardInstance.SettingsModel.DiscordSmallImage, sInfo.channel_url);
                            DiscordInstance.StartUpdateTimer();
                        }
                       
                        var timer = new System.Timers.Timer();
                        timer.Interval = 5 * 60 * 1000;
                        timer.Elapsed += OnStreamUpdate;
                        timer.Start();
                    }
                }
                //Electron.IpcMain.On("SaveStreamInfo", SaveStreamInfo);
                Task.Delay(2000).ContinueWith(_ => { OnStart = false; });

                if (!isDev)
                {
                    Console.WriteLine("not dev");

                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        CheckForUpdates();
                    });



                }
                
                if (BetaTest == true)
                {
                    return Redirect("/betatest/index");
                }
                else
                {

                    return View();
                }
            }
            
            else
            {
                if (loggedin)
                {
                    return View();
                }
                else
                {
                    return View("Login");
                }
            }
        }


        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
        public async void CheckForUpdates()
        {
            if (HybridSupport.IsElectronActive)
            {
                var autoUpdater = Electron.AutoUpdater;
                
                Log("Checking for Updates", LogTypes.System);
                autoUpdater.OnUpdateAvailable += OnUpdateAvailable;
                await autoUpdater.CheckForUpdatesAsync();
            }

        }

        private void OnUpdateAvailable(UpdateInfo obj)
        {
            // Log("Update available", LogTypes.System);

            GotoUpdatePage();

        }

        private void GotoUpdatePage()
        {
            var dict = new Dictionary<string, string>{
                    {"title", "Update found"},
                    {"msg", "We found a new Update redirect to Update page"},
                    {"uri", "/update"},
                    {"time", "3"}
            };
            Electron.IpcMain.Send(MainWindow, "window-wd", JsonConvert.SerializeObject(dict, Formatting.Indented));
            Electron.AutoUpdater.OnUpdateAvailable -= OnUpdateAvailable;
        }

        private async void OnStreamUpdate(object sender, ElapsedEventArgs e)
        {
            if (BotAuthorized && StreamerAuthorized && ChatInstance.ChatDataSet)
            {
                var sInfo = GetStreamInfo().Result;
                if (sInfo != null)
                {
                    streamInfo = sInfo;
                    Log(JsonConvert.SerializeObject(sInfo, Formatting.Indented), LogTypes.StreamInfo);
                    var info = JsonConvert.SerializeObject(streamInfo, Formatting.Indented);
                    if (DiscordInstance.DiscordState == true)
                    {
                        DiscordInstance.SetInfo(DashboardInstance.SettingsModel.DiscordLargeImage, DashboardInstance.SettingsModel.DiscordSmallImage, sInfo.channel_url);
                    }
                    await _hub.Clients.All.UpdateStreamInfo(info);
                }
                else
                {

                }
            }
        }

        public async Task<IActionResult> SaveSettings(HomeModel model)
        {

            var channelid = await Trovo.GetStreamerChannelID();
            var dict = new Dictionary<string, dynamic>{
                { "channel_id", channelid },
                {   "live_title", model.StreamTitle},
                {"category_id", model.Category},
                {"audi_type", model.AgeLimit}
            };
            Console.WriteLine(JsonConvert.SerializeObject(dict, Formatting.Indented));
            var res = TrovoHandle.EditChannelInfo(dict);
            if (res.IsSuccessStatusCode)
            {
                Console.WriteLine("Saved");
                if (DiscordInstance.DiscordState == true)
                {
                    DiscordInstance.SetInfo(DashboardInstance.SettingsModel.DiscordLargeImage, DashboardInstance.SettingsModel.DiscordSmallImage, streamInfo.channel_url);
                }
            }
            else
            {
                var cont = await res.Content.ReadAsStringAsync();
                Console.WriteLine(cont);
            }
            
            return Redirect("/home/index");
        }

        private async Task<StreamModel> GetStreamInfo()
        {
            var res = TrovoHandle.GetChannelInfoByChannelName(DashboardInstance.SettingsModel.Channel);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<StreamModel>(content);
            }
            else
            {
                Console.WriteLine(await res.Content.ReadAsStringAsync());
                return null;
            }
        }

        private void RemoveListeners()
        {
            Electron.IpcMain.RemoveAllListeners("GoToStream");
            Electron.IpcMain.RemoveAllListeners("SaveStreamInfo");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public void MinimizeWindow()
        {
            if(HybridSupport.IsElectronActive)
            {
                MainWindow.Minimize();
            }
        }
    }


}
