using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GloryBot.Controllers
{
    public class SubscriberController : Controller
    {
        private readonly ILogger<SubscriberController> _logger;

        public SubscriberController(ILogger<SubscriberController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // var subs = GetSubs().Result;
            // ViewData["subscriptions"] = subs;
            IsHomeActive = false;

            return View();
        }

        // private async Task<List<Subscription>> GetSubs() {
        //     var subs = await TrovoCore.GetSubscriber();
        //     return subs;
        // }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}