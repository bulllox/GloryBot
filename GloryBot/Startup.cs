using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GloryBot.Interface;
using GloryBot.Hubs;
using ElectronNET.API;

namespace GloryBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddSignalR();
            

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                isDev = true;
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapHub<AlertHub>("/alertHub");
                endpoints.MapHub<HomeHub>("/homeHub");
                endpoints.MapHub<ChatHub>("/chatHub");
                var routes = TDGlobals.TryLoadJson<List<RouteModel>>("./route.json");
                foreach (var route in routes)
                {
                    var wild = "";
                    Console.WriteLine($"Registering Route: {route.Name}");
                    Log($"[Startup]: register route: \"{route.Name}\"");
                    if (route.Wildcards != null && route.Wildcards.Count > 0)
                    {
                        foreach (var wildcard in route.Wildcards)
                        {
                            wild += "/{" + wildcard + "}";
                        }
                    }
                    var pattern = "{controller=" + route.Controller + "}/{action=" + route.Action + "}" + wild;
                    endpoints.MapControllerRoute(name: route.Name, pattern: pattern);
                }
            });

            if (HybridSupport.IsElectronActive)
            {
                CreateWindow();
            }
            else
            {
                Console.WriteLine("No Electron");
            }
        }

        public async void CreateWindow()
        {
            SetChatState(false);
            try
            {

                Log("[Electron.Net]: creating Electron Window", LogTypes.System);
                MainWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
                {
                    Width = 1540,
                    Height = 990,
                    Show = false,
                    Icon = ConvertSlash($"{PublicFolder()}/wwwroot/Icons/logo_small.png"),
                    Frame = false,
                    WebPreferences = new WebPreferences
                    {
                        NodeIntegration = false,
                        Preload = ConvertSlash($"{PublicFolder()}/wwwroot/js/preload.js"),
                        WebSecurity = false
                    }
                });
                await MainWindow.WebContents.Session.ClearCacheAsync();
                Electron.IpcMain.RemoveAllListeners("closeWindow");
                Electron.IpcMain.On("closeWindow", (args) =>
                {
                    try
                    {
                        Electron.App.Exit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Log("[Electron.Net Error}: " + ex.Message, LogTypes.Error);
                    }
                });
                Electron.GlobalShortcut.Register("Alt+K", () =>
                {
                    Console.WriteLine("Shortcut pressed ");
                    MainWindow.WebContents.OpenDevTools();
                });

                Electron.IpcMain.RemoveAllListeners("minimizeWindow");

                Electron.IpcMain.On("minimizeWindow", (args) => MainWindow.Minimize());
                Electron.App.WillQuit += (args) => Task.Run(() => Electron.GlobalShortcut.UnregisterAll());
                MainWindow.OnClose += () => Electron.App.Exit();
                MainWindow.OnReadyToShow += () =>
                {
                    MainWindow.Show();
                    MainWindow.Focus();
                    Log("[Electron.Net]: Window created");
                };
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log("[Electron.Net Error}: " + ex.Message, LogTypes.Error);
            }
        }

  
    }
}
