using OrderBook.Models.OrderBook;

namespace OrderBook.Models.Logging;

public class OrderBookSnapshot
{
    public DateTimeOffset Timestamp { get; set; }
    public OrderBookModel OrderBook { get; set; }

    public OrderBookSnapshot(long microtimestamp, List<OrderBookRecordModel> asks, List<OrderBookRecordModel> bids)
    {
        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(microtimestamp / 1000);
        OrderBook = new OrderBookModel
        {
            Asks = asks,
            Bids = bids
        };
    }
}