using OrderBook.Models.OrderBook;

namespace OrderBook.Services.Interfaces;

/// <summary>
/// Service for interacting with the Bitstamp API via WebSocket.
/// </summary>
public interface IBitstampService
{ 
    /// <summary>
    /// Event raised when new order book data is received.
    /// </summary>
    event EventHandler<OrderBookModel> OnDataReceived;
    
    /// <summary>
    /// Connects to the Bitstamp WebSocket API and starts receiving order book data.
    /// </summary>
    /// <param name="marketSymbol">The market symbol to subscribe to.</param>
    Task ConnectAndStartReceivingOrderBook(string marketSymbol);
    
    /// <summary>
    /// Disconnects from the Bitstamp WebSocket API.
    /// </summary>
    Task Disconnect();
}