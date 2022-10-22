using Microsoft.AspNetCore.SignalR;
using GloryBot.Interface;

namespace GloryBot.Hubs;

public class AlertHub : Hub<IAlertHub>
{
    public async Task Send(string data)
    {
        await Clients.Caller.ShowAlert(data);
    }

}
