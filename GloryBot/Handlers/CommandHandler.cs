using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using GloryBot.Extensions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Chat = TrovoCore.Chat;

namespace GloryBot.Handlers;

public class CommandHandler
{

    public Dictionary<string, HashSet<dynamic>> cmdHandle { get; set; } = new();
    private Dictionary<string, List<UserRoles>> commandPermission { get; set; } = new();
    public List<PrefixModel> prefixes { get; private set; } = new();

    public CommandHandler()
    {
    }


    public void Init()
    {
        RegisterCommand("so", new Action<Client, string[]>(Shoutout), new List<UserRoles> { { UserRoles.Streamer }, { UserRoles.Mod } });
        RegisterCommand("uptime", new Action<Client, string[]>(Uptime), new List<UserRoles> { { UserRoles.All } });
        RegisterCommand("commands", new Action<Client, string[]>(GetCommands), new List<UserRoles> { { UserRoles.All } });
        RegisterCommand("alert", new Action<Client, string[]>(TestAlert), new List<UserRoles> { { UserRoles.Streamer } });
    }

    // args = !alert follow
    private void TestAlert(Client client, string[] args)
    {
        if (args.Length == 0)
        {
            Chat.SendChatMessage("Usage: !alert <alert type>");
            return;
        }
        var alertType = args[0];
        
        switch (alertType)
        {
            case "spell":

                ChatInstance.ChatHandle.DoSpellAlert(client, "Stay Safe", 4, 400);
                break;
            case "follow":
                ChatInstance.ChatHandle.DoFollowAlert("Bulllox");
                break;

            case "sub":
                ChatInstance.ChatHandle.DoSubAlert(client);
                break;
            case "gift":

                break;
        }
    }

    private void GetCommands(Client client, string[] obj)
    {
        var text = "commands: ";
        foreach (var key in cmdHandle.Keys)
        {
            var perm = commandPermission[key];
          
                text += $"!{key} | ";
            
        }
        text = text.Remove(text.Length - 1);
        Chat.SendChatMessage(text);
    }

    private void Uptime(Client client, string[] args)
    {
        var UptimeText = "";
        if (streamInfo.is_live)
        {
            var startTime = DateTimeOffset.FromUnixTimeSeconds(streamInfo.started_at.ToInt());

            var localTime = startTime.ToLocalTime();

            var time = DateTime.Now - localTime;

            var hours = (time.Hours < 10) ? $"0{time.Hours}" : time.Hours.ToString();
            var minutes = (time.Minutes < 10) ? $"0{time.Minutes}" : time.Minutes.ToString();
            var seconds = (time.Seconds < 10) ? $"0{time.Seconds}" : time.Seconds.ToString();
            var toSend = string.Format("{0}:{1}:{2}", hours, minutes, seconds);
            UptimeText = string.Format(Translate("command.uptime"), toSend);
        }
        else
        {
            UptimeText = Translate("streameroffline");
        }
        Chat.SendChatMessage(UptimeText);
    }

    private async void Shoutout(Client client, string[] args)
    {
        if (args.Length < 1)
        {
            Chat.SendChatMessage(Translate("NeedMoreArgs", "Sry i need more aguments to use this command, exemple: !so <name>"));
        }
        else
        {

            var list = args.ToList();
            list.RemoveAt(0);
            var user = list[0];
            var res = TrovoHandle.GetChannelInfoByChannelName(user);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<StreamModel>(content);
                var str = string.Format(Translate("shoutout"), user, model.channel_url);
                Chat.SendChatMessage(str);

            }
            else
            {
                Console.WriteLine(await res.Content.ReadAsStringAsync());
                Console.WriteLine("can't find", user);
            }
        }
    }

    // Edit View

    public void RegisterPrefix(string name, string value)
    {
        var prefix = new PrefixModel
        {
            PrefixName = name,
            PrefixValue = value
        };
        prefixes.AddIfNotExists(prefix);
    }

    public void RegisterCommand(string CommandName, dynamic value, List<UserRoles> role = null)
    {
        CommandName = CommandName.ToLower();
        if (!cmdHandle.TryGetValue(CommandName, out var handler))
        {
            handler = new HashSet<dynamic>();
            cmdHandle[CommandName] = handler;
        }
        handler.Add(value);
        if (role != null)
        {
            commandPermission.AddIfNotExists(CommandName, role);
        }
        Console.WriteLine($"Register Command {CommandName}");
    }

    public void HandleCommand(Client client, string CommandName, string[] args)
    {

        if (!cmdHandle.TryGetValue(CommandName, out var handlers))
        {
            Chat.SendChatMessage(Translate("commandNotFound", "Sorry, i don't know this Command!"));
            return;
        }

        // CommandText = Hallo {user} {channel} {point} 

        foreach (var handler in handlers)
        {
            switch (handler.GetType().ToString())
            {
                case "GloryBot.Models.CommandModel":
                    var cModel = (CommandModel)handler;
                    var commandText = string.Empty;
                    if (cModel.Roles.Contains(client.Role) || client.Role == UserRoles.Streamer || cModel.Roles.Contains(UserRoles.All))
                    {

                        commandText = cModel.CommandText.PregReplace(new string[]
                        {
                            "{user}"
                        }, new string[] { client.UserName });
                        Chat.SendChatMessage(commandText);
                    }
                    else
                    {
                        Chat.SendChatMessage(Translate("noCommandPermission", "You don't have the rights to run this command!"));
                    }
                    break;
                case "System.String":
                    List<UserRoles> cPermission = commandPermission[CommandName];
                    if (commandPermission != null && client.Role == UserRoles.Streamer || cPermission.Contains(client.Role) || cPermission.Contains(UserRoles.All))
                    {
                        Chat.SendChatMessage(handler);
                    }
                    else
                    {
                        Chat.SendChatMessage(Translate("noCommandPermission", "You don't have the rights to run this command!"));
                    }
                    break;
                default:
                    var cPerm = commandPermission[CommandName];
                    if (cPerm != null && client.Role == UserRoles.Streamer || cPerm.Contains(client.Role) || cPerm.Contains(UserRoles.All))
                    {
                        handler(client, args);
                    }
                    else
                    {
                        Chat.SendChatMessage(Translate("noCommandPermission", "You don't have the rights to run this command!"));
                    }
                    break;
            }

        }

    }
    /*
    private bool IsAction(Type type)
    {
        if (type == typeof(System.Action)) return true;
        Type generic = null;
        if (type.IsGenericTypeDefinition) generic = type;
        else if (type.IsGenericType) generic = type.GetGenericTypeDefinition();
        if (generic == null) return false;
        if (generic == typeof(System.Action<>)) return true;
        if (generic == typeof(System.Action<,>)) return true;
        return false;
    }*/

    internal CommandModel Find(Func<CommandModel, bool> lambda)
    {
        var commands = GetCommandList();
        return commands.Where(lambda).First();
    }
    public void UpdateCommand(string commandName, dynamic data)
    {
        if (cmdHandle.ContainsKey(commandName))
        {
            cmdHandle[commandName] = data;
        }
    }

    public void DeleteCommand(string commandName)
    {
        if (cmdHandle.ContainsKey(commandName))
        {
            cmdHandle.Remove(commandName);
        }
    }

    public List<CommandModel> GetCommandList()
    {
        var list = new List<CommandModel>();
        foreach (var key in cmdHandle.Keys)
        {
            if (cmdHandle.TryGetValue(key, out var handles))
            {
                foreach (var handle in handles)
                {
                    if (handle.GetType().ToString() == "GloryBot.Models.CommandModel")
                    {
                        list.Add((CommandModel)handle);
                    }
                }
            }
        }
        return list;
    }


}