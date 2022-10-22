using DiscordRPC;
using DiscordRPC.IO;
using DiscordRPC.Logging;

namespace GloryBot.Handlers
{
    public class DiscordHandle
    {
        private DiscordRpcClient _rpcClient;
        public bool DiscordState = false;
        public void StartDiscord(string clientid)
        {
            _rpcClient = new(clientid);
           
            _rpcClient.Initialize();
        }
        public void SetInfo(string largeImage, string smallImage, string url)
        {
            if (_rpcClient != null)
            {
                _rpcClient.SetPresence(new RichPresence
                {
                    Details = Translate("discord.detail", "streaming Trovo.live"),
                    State = string.Format(Translate("discord.state", "Is live {0}"), streamInfo.category_name),
                    Buttons = new Button[1]
                        {
                        new Button
                        {
                            Label = Translate("discord.watch", "Watch"),
                            Url = url
                        }
                        },
                    Assets = new Assets
                    {
                        LargeImageKey = largeImage,
                        LargeImageText = "",
                        SmallImageKey = smallImage
                    }
                });
                DiscordState = true;
            }
        }

        public void StartUpdateTimer()
        {
            if (_rpcClient != null)
            {
                _rpcClient.UpdateStartTime();
            }
        }
    }
}
