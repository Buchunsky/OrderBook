using System.Net.WebSockets;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OrderBook.DTO;
using OrderBook.Services;

namespace OrderBook.UnitTests;

[TestFixture]
public class WebSocketServiceIntegrationTests
{
    private WebSocketService _webSocketServiceUnit;
    private const string WebSocketUrl = "wss://ws.bitstamp.net";
    
    [SetUp]
    public void Setup()
    {
        _webSocketServiceUnit = new WebSocketService();
    }
    
    [Test]
    public async Task SendAsync_ValidData_MessageIsSubscribed()
    {
        // Arrange
        var uri = new Uri(WebSocketUrl);
        var cancellationToken = CancellationToken.None;
        await _webSocketServiceUnit.ConnectAsync(uri, cancellationToken);
        
        var data = CreateSubscriptionMessage();
        var messageType = WebSocketMessageType.Text;

        // Act
        await _webSocketServiceUnit.SendAsync(data, messageType, cancellationToken);

        var buffer = new byte[8192];
        var webSocketResult = await _webSocketServiceUnit.ReceiveAsync(buffer, cancellationToken);
       
        //Assert
        ClassicAssert.IsTrue(System.Text.Encoding.UTF8.GetString(buffer, 0, webSocketResult.Count).Contains("subscription_succeeded"));
        await _webSocketServiceUnit.DisconnectAsync();
    }

    [Test]
    public async Task WebSocket_ValidData_OrderBookDataIsReceived()
    {
        // Arrange
        var uri = new Uri(WebSocketUrl);
        var cancellationToken = CancellationToken.None;
        await _webSocketServiceUnit.ConnectAsync(uri, cancellationToken);
        
        var data = CreateSubscriptionMessage();
        var messageType = WebSocketMessageType.Text;

        // Act
        await _webSocketServiceUnit.SendAsync(data, messageType, cancellationToken);

        var buffer = new byte[8192];
        //first message is for successful subscription
        await _webSocketServiceUnit.ReceiveAsync(buffer, cancellationToken);
       
        var webSocketResultWithOrderBookData = await _webSocketServiceUnit.ReceiveAsync(buffer, cancellationToken);
        var response = JsonConvert.DeserializeObject<GetOrdersProviderResponse>(System.Text.Encoding.UTF8.GetString(buffer, 0, webSocketResultWithOrderBookData.Count));
        //Assert
        
        ClassicAssert.AreEqual(true,  response != null && response.Body != null && response.Body.Asks.Count != 0);
        await _webSocketServiceUnit.DisconnectAsync();
    }
    
    private byte[] CreateSubscriptionMessage()
    {
        var subscriptionMessage = new
        {
            @event = "bts:subscribe",
            data = new
            {
                channel = "order_book_btceur"
            }
        };
        return System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(subscriptionMessage));
    }
}