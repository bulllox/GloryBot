
using GloryBot.Handlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using TrovoCore;

namespace GloryBot
{
    public class Program
    {
        private static string[] pluginPaths = new string[] {

        };

        

        public static async Task Main(string[] args)
        {
            
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                RootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\GloryBot\"; 
            } else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                RootPath = "/usr/share/GloryBot/";
            }
            DbPath = ConvertSlash($"{RootPath}/Assets/Database/GloryBot.db");
            if(Directory.Exists(Asset("logs")))
            {
                foreach(var file in Directory.GetFiles(Asset("logs")))
                {
                    File.Delete(file);
                }
                Directory.Delete(Asset("logs"));
            }
            try
            {
                
                CheckForFolders();
                DashboardInstance.SettingsModel = new();

               

                ChatInstance = new();
                ChatInstance.ChatHandle = new();
                // TDGlobals.LastCategorys = new();
                // TDGlobals.LastCategorys = LoadLastCats();
                DashboardInstance.SettingsModel = LoadSettings();

                // Load Language Database
                LangInstance = new();
                LangInstance.LoadLangDb();
                LoadTranslation(DashboardInstance.SettingsModel.Lang);

                Globals.AccessData.BotAccess = LoadAuthData("Bot");
                Globals.AccessData.StreamerAccess = LoadAuthData("Streamer");

                if (!string.IsNullOrEmpty(DashboardInstance.SettingsModel.Channel))
                {
                    TDGlobals.ChatInstance.ChatDataSet = true;
                }
                await CheckAuthData("Bot");
                await CheckAuthData("Streamer");

                // Load CateGorys
                DashboardInstance.GameCategorys = new();
                var cats = await Trovo.LoadCategorys();
                DashboardInstance.GameCategorys = cats;

                // // TDGlobals.GetCategorys();
                // // TDGlobals.GetChannelUserInfo();
                var emotes = await Trovo.LoadEmotes();
                ChatInstance.Emotes = emotes;
                ChatInstance.CommandInstance = new();
                ChatInstance.CommandInstance.Init();
                                
                LoadCommands();

                ChatInstance.LoadNodifications();
                // // PluginInstance = new();
                // // PluginInstance.LoadPlugins();

  
                LoadAlerts();
                DiscordInstance = new();
                
              

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            CreateHostBuilder(args).Build().Run();
        }

        
        private static int StreamTime = 0;
        private static void onDiscordTick(object sender, ElapsedEventArgs e)
        {
            StreamTime++;
            
        }

        private static void LoadAlerts()
        {
             using (var db = new LiteDatabase(DbPath))
            {
                var col = db.GetCollection<AlertModel>("alerts");

                var res = col.FindAll().ToList();
                ChatInstance.Alerts = res;
            }
        }

        private static List<LastCatData> LoadLastCats()
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var col = db.GetCollection<LastCatData>("lastCats");

                var res = col.FindAll().ToList();
                return res;
            }
        }
        private static SettingsModel LoadSettings()
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var col = db.GetCollection<SettingsModel>("settings");
                if (col.Count() <= 0)
                {
                    var model = new SettingsModel
                    {
                        Lang = "German"
                    };
                    col.Insert(model);
                    return model;
                }
                else
                {
                    return col.FindById(1);
                }
            }
        }

        private static AccessToken LoadAuthData(string user)
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var col = db.GetCollection<AccessToken>("Auth");

                if (!col.Exists(Query.EQ("username", user)))
                {
                    var model = new AccessToken
                    {
                        username = user
                    };
                    col.Insert(model);
                    return model;
                }
                else
                {
                    return col.FindOne(Query.EQ("username", user));
                }
            }
        }


        private static void CheckForFolders()
        {
            Directory.CreateDirectory(ConvertSlash(RootPath));
            Directory.CreateDirectory(ConvertSlash(RootPath + @"\Assets"));
            Directory.CreateDirectory(ConvertSlash(Asset("Alerts")));
            Directory.CreateDirectory(ConvertSlash(Asset("Configs")));
            Directory.CreateDirectory(ConvertSlash(Asset("logs")));
            Directory.CreateDirectory(ConvertSlash(Asset("Plugins")));
            Directory.CreateDirectory(ConvertSlash(Asset("Database")));
            CreateFiles();
        }

        private static void CreateFiles()
        {
            var usercolors = File.ReadAllText("ToMove/Configs/usercolors.json");
            File.WriteAllText(ConvertSlash(Asset("Configs/usercolors.json")), usercolors);

        }

        private static void LoadCommands()
         {
             using (var db = new LiteDatabase(DbPath))
             {
                 var col = db.GetCollection<CommandModel>("commands");
                 var commands = col.FindAll().ToList();
                 foreach (var command in commands)
                 {
                     Console.WriteLine($"Loaded Command: {command.Command}");
                     ChatInstance.CommandInstance.RegisterCommand(command.Command, command);
                 }
             }
         }
        private static void SetAccessData(string user, AccessToken token)
        {
            var currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            var time = currentTime + token.expires_in.ToInt();
            switch (user)
            {
                case "Bot":
                    Globals.AccessData.BotAccess.username = "Bot";
                    Globals.AccessData.BotAccess.access_token = token.access_token;
                    Globals.AccessData.BotAccess.expires_in = time.ToString();
                    Globals.AccessData.BotAccess.token_type = token.token_type;
                    Globals.AccessData.BotAccess.refresh_token = token.refresh_token;

                    Globals.AccessData.Save<AccessToken>(Globals.AccessData.BotAccess, DbPath, "Auth");
                    Console.WriteLine("Token refreshed");
                    Log($"Token refreshed", LogTypes.Bot);
                    TDGlobals.BotAuthorized = true;
                    break;
                case "Streamer":
                    Globals.AccessData.StreamerAccess.username = "Streamer";
                    Globals.AccessData.StreamerAccess.access_token = token.access_token;
                    Globals.AccessData.StreamerAccess.expires_in = time.ToString();
                    Globals.AccessData.StreamerAccess.token_type = token.token_type;
                    Globals.AccessData.StreamerAccess.refresh_token = token.refresh_token;

                    Globals.AccessData.Save<AccessToken>(Globals.AccessData.StreamerAccess, DbPath, "Auth");
                    Console.WriteLine("Token refreshed");
                    Log($"Token refreshed", LogTypes.Bot);
                    TDGlobals.StreamerAuthorized = true;
                    break;
            }
        }
        private async static Task CheckAuthData(string user)
        {
            Log($"Checking Access Token from {user}", LogTypes.Bot);
            if (Trovo.HasAccessData(user) && !Trovo.IsNotExpired(user))
            {
                Log($"Access Token expired needs refresh", LogTypes.Bot);
                var refresh_token = (user == "Bot") ? Globals.AccessData.BotAccess.refresh_token : Globals.AccessData.StreamerAccess.refresh_token;
                try
                {
                    var res = TrovoHandle.RefreshToken(refresh_token);
                    if (res.IsSuccessStatusCode)
                    {
                        var output = JsonConvert.DeserializeObject<AccessToken>(await res.Content.ReadAsStringAsync());
                        SetAccessData(user, output);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (Trovo.HasAccessData(user) && Trovo.IsNotExpired(user))
            {
                if (user == "Bot")
                {
                    TDGlobals.BotAuthorized = true;
                }
                else if (user == "Streamer")
                {
                    TDGlobals.StreamerAuthorized = true;
                }
            }
        }

      
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)              
                .ConfigureWebHostDefaults(webBuilder =>
                {                   
                    webBuilder.UseElectron(args);
                    webBuilder.UseEnvironment("Development");
                    webBuilder.UseStartup<Startup>();
                });

        
    }
}
