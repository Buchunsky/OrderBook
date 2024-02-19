using System.Net.WebSockets;

namespace OrderBook.Services.Interfaces;

/// <summary>
/// Service for managing WebSocket connections.
/// </summary>
public interface IWebSocketService
{
    
    /// <summary>
    /// Connects to the WebSocket server.
    /// </summary>
    /// <param name="uri">The URI of the WebSocket server.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ConnectAsync(Uri uri, CancellationToken cancellationToken);
    
    /// <summary>
    /// Receives data from the WebSocket server.
    /// </summary>
    /// <param name="buffer">The buffer to store the received data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the receive operation.</returns>
    Task<WebSocketReceiveResult> ReceiveAsync(byte[] buffer, CancellationToken cancellationToken);
    
    /// <summary>
    /// Sends data to the WebSocket server.
    /// </summary>
    /// <param name="data">The data to send.</param>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SendAsync(byte[] data, WebSocketMessageType messageType, CancellationToken cancellationToken);
    
    /// <summary>
    /// Checks whether the WebSocket connection is open.
    /// </summary>
    /// <returns>True if the WebSocket connection is open; otherwise, false.</returns>
    bool WebSocketIsOpen();
    
    /// <summary>
    /// Disconnects from the WebSocket server.
    /// </summary>
    Task DisconnectAsync();
}