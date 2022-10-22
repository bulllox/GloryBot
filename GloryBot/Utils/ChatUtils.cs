using GloryBot.Handlers;
using System.Linq;
using System.Net.Http;

namespace GloryBot.Utils
{
    public class ChatUtils
    {
        public bool ChatConnectStatus = false;
        public bool ChatOpen = false;
        public ChatHandle ChatHandle { get; set; } = new();
        // public static ChatHandle TrovoChat { get; set; } = new();
        public List<Dictionary<string, string>> ChatMessages { get; set; } = new();
        public List<EmoteModel> Emotes { get; set; } = new();
        public Dictionary<string, PluginModel> PluginList { get; set; } = new();
        public CommandHandler CommandInstance { get; set; } = new();
        public bool ChatDataSet { get; set; } = false;

        public List<AlertModel> Alerts { get; set; } = new();

        public List<NodificationModel> NodyList { get; set; } = new();
        public List<string> ChatConnections { get; set; } = new();
        public List<string> WelcomeList = new();
        public void LoadNodifications()
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var col = db.GetCollection<NodificationModel>("nodifications");

                NodyList = col.FindAll().ToList();
            }
        }

        public void WelcomeUser(string nick_name)
        {
            if (!WelcomeList.Contains(nick_name))
            {
                // Send Chat Message
                var msg = string.Format(Translate("welcomeMessage", nick_name));
                Chat.SendChatMessage(msg);
                WelcomeList.Add(nick_name);
            }
        }

        public Dictionary<string, dynamic> SplitCommand(String content)
        {
            var args = content.Split(" ");
            var cmdName = args[0].Replace("!", "");
            args = args.Where(w => w != args[0]).ToArray();

            var dict = new Dictionary<string, dynamic>
            {
                { "command", cmdName },
            { "args", args }
            };
            return dict;
        }


    }
}
