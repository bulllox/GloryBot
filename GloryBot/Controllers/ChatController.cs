using System.Timers;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using Newtonsoft.Json.Linq;
using GloryBot.Hubs;
using GloryBot.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace GloryBot.Controllers;

public class ChatController : Controller
{
    private readonly ILogger<WebAlertController> _logger;
    private readonly IHubContext<ChatHub, IChatHub> _hub;
   
    public ChatController(ILogger<WebAlertController> logger, IHubContext<ChatHub, IChatHub> hubs)
    {
        _hub = hubs;
        _logger = logger;
    }

    public IActionResult Index()
    {
        Chat.ClearEvent();
        
        Chat.OnMessage += ChatMessage;


        IsHomeActive = false;
        SetChatState(true);

        return View();
    }

    private async void ChatMessage(string message)
    {
        var msg = JsonConvert.DeserializeObject<ChatModel>(message);
        ChatModel.Chat chat = msg.data.chats[0];

        var role = UserRoles.All;
        if (!string.IsNullOrEmpty(chat.custom_role))
        {
            var customRoles = JsonConvert.DeserializeObject<List<CustomRole>>(chat.custom_role);
            role = (UserRoles)customRoles[0].roleType;
        }
        var cmdName = ChatInstance.SplitCommand(chat.content)["command"];
        var args = (string[])ChatInstance.SplitCommand(chat.content)["args"];

        var client = new Client
        {
            UserName = chat.nick_name,
            Role = role,
            Tier = chat.sub_tier,
            Lvl = chat.sub_lv
        };
        Log(JsonConvert.SerializeObject(chat, Formatting.Indented), LogTypes.Chat);
        var dict = new Dictionary<string, dynamic>();
        try
        {
            switch ((ChatType)chat.type.ToInt())
            {
                case ChatType.Normal:
                    // Give User a color
                    var rand = new Random();
                    var color = (chat.sub_tier != "" && chat.sub_tier.ToInt() > 0) ? Chat.SubColor : Chat.Colors[rand.Next(0, Chat.Colors.Count - 1)];
                    Chat.UserColor.AddIfNotExists(chat.nick_name, color);

                    // Handle Message
                    var chatMessage = ChatInstance.ChatHandle.ReplaceEmojis(chat.nick_name, chat.content);
                    // Handle Avatar adds full link if not exists
                    var avatar = ChatInstance.ChatHandle.BuildAvatar(chat.avatar);
                    if (chat.content.StartsWith("!"))
                    {
                        
                        //SendToChatBox("newmsg", chat.content);
                        await _hub.Clients.All.ReceiveMessage(avatar, Chat.UserColor[chat.nick_name], chat.nick_name, chatMessage.Result);
                        ChatInstance.CommandInstance.HandleCommand(client, cmdName, args);
                    }
                    else
                    {
                        
                        await _hub.Clients.All.ReceiveMessage(avatar, Chat.UserColor[chat.nick_name], chat.nick_name, chatMessage.Result);
                        //SendToChatBox("newmsg", ChatInstance.ChatHandle.HandleNormalMessage(chat));
                    }
                    break;
                case ChatType.Spell:
                    
                    var price = chat.content_data.gift_price_info.number * chat.content_data.gift_num;
                    ChatInstance.ChatHandle.DoSpellAlert(client, chat.content_data.gift_display_name, chat.content_data.gift_num, price);
                    Electron.IpcMain.Send(MainWindow, "show-chat-alert", JsonConvert.SerializeObject(dict, Formatting.Indented));
                    break;
                case ChatType.Follow:
                    ChatInstance.ChatHandle.DoFollowAlert(chat.nick_name);
                    break;
                case ChatType.Sub:
                    // DoSubAlert(chat.nick_name);
                    ChatInstance.ChatHandle.DoSubAlert(client);
                    break;
                case ChatType.StreamOnOff:
                    if (chat.content == "stream_on" && DiscordInstance.DiscordState == false && !string.IsNullOrEmpty(DashboardInstance.SettingsModel.DiscordClientId))
                    {
                        DiscordInstance.StartDiscord(DashboardInstance.SettingsModel.DiscordClientId);
                        DiscordInstance.SetInfo(DashboardInstance.SettingsModel.DiscordLargeImage, DashboardInstance.SettingsModel.DiscordSmallImage, streamInfo.channel_url);
                        DiscordInstance.StartUpdateTimer();
                    }

                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

}