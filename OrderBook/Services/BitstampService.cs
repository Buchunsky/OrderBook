using System.Net.WebSockets;
using Newtonsoft.Json;
using NLog;
using OrderBook.DTO;
using OrderBook.Utils;
using OrderBook.Models.Logging;
using OrderBook.Models.OrderBook;
using OrderBook.Services.Interfaces;


namespace OrderBook.Services;

public class BitstampService : IBitstampService
{
    private readonly IWebSocketService _webSocketService;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly string? _bitstampSocketUrl;
    private readonly Logger _logger;

    public event EventHandler<OrderBookModel>? OnDataReceived;

    private const int BufferSize = 8192;

    public BitstampService(IConfiguration configuration, IWebSocketService webSocketService)
    {
        _logger = LogManager.GetCurrentClassLogger();
        _bitstampSocketUrl = configuration["BitstampSocketUrl"];

        _cancellationTokenSource = new CancellationTokenSource();
        _webSocketService = webSocketService;
    }

    public async Task ConnectAndStartReceivingOrderBook(string marketSymbol)
    {
        await _webSocketService.ConnectAsync(new Uri(_bitstampSocketUrl), CancellationToken.None);

        var bytes = System.Text.Encoding.UTF8.GetBytes(CreateSubscriptionMessage(marketSymbol));

        await _webSocketService.SendAsync(bytes, WebSocketMessageType.Text, _cancellationTokenSource.Token);
        await Task.Factory.StartNew(_ => ReceiveLoop(), TaskCreationOptions.LongRunning);
    }

    private async Task ReceiveLoop()
    {
        while (_webSocketService.WebSocketIsOpen())
        {
            var buffer = new byte[BufferSize];
            var webSocketResult = await _webSocketService.ReceiveAsync(buffer, _cancellationTokenSource.Token);

            try
            {
                NotifySubscribersWithDataAndLogData(buffer, webSocketResult);
            }
            catch (Exception e)
            {
                _logger.Error($"Error while receiving data: {e.Message} \n StackTrace: {e.StackTrace}");
            }
        }
    }

    public async Task Disconnect()
    {
        _cancellationTokenSource.Cancel();
        await _webSocketService.DisconnectAsync();
    }

    private void NotifySubscribersWithDataAndLogData(byte[] buffer, WebSocketReceiveResult webSocketResult)
    {
        var responseString = System.Text.Encoding.UTF8.GetString(buffer, 0, webSocketResult.Count);
        var response = JsonConvert.DeserializeObject<GetOrdersProviderResponse>(responseString);

        if (response != null && response?.Body.Bids != null)
        {
            var orderBookModel = ResponceParser.Parce(response);
            LogOrderBookSnapshot(response.Body.Microtimestamp, orderBookModel);
            OnDataReceived?.Invoke(this, orderBookModel);
        }
    }
    
    private void LogOrderBookSnapshot(long microtimestamp, OrderBookModel orderBook)
    {
        OrderBookSnapshot snapshot = new(microtimestamp, orderBook.Asks, orderBook.Bids);
        //logging only showed records to the user
        _logger.Info(
            $"Received new order book snapshot from {snapshot.Timestamp} : {JsonConvert.SerializeObject(snapshot.OrderBook)}");
    }

    private string CreateSubscriptionMessage(string marketSymbol)
    {
        var subscriptionMessage = new
        {
            @event = "bts:subscribe",
            data = new
            {
                channel = $"order_book_{marketSymbol}"
            }
        };

        return JsonConvert.SerializeObject(subscriptionMessage);
    }
}