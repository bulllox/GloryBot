using System.Net.WebSockets;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

namespace GloryBot.Handlers;


public class CustomWebSocket
{

    // public delegate void WebSocketResponse(object sender, string message);
    // public event WebSocketResponse OnResponse;
    // private ClientWebSocket _Client { get; set; }

    // public async void Connect(string AuthMessage)
    // {
    //     using (_Client = new ClientWebSocket())
    //     {
    //         var uri = new Uri(LinkCollection.ChatService);
    //         await _Client.ConnectAsync(uri, CancellationToken.None);
    //         SendMessage(AuthMessage);
    //         BeginRead();
    //     }
    // }

    // public async void SendMessage(string message)
    // {
    //     try {
    //     await _Client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);
    //     } catch(Exception ex) {
    //         Console.WriteLine(ex.Sysem);
    //     }
    // }

    // public void BeginRead()
    // {
    //     var buffer = new byte[1024];
    //     var result = _Client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).Result;
    //     var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
    //     OnResponseMessage(message);
    // }

    // protected virtual void OnResponseMessage(string message)
    // {
    //     OnResponse?.Invoke(this, message);
    // }
}