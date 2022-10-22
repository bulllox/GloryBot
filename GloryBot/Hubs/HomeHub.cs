using GloryBot.Interface;
using Microsoft.AspNetCore.SignalR;

namespace GloryBot.Hubs
{
    public class HomeHub : Hub<IHomeHub>
    {
        public async Task Update(string streaminfo)
        {
            await Clients.Caller.UpdateStreamInfo(streaminfo);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
