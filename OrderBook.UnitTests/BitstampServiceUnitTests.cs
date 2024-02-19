using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OrderBook.Services;
using OrderBook.Services.Interfaces;

namespace OrderBook.UnitTests;

[TestFixture]
public class BitstampServiceUnitTests
{
    private BitstampService _bitstampService;
    private Mock<IWebSocketService> _mockWebSocketService;
    
    [SetUp]
    public void Setup()
    {
        _mockWebSocketService = new Mock<IWebSocketService>();
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("BitstampSocketUrl", "wss://ws.bitstamp.net")
            }!)
            .Build();
        _bitstampService = new BitstampService(configuration, _mockWebSocketService.Object);
    }
    
    [Test]
    public async Task ConnectAndStartReceivingOrderBook_CallsWebSocketConnectAsync()
    {
        // Arrange
        var marketSymbol = "btcusd";

        // Act
        await _bitstampService.ConnectAndStartReceivingOrderBook(marketSymbol);

        // Assert
        _mockWebSocketService.Verify(x => x.ConnectAsync(It.IsAny<Uri>(), default), Times.Once);
    }
    
    [Test]
    public async Task Disconnect_CallsWebSocketDisconnectAsync()
    {
        // Act
        await _bitstampService.Disconnect();

        // Assert
        _mockWebSocketService.Verify(x => x.DisconnectAsync(), Times.Once);
        ClassicAssert.IsFalse(_mockWebSocketService.Object.WebSocketIsOpen());
    }
}