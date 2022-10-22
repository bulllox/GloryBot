using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GloryBot.Controllers
{
    public class FollowerController : Controller
    {
        private readonly ILogger<FollowerController> _logger;

        public FollowerController(ILogger<FollowerController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            IsHomeActive = false;
            ViewData["follower"] = await GetFollowerAsync();
            return View();
        }
        
         private async Task<FollowerModel> GetFollowerAsync() {
            var channelId = await Trovo.GetStreamerChannelID();
            var follower = await TrovoHandle.GetFollower(channelId, streamInfo.followers);
            return follower;
         }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}