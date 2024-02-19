using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OrderBook.Models;
using OrderBook.Models.OrderBook;
using OrderBook.Services;

namespace OrderBook.UnitTests;

public class BitstampServiceIntegrationTests
{
    private BitstampService _bitstampService;
    private ManualResetEventSlim _dataReceivedEvent;
    
    private OrderBookModel _model;
    
    [SetUp]
    public void Setup()
    {
        _dataReceivedEvent = new ManualResetEventSlim(false);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("BitstampSocketUrl", "wss://ws.bitstamp.net")
            }!)
            .Build();
        
        _bitstampService = new BitstampService(configuration, new WebSocketService());
        _bitstampService.OnDataReceived += (sender, model) =>
        {
            _model = model;
            _dataReceivedEvent.Set();
        };
    }
    
    [Test]
    public async Task ConnectAndStartReceivingOrderBook_ValidMarketSymbol_ReceivesData()
    {
        // Arrange
        var marketSymbol = "btcusd";

        // Act
        await _bitstampService.ConnectAndStartReceivingOrderBook(marketSymbol);
        if (!_dataReceivedEvent.Wait(TimeSpan.FromSeconds(3)))
        {
            Assert.Fail("Timed out waiting for data to be received.");
        }
        // Assert
        ClassicAssert.IsTrue(_model != null);
    }
    
    [Test]
    public async Task ConnectAndStartReceivingOrderBook_ValidMarketSymbol_ReceivesMultipleData()
    {
        // Arrange
        var marketSymbol = "btcusd";
        var dataReceivedCount = 0;

        _bitstampService.OnDataReceived += (sender, model) =>
        {
            dataReceivedCount++;
        };

        // Act
        await _bitstampService.ConnectAndStartReceivingOrderBook(marketSymbol);

        await Task.Delay(TimeSpan.FromSeconds(3));

        // Assert
        ClassicAssert.Greater(dataReceivedCount, 1);
    }
    
    [Test]
    public async Task ConnectAndStartReceivingOrderBook_InvalidMarketSymbol_FailsToConnect()
    {
        // Arrange
        var invalidMarketSymbol = "invalidsymbol";

        // Act
        var dataReceivedCount = 0;
        _bitstampService.OnDataReceived += (sender, model) =>
        {
            dataReceivedCount++;
        };
        await _bitstampService.ConnectAndStartReceivingOrderBook(invalidMarketSymbol);
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        
        ClassicAssert.AreEqual(dataReceivedCount, 0);
    }
    
    [Test]
    public async Task BitstampService_DisconnectsAndStopsReceivingData()
    {
        // Arrange
        var marketSymbol = "btcusd";
        var dataReceivedCount = 0;

        _bitstampService.OnDataReceived += async (sender, model) =>
        {
            dataReceivedCount++;
            
            if (dataReceivedCount == 4)
            {
                await _bitstampService.Disconnect();
            }
        };

        // Act
        await _bitstampService.ConnectAndStartReceivingOrderBook(marketSymbol);
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Assert
        ClassicAssert.Less(dataReceivedCount, 5);
    }
}