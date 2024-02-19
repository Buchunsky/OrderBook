namespace OrderBook.Models.OrderBook;

public class OrderBookModel
{
    public List<OrderBookRecordModel> Asks { get; set; } = [];
    public List<OrderBookRecordModel> Bids { get; set; } = [];
}