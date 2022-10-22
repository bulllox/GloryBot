using GloryBot.Models.SaveModels;
using Quobject.SocketIoClientDotNet.Client;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
namespace GloryBot.Controllers;
public class SettingsController : Controller
{

    // TODO Add Auth System

    public IActionResult Index()
    {
        TDGlobals.SetChatState(false);
        IsHomeActive = false;

        Electron.IpcMain.RemoveAllListeners("settings-save-obs-setting");
        Electron.IpcMain.RemoveAllListeners("settings-save-discord");
        /* Electron.IpcMain.On("settings-save-discord", (args) => {
             var jString = JsonConvert.SerializeObject(args, Formatting.Indented);
             var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jString);
             var settings = DashboardInstance.SettingsModel;
             settings.DiscordClientId = data["clientId"];
             settings.DiscordLargeImage = data["largeImage"];
             settings.DiscordSmallImage = data["smallImage"];
             settings.Save();
             var dict = new Dictionary<string, string>{
             {"title", "Save Success"},
             {"msg", "Discord data saved"},
             {"uri", "/settings/index"},
             {"time", "3"}
             };
             Electron.IpcMain.Send(MainWindow, "window-wd", JsonConvert.SerializeObject(dict, Formatting.Indented));
         });*/

        //Electron.IpcMain.On("botAuth", DoBotAuth);
        // Electron.IpcMain.On("StreamerAuth", DoStreamerAuth);
        Electron.IpcMain.On("settings-save-obs-setting", SaveObsSettings);
        ViewData["Options"] = LangInstance.GetLanguage();
        return View();
    }

    [HttpPost]
    public IActionResult SaveChannel(TempSettingsModel model)
    {
        DashboardInstance.SettingsModel.Channel = model.ChannelName;
        DashboardInstance.SettingsModel.Save();
        ChatInstance.ChatDataSet = true;

        return Redirect("/settings/index");
    }

    [HttpPost]
    public IActionResult SaveDiscord(TempSettingsModel model)
    {
        DashboardInstance.SettingsModel.DiscordClientId = model.DiscordClientID;
        DashboardInstance.SettingsModel.DiscordLargeImage = model.DiscordLargeImage;
        DashboardInstance.SettingsModel.DiscordSmallImage = model.DiscordSmallImage;
        DashboardInstance.SettingsModel.Save();

        return Redirect("/settings/index");

    }

    public IActionResult ChangeLanguage(string lang)
    {
        ChangeTranslation(lang);
        return Redirect("/settings/index");
    }

    private void SaveObsSettings(object obj)
    {
        Console.WriteLine("hallo Obs");
        var data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(obj.ToString());
        Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
        DashboardInstance.SettingsModel.ObsUrl = data["obsurl"];
        DashboardInstance.SettingsModel.ObsPassword = data["obspassword"];
        DashboardInstance.SettingsModel.ObsActive = (data["active"] == "on") ? 1 : 0;
        if (!ObsInstance.ObsConnected && data["active"] == "on")
        {
            ObsInstance.ConnectObs();
        }
        DashboardInstance.SettingsModel.Save();
    }

    [HttpPost]
    public IActionResult StreamerAuth()
    {
        if (!Trovo.HasAccessData("Streamer"))
        {
            Trovo.db = DbPath;
            var scopes = $"{TrovoCore.Enums.Scopes.ChannelDetailsSelf}+{TrovoCore.Enums.Scopes.ChannelSubscriptions}+{TrovoCore.Enums.Scopes.ChannelUpdateSelf}+{TrovoCore.Enums.Scopes.ChatSendSelf}+{TrovoCore.Enums.Scopes.ManageMessages}+{TrovoCore.Enums.Scopes.SendToMyChannel}+{TrovoCore.Enums.Scopes.UserDetailsSelf}";
            var url = string.Format(LinkCollection.LoginPage, Globals.ClientID, scopes, "http://127.0.0.1:9090/", TDGlobals.RandomString(5));
            Trovo.Authorize("Streamer", url, new Action<string>(OnStreamerFinish));

        }
        return Redirect("/settings/index");
    }


    private void OnStreamerFinish(string result)
    {
        if (result.ToBoolean() == true)
        {
            StreamerAuthorized = true;
        }
        else
        {
            StreamerAuthorized = false;
        }
        //MainWindow.Reload();
    }
    [HttpPost]
    public IActionResult BotAuth()
    {
        if (!Trovo.HasAccessData("Bot"))
        {
            Trovo.db = DbPath;
            var scopes = $"{TrovoCore.Enums.Scopes.ChannelDetailsSelf}+{TrovoCore.Enums.Scopes.ChannelSubscriptions}+{TrovoCore.Enums.Scopes.ChannelUpdateSelf}+{TrovoCore.Enums.Scopes.ChatSendSelf}+{TrovoCore.Enums.Scopes.ManageMessages}+{TrovoCore.Enums.Scopes.SendToMyChannel}+{TrovoCore.Enums.Scopes.UserDetailsSelf}";
            var url = string.Format(LinkCollection.LoginPage, Globals.ClientID, scopes, "http://127.0.0.1:9090/", TDGlobals.RandomString(5));
            Trovo.Authorize("Bot", url, new Action<string>(OnBotFinish));
        }
        return Redirect("/settings/index");
    }

    private void OnBotFinish(string result)
    {
        if (result.ToBoolean() == true)
        {
            BotAuthorized = true;
            OpenChat();
        }
        else
        {
            BotAuthorized = false;
        }
    }

    private void OpenChat()
    {
        if (!ChatInstance.ChatOpen)
        {
            ChatInstance.ChatHandle.Connect();
        }
    }
    /*
    public void ChangeLanguage(string Lang)
    {

        Translation.ChangeTranslation(Lang);
        if (ChangeLangCall == 1)
        {
            ChangeLangCall = 0;
        }
    }*/

}