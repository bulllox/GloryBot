using GloryBot.Interface;
using Microsoft.AspNetCore.SignalR;
namespace GloryBot.Hubs
{
    public class ChatHub : Hub<IChatHub>
    {
         
        public async Task Send(string user, string message)
        {
            var token = (user == "Bot") ? Globals.AccessData.BotAccess.access_token : Globals.AccessData.StreamerAccess.access_token;
            var channelid = await Trovo.GetStreamerChannelID();
            if(message.StartsWith("/"))
            {
                switch(message)
                {
                    case "/clear":
                        Clients.All.ClearChat();
                        break;
                }
                TrovoHandle.PerformChatCommand(token, channelid, message.Replace("/", ""));
            } 
            else
            {
                TrovoHandle.SendChatMessageToChannel(token, channelid, message);
            }
        }

        public override Task OnConnectedAsync()
        {
            ChatInstance.ChatConnections.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task GetOldMessage()
        {
            if (ChatInstance.ChatMessages.Count > 0)
            {
                foreach (var chatMessage in ChatInstance.ChatMessages)
                {
                    try
                    {
                        var user = chatMessage["user"];
                        var avatar = chatMessage["avatar"];
                        var color = chatMessage["color"];
                        var message = chatMessage["message"];
                        Clients.All.ReceiveMessage(avatar, color, user, message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
