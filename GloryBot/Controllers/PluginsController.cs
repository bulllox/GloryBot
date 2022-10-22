using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace GloryBot.Controllers
{
    public class StateModel
    {
        public string state { get; set; } = "";
        public string command {get; set;} = "";
        public string plugin { get; set; } = "";
    }
    public class PluginsController : Controller
    {
        private readonly ILogger<PluginsController> _logger;

        public PluginsController(ILogger<PluginsController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            Electron.IpcMain.On("plugin:state", ChangePluginState);
            IsHomeActive = false;
            return View();
        }

        private void ChangePluginState(object obj)
        {

            var state = JsonConvert.DeserializeObject<StateModel>(JsonConvert.SerializeObject(obj));
            var plugin = ChatInstance.PluginList[state.command];            
        
            if (state.state == "on")
            {
                if (Directory.Exists(Asset("Plugins/" + state.plugin + "disabled")))
                {
                    Directory.Move(Asset($"Plugins/{state.plugin}disabled"), Asset($"Plugins/{state.plugin}"));
                    plugin.IsActive = true;
                }
            }
            else if (state.state == "off")
            {
                if (Directory.Exists(Asset("Plugins/" + state.plugin)))
                {
                    Directory.Move(Asset($"Plugins/{state.plugin}"), Asset($"Plugins/{state.plugin}disabled"));
                    plugin.IsActive = false;
                }
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}