using Newtonsoft.Json.Linq;
using TrovoCore;
using TrovoCore.Enums;
using GloryBot.Events;
using Chat = TrovoCore.Chat;

using System.Linq;
using System.Threading;
using System.Xml.Linq;
using DiscordRPC;
using static TrovoCore.Models.ChatModel;

namespace GloryBot.Handlers;




public class ChatHandle
{

    public delegate void AlertHandle(object sender, AlertEventArgs e);
    public event AlertHandle OnAlert;
   
    public ChatHandle()
    {
    }

    public void Connect()
    {

        if (!Chat.Connected)
        {
            Chat.LoadColors(ConvertSlash(Asset("Configs\\usercolors.json")));
            Chat.ClearEvent();
            Chat.OnMessage += OnChatMessage;
            // ChatUtils.ClearChat();
            Chat.ConnectToChat();
            if (DashboardInstance.SettingsModel.ObsActive == 1 && !ObsInstance.ObsConnected)
            {
                ObsInstance.ConnectObs();
            }

        }
    }
    private void OnChatMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<ChatModel>(message);
                var tmpMessage = JsonConvert.SerializeObject(msg, Formatting.Indented);
                Log(tmpMessage, LogTypes.Chat);

                if (Chat.Connected && ChatInstance.ChatOpen == false)
                {
                    HandleOfChat(msg.data.chats);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("On ChatMessage : " + ex);
            }
            ChatBackroundHandling();

            
        }

    }

    // Handles everything that happens in chat
    private void ChatBackroundHandling()
    {
        // Handle Chat nodifications
        foreach (var nody in ChatInstance.NodyList)
        {
            if (nody.Active)
            {
                if (nody.Lines == nody.CallAfterLines)
                {
                    nody.TriggerMessage();
                    nody.Lines = 0;
                }
                else
                {
                    nody.Lines++;
                }
            }
        }
    }

    // Handle of chat messages link when you are in the dashboard
    private void HandleOfChat(List<ChatModel.Chat> chats)
    {
       
        var dict = new Dictionary<string, dynamic>();
        foreach (var chat in chats)
        {
        
            var role = UserRoles.All;
            if (!string.IsNullOrEmpty(chat.custom_role))
            {
                var customRoles = JsonConvert.DeserializeObject<List<CustomRole>>(chat.custom_role);
                role = (UserRoles)customRoles[0].roleType;
            }
            var client = new Client
            {
                UserName = chat.nick_name,
                Role = role,
                Tier = chat.sub_tier,
                Lvl = chat.sub_lv
            };
            switch ((ChatType)chat.type.ToInt())
            {
                case ChatType.Normal:

                    if (!OnStart && chat.content.StartsWith("!"))
                    {
                        
                        var cmdName = ChatInstance.SplitCommand(chat.content)["command"];
                        var args = (string[])ChatInstance.SplitCommand(chat.content)["args"];
      
                        if (!OnStart)
                        {
                            ChatInstance.CommandInstance.HandleCommand(client, cmdName, args);
                        }
                    }
                    else
                    {
                        var rand = new Random();
                        var color = (chat.sub_tier != "" && chat.sub_tier.ToInt() > 0) ? Chat.SubColor : Chat.Colors[rand.Next(0, Chat.Colors.Count - 1)];
                        Chat.UserColor.AddIfNotExists(chat.nick_name, color);

                        // Handle Message
                        var chatMessage = ChatInstance.ChatHandle.ReplaceEmojis(chat.nick_name, chat.content);
                        // Handle Avatar adds full link if not exists
                        var avatar = ChatInstance.ChatHandle.BuildAvatar(chat.avatar);
                        var msg = new Dictionary<string, string>
                        {
                            { "user", chat.nick_name },
                            { "avatar", avatar },
                            { "color", Chat.UserColor[chat.nick_name] },
                            { "message", chatMessage.Result }
                        };
                        ChatInstance.ChatMessages.Add(msg);
                    }
                    break;
                case ChatType.Spell:
                    var price = chat.content_data.gift_price_info.number * chat.content_data.gift_num;
                    DoSpellAlert(client, chat.content_data.gift_display_name, chat.content_data.gift_num, price);
                    Electron.IpcMain.Send(MainWindow, "show-chat-alert", JsonConvert.SerializeObject(dict, Formatting.Indented));
                    break;
                case ChatType.Follow:
                    ChatInstance.ChatHandle.DoFollowAlert(chat.nick_name);
                    break;
                case ChatType.Sub:
                    DoSubAlert(client);
                    break;
                case ChatType.GiftSubOnUser:
                    break;
                case ChatType.StreamOnOff:
                    // on = stream_on off = stream_off
                    if (chat.content == "stream_on" && DiscordInstance.DiscordState == false && !string.IsNullOrEmpty(DashboardInstance.SettingsModel.DiscordClientId))
                    {
                        DiscordInstance.StartDiscord(DashboardInstance.SettingsModel.DiscordClientId);
                        DiscordInstance.SetInfo(DashboardInstance.SettingsModel.DiscordLargeImage, DashboardInstance.SettingsModel.DiscordSmallImage, streamInfo.channel_url);
                        DiscordInstance.StartUpdateTimer();
                    }

                    break;
            }


        }
    }

    public void DoFollowAlert(string user)
    {
        if (ChatInstance.Alerts.Exists(x => x.Name == "follow"))
        {
            File.WriteAllText(Asset("Alerts/LastFollow.txt"), user + "  ");
            var model = ChatInstance.Alerts.Find(x => x.Name == "follow");
            var text = model.AlertText.Replace("{user}", user);
            Console.WriteLine(JsonConvert.SerializeObject(model, Formatting.Indented));
            DoAlert(model.ImagePath, model.SoundFilePath, text, model.AlertDuration, model.AlertVolume, model.Animation, model.TextColor);
        }
    }
  
    public void DoAlert(string image, string soundFile, string text, string duration, string volume, string animation, string textColor)
    {
        var e = new AlertEventArgs
        {
            image = image,
            SoundFile = soundFile,
            text = text,
            Duration = duration,
            Volume = volume,
            Animation = animation,
            TextColor = textColor
        };
        try {
        OnAlert?.Invoke(this, e);
        } catch(Exception ex) {
            Console.WriteLine(ex);
        }
    }

 

    public void DoSpellAlert(Client client, string spell_name, int spell_count, int spell_price)
    {
        if (ChatInstance.Alerts.Exists(x => x.Name == "spell"))
        {
            var model = ChatInstance.Alerts.Find(x => x.Name == "spell");
            var text = model.AlertText.Replace("{user}", client.UserName);
            text = text.Replace("{spell_name}", spell_name);
            text = text.Replace("{spell_count}", spell_count.ToString());
            text = text.Replace("{spell_price}", spell_price.ToString());
            System.IO.File.WriteAllText(Asset("Alerts/LastSpell.txt"), client.UserName + "  ");
            DoAlert(model.ImagePath, model.SoundFilePath, text, model.AlertDuration, model.AlertVolume, model.Animation, model.TextColor);
        }
    }

    public void DoSubAlert(Client client)
    {
        if (ChatInstance.Alerts.Exists(x => x.Name == "sub"))
        {
            var model = ChatInstance.Alerts.Find(x => x.Name == "sub");
            var text = model.AlertText.Replace("{user}", client.UserName);
            text = text.Replace("{tier}", client.Tier.ToString());
            text = text.Replace("{lvl}", client.Lvl.ToString());
            System.IO.File.WriteAllText(Asset("Alerts/LastSub.txt"), client.UserName + "  ");
            DoAlert(model.ImagePath, model.SoundFilePath, text, model.AlertDuration, model.AlertVolume, model.Animation, model.TextColor);
        }
    }
    public string HandleNormalMessage(ChatModel.Chat chat)
    {
        try
        {
            var msg = "";
            var avatar = BuildAvatar(chat.avatar);
            var EmojiMessage = ReplaceEmojis(chat.nick_name, chat.content);
            Log(JsonConvert.SerializeObject(ChatInstance.Emotes, Formatting.Indented), LogTypes.Emote);
            var rand = new Random();
            var color = (chat.sub_tier != "" && chat.sub_tier.ToInt() > 0) ? Chat.SubColor : Chat.Colors[rand.Next(0, Chat.Colors.Count - 1)];
            Chat.UserColor.AddIfNotExists(chat.nick_name, color);

            msg = $"<img src='{avatar}' width='26' height='26' alt='no pic' style='margin-right:4px;'><span style='font-size:16px;color:{Chat.UserColor[chat.nick_name]};'>{chat.nick_name}</span>: <span style='font-size:16px;'>{EmojiMessage}</span>";
            return msg;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Handle Normal Message" + ex.Message);
            return "";
        }
    }
    public async Task<string> ReplaceEmojis(string user, string content)
    {
        var args = content.Split(' ');
        foreach (var item in args)
        {
            if (item.Contains(":"))
            {
                try
                {

                    var emoteName = item.Replace(":", "");
                    EmoteModel userEmote = null;

                    if (ChatInstance.Emotes.Exists(x => x.Name == emoteName))
                    {
                        userEmote = ChatInstance.Emotes.Where(uM => uM.Name == emoteName).First();
                    }
                    if(userEmote != null)
                    {
                        content = content.Replace(item, $"<img src='{userEmote.Url}' width='32' height='32' alt='no pic'>");
                    }
                    
                } catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

        }
        return content;
    }


    public string BuildAvatar(string avatar)
    {
        var res = "";
        if (!avatar.Contains("https://headicon.trovo.live/"))
        {
            if (avatar.Contains("?t=") || avatar.Contains("&t="))
            {
                res = $"https://headicon.trovo.live/user/{avatar}";
            }
            else
            {
                res = $"https://headicon.trovo.live/default/{avatar}";
            }
        }
        else
        {
            res = avatar;
        }
        return res;
    }


}