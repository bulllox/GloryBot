using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GloryBot.Controllers
{
    // TODO Alerts als Browser Quelle programmieren
    public class AlertsController : Controller
    {
        private readonly ILogger<AlertsController> _logger;

        public AlertsController(ILogger<AlertsController> logger)
        {
            _logger = logger;

        }

        public IActionResult Index()
        {
            IsHomeActive = false;
            if (HybridSupport.IsElectronActive)
            {
                       
                
            }
      
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> SaveAlert(AlertUploadModel model) {
            var alertName = model.Name;
            var volume = model.AlertVolume;
            var duration = model.AlertDuration;
            var animation = model.Animation;
            var text = model.Text;
            var textColor = model.TextColor;

            if (!ModelState.IsValid) { return View("Index"); }

            if(model.ImageFile != null) {
                using(var stream = new FileStream(ConvertSlash($"{PublicFolder()}/wwwroot/alerts/images/{model.ImageFile.FileName}"), FileMode.Create)) {
                    await model.ImageFile.CopyToAsync(stream);
                }
            }
            if(model.SoundFile != null) {
                
                using(var stream = new FileStream(ConvertSlash($"{PublicFolder()}/wwwroot/alerts/sounds/{model.SoundFile.FileName}"), FileMode.Create)) {
                    await model.SoundFile.CopyToAsync(stream);
                }
            }
            if(ChatInstance.Alerts.Exists(x => x.Name == alertName)) {
                var alert = ChatInstance.Alerts.Find(x => x.Name == alertName);
                alert.AlertText = text;
                alert.AlertDuration = duration;
                alert.AlertVolume = volume;
                alert.Animation = animation;
                alert.ImagePath = "/alerts/images/" + model.ImageFile.FileName;
                alert.SoundFilePath = "/alerts/sounds/" + model.SoundFile.FileName;
                alert.TextColor = textColor;
                alert.Save();
            } else {
                var alert = new AlertModel{
                    Name = alertName,
                    AlertText = text,
                    AlertVolume = volume,
                    AlertDuration = duration,
                    Animation = animation,
                    ImagePath = "/alerts/images/" + model.ImageFile.FileName,
                    SoundFilePath = "/alerts/sounds/" + model.SoundFile.FileName,
                    TextColor = textColor
                };
                alert.Save();
                ChatInstance.Alerts.AddIfNotExists(alert);
            }
            return View("Index");
        }

        public IActionResult Variables()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}