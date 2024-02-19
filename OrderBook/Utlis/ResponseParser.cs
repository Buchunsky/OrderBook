using OrderBook.DTO;
using OrderBook.Models.OrderBook;

namespace OrderBook.Utils;

public static class ResponceParser
{
    private const int ItemsAmount = 20;
    
    public static OrderBookModel Parce(GetOrdersProviderResponse responce)
    {
        OrderBookModel orderBookModel = new();
        
        foreach (var ask in responce.Body.Asks.Take(ItemsAmount))
        {
            //in the json that socket returns, price is the first item, amount is second
            if (decimal.TryParse(ask[0], out decimal price) && decimal.TryParse(ask[1], out decimal currencyAmount))
            {
                orderBookModel.Asks.Add(new OrderBookRecordModel(){EurPrice = price, CurrencyAmount = currencyAmount});
            }
        }

        foreach (var bid in responce.Body.Bids.Take(ItemsAmount))
        {
            if (decimal.TryParse(bid[0], out decimal price) && decimal.TryParse(bid[1], out decimal currencyAmount))
            {
                orderBookModel.Bids.Add(new OrderBookRecordModel(){EurPrice = price, CurrencyAmount = currencyAmount});
            }
        }

        return orderBookModel;
    }
}