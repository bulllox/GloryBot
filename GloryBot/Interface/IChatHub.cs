namespace GloryBot.Interface
{
    public interface IChatHub
    {
        public Task ReceiveMessage(string avatar, string color, string username, string message);
        public Task ClearChat();

    }
}
