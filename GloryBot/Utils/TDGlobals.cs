using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Web;
using GloryBot.Handlers;

namespace GloryBot.Utils;
public static class TDGlobals
{
    public static bool IsServer {get; set;} = false;
    public static bool BetaTest { get; private set; } = false;
    public static bool loggedin {get; set;} = false;
    #region TrovoVars 
    #endregion
    #region GlobalVars
    public static bool OnStart { get; set; } = true;
    public static Lang LangInstance { get; set; } = new();

    public static BrowserWindow MainWindow { get; set; }

    #region Bot
    public static StreamModel streamInfo { get; set; } = new();
    public static bool BotAuthorized { get; set; } = false;
    public static bool StreamerAuthorized { get; set; } = false;
    #endregion
    // public static PluginHandler PluginInstance { get; set; } = new();

    //public static GloryBot.Models.GameCategoryModel GameCategorys { get; set; } = new();

    public static long CurrentTimeInSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

    public static bool IsHomeActive { get; set; } = false; 

    #region  Dashboard

    // public static DbModel Database {get; set;} = new();
    public static DashBoard DashboardInstance { get; set; } = new();
    public static List<LastCatData> LastCategorys { get; set; } = new();
    #endregion

    #region  Chat 
    public static ChatUtils ChatInstance { get; set; } = new();

    public static ObsHandler ObsInstance { get; private set; } = new();
   

    public static string DbPath { get; set; } = "";
    // public static List<IPlugin> Plugins { get; set; } = new();
    #endregion

    #endregion

    public static DiscordHandle DiscordInstance { get; set; } = new();
    public static string RootPath { get; set; } = ""; 

    public static bool isDev { get; set; } = false;


    public static string ConvertSlash(string path) => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) ? path.Replace("/", "\\") : path.Replace("\\", "/");
    
    // Loads Lang Database
    
    // Sets Root Path
    public static string PublicFolder()
    {
        var path = System.Environment.CurrentDirectory;
        string seachPattern = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) ? "obj/Host/bin" : @"obj\Host\bin";
        string removeString = "";
        string simbol = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) ? "/" : "\\";

        if (path.Contains(seachPattern))
            removeString = seachPattern;

        var tmpPath = string.Empty;
        if (!string.IsNullOrEmpty(removeString))
        {
            tmpPath = path.Remove(path.Length - removeString.Length);
            if (!tmpPath.EndsWith(simbol))
            {
                tmpPath += simbol;
            }
        }
        else
        {
            tmpPath = (!path.EndsWith(simbol)) ? path + simbol : path;
        }
        return tmpPath;
    }

    // Try´s to laod Json File or Create a new one
    public static T TryLoadJson<T>(string path)
    {
        if (File.Exists(path))
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
        else
        {
            var instance = Activator.CreateInstance(typeof(T));
            var jString = JsonConvert.SerializeObject(instance, Formatting.Indented);
            File.WriteAllText(path, jString);
            return JsonConvert.DeserializeObject<T>(jString);
        }
    }
    private static readonly Random _random = new Random();
    public static string RandomString(int size, bool lowerCase = false)
    {
        var builder = new StringBuilder(size);

        // Unicode/ASCII Letters are divided into two blocks
        // (Letters 65–90 / 97–122):
        // The first group containing the uppercase letters and
        // the second group containing the lowercase.  

        // char is a single Unicode character  
        char offset = lowerCase ? 'a' : 'A';
        const int lettersOffset = 26; // A...Z or a..z: length=26  

        for (var i = 0; i < size; i++)
        {
            var @char = (char)_random.Next(offset, offset + lettersOffset);
            builder.Append(@char);
        }

        return lowerCase ? builder.ToString().ToLower() : builder.ToString();
    }

    // Log everything in the right file
    public static void Log(string message, LogTypes type = LogTypes.System)
    {
        var path = ConvertSlash(Asset("logs"));
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        var tmpMessage = $"[{DateTime.Now}]: {message}\r\n";
        switch (type)
        {
            case LogTypes.Error:
                File.AppendAllText(ConvertSlash(path + "/error.log"), tmpMessage);
                break;

            case LogTypes.System:
                File.AppendAllText(ConvertSlash(path + "/system.log"), tmpMessage);
                break;
            case LogTypes.Chat:
                if (isDev)
                {
                    File.AppendAllText(ConvertSlash(path + "/Chat.log"), message);
                }
                break;
            case LogTypes.Emote:
                if (isDev)
                {
                    File.AppendAllText(ConvertSlash(path + "/Emote.log"), message);
                }
                break;
            case LogTypes.StreamInfo:
                if (isDev)
                {
                    File.AppendAllText(ConvertSlash(path + "/StreamInfo.log"), message);
                }
                break;
            case LogTypes.StreamTool:
                if (isDev)
                {
                    File.AppendAllText(ConvertSlash(path + "/StreamTool.log"), tmpMessage);
                }
                break;
            case LogTypes.Bot:
                File.AppendAllText(ConvertSlash(path + "/Bot.log"), tmpMessage);
                break;

        }

        return;
    }

    // Get Menu for Layout
    public static List<UrlModel> GetMenu() {
        var dict = new List<UrlModel>{
            new UrlModel{
                Url = "/home/index",
                Name = "DashBoard"
            },
            new UrlModel{
                Url = "/chat/index",
                Name = "Chat"
            },
            new UrlModel{
                Url = "/commands/index",
                Name = Translate("layout.commands", "Commands"),
                SubMenus = new()
                {
                    new UrlModel
                    {
                        Url = "/commands/index",
                        Name = "List"
                    },
                    new UrlModel
                    {
                        Url = "/commands/add",
                        Name = Translate("Add", "Add")
                    }
                }
            },
            new UrlModel{
                Url= "/nodifications/index",
                Name = Translate("layout.nodifications", "Nodifications"),
                SubMenus = new()
                {
                    new UrlModel
                    {
                        Url = "/nodifications/index",
                        Name = "List"

                    },
                    new UrlModel
                    {
                        Url = "/nodifications/add",
                        Name = Translate("Add", "Add")
                    }
                }
            },
            new UrlModel{
                Url = "/plugins/index",
                Name = "Plugins",
                Enabled = false
            },
            new UrlModel{
                Url = "/alerts/index",
                Name = "Alerts"
            },
            new UrlModel{
                Url = "/subscriber/index",
                Name = "Subscriber"
            },
            new UrlModel{
                Url = "/follower/index",
                Name = "Follower",
                Enabled = true
            }
        };
        return dict;
    }

    // Returns Path to Asset folder
    public static string Asset(string path)
    {
        return ConvertSlash($"{RootPath}/Assets/{path}");
    }

    // Javascript like settimeout
    public static void SetTimeout(Action value, int time)
    {
        Task.Delay(time).ContinueWith(_ => { value.Invoke(); });
    }


    public static void SetChatState(bool state)
    {
        TDGlobals.ChatInstance.ChatOpen = state;
    }
    

}