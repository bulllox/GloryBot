using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using GloryBot.Events;
using GloryBot.Hubs;
using GloryBot.Interface;
using System.Timers;
using System.ComponentModel;
using GloryBot.Enums;
using GloryBot.Extensions;

namespace GloryBot.Controllers;

public class WebAlertController : Controller
{
    private readonly ILogger<WebAlertController> _logger;
    private readonly IHubContext<AlertHub, IAlertHub> _hub;
    private bool isAlertRunning = false;

    private BackgroundWorker worker = new();

    private List<Dictionary<string, string>> Pending = new();

    public WebAlertController(ILogger<WebAlertController> logger, IHubContext<AlertHub, IAlertHub> hubs)
    {
        Pending = new();
        isAlertRunning = false;

        _hub = hubs;
        _logger = logger;
        ChatInstance.ChatHandle.OnAlert += AlertResponse;

        IsHomeActive = false;

        if (!worker.IsBusy)
            worker.RunWorkerAsync();
    }
    /*
     *  
     * Spell
     * Follow
     */
  
    // Set next Alert to be played
    private async void ProcessNext()
    {
        if (Pending.Count > 0)
        {
            var data = Pending.Shift<Dictionary<string, string>>();
            await PlayAlertAsync(data);
        } else
        {
            isAlertRunning = false;
        }
    }
    // Play alert
    private async Task PlayAlertAsync(Dictionary<string, string> data)
    {
        await this._hub.Clients.All.ShowAlert(JsonConvert.SerializeObject(data, Formatting.Indented));
        isAlertRunning = true;
        SetTimeout(() =>
        {
            // get next alert
            ProcessNext();
        }, data["duration"].ToInt() * 1001);
    }
    // Got Alert
    private async void AlertResponse(object sender, AlertEventArgs e)
    {
        var data = new Dictionary<string, string>
                {
                    { "text", e.text },
                    { "sourceImage", e.image },
                    { "sourceSound", e.SoundFile },
                    { "duration", e.Duration.ToString() },
                    { "Volume", e.Volume},
                    { "Animation", e.Animation },
                    { "TextColor", e.TextColor }
                };
        Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
        // When alert is playing add Alert to pending list
        if(isAlertRunning)
        {
            Pending.Add(data);
        } 
        else
        {
            // If no alert is running play alert
            await PlayAlertAsync(data);
        }

    }


    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}