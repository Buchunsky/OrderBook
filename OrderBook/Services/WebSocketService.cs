using System.Net.WebSockets;
using OrderBook.Services.Interfaces;

namespace OrderBook.Services;

public class WebSocketService : IWebSocketService
{
    private readonly ClientWebSocket _webSocket;

    public WebSocketService()
    {
        _webSocket = new ClientWebSocket();
    }

    public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
    {
        await _webSocket.ConnectAsync(uri, cancellationToken);
    }

    public async Task<WebSocketReceiveResult> ReceiveAsync(byte[] buffer, CancellationToken cancellationToken) =>
         await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

    public async Task SendAsync(byte[] data, WebSocketMessageType messageType, CancellationToken cancellationToken) =>
        await _webSocket.SendAsync(new ArraySegment<byte>(data), messageType, true, cancellationToken);
    

    public bool WebSocketIsOpen() => _webSocket.State == WebSocketState.Open;

    public async Task DisconnectAsync()
    {
        if (_webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }
}