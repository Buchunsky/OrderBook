using Bunit;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OrderBook.Models;
using OrderBook.Models.OrderBook;
using OrderBook.Pages;
using TestContext = Bunit.TestContext;

namespace OrderBook.UnitTests;

public class PagesUnitTests : TestContext
{ 
    [Test]
    public void OrderBookDisplay_RendersBidsAndAsks()
    {
        
        //Arrange
        var bids = new List<OrderBookRecordModel>
        {
            new OrderBookRecordModel {CurrencyAmount = 1, EurPrice = 50000},
            new OrderBookRecordModel {CurrencyAmount = 2, EurPrice = 51000}
        };

        var asks = new List<OrderBookRecordModel>
        {
            new OrderBookRecordModel {CurrencyAmount = 3, EurPrice = 52000},
            new OrderBookRecordModel {CurrencyAmount = 4, EurPrice = 53000}
        };
        
        var cut = RenderComponent<OrderBookView>(parameters =>
            parameters
                .Add(p => p.Bids, bids)
                .Add(p => p.Asks, asks)
        );
        
        // Act
        var bidBars = cut.FindAll(".bid-bar");
        var askBars = cut.FindAll(".ask-bar");

        // Assert
        ClassicAssert.AreEqual(bidBars.Count, 2);
        ClassicAssert.AreEqual(askBars.Count, 2);
        
        var bidBarHeights = bidBars.Select(bar => decimal.Parse(bar.Attributes["style"].Value
                .Split(';')
                .First(attr => attr.Trim().StartsWith("height:"))
                .Split(':')[1]
                .Trim('%')))
            .ToList();

        var askBarHeights = askBars.Select(bar => decimal.Parse(bar.Attributes["style"].Value
                .Split(';')
                .First(attr => attr.Trim().StartsWith("height:"))
                .Split(':')[1]
                .Trim('%')))
            .ToList();
        
        
        ClassicAssert.AreNotSame(bidBarHeights[0], bidBarHeights[1]);
        ClassicAssert.AreNotSame(askBarHeights[0], askBarHeights[1]);

    }
    
    [Test]
    public void ChangingEnteredBtcAmountCalculatesRequiredFiatAmount()
    {
        // Arrange
        var bids = new List<OrderBookRecordModel>
        {
            new OrderBookRecordModel { CurrencyAmount = 1, EurPrice = 50000 },
            new OrderBookRecordModel { CurrencyAmount = 2, EurPrice = 51000 }
        };

        var cut = RenderComponent<CurrencyInput>(parameters => 
            parameters
                .Add(p => p.Bids, bids)
        );
        
        // Act
        var inputElement = cut.Find("input");
        cut.Instance.EnteredBtcAmount = 2;
        inputElement.Input(2);
        
        var text = cut.Find(".fiat-amount");
        // Assert
        ClassicAssert.AreEqual(text.InnerHtml, "102000");
    }
    
    [Test]
    public void CalculateFiatAmount_CalculatesClosestBid()
    {
        // Arrange
        var bids = new List<OrderBookRecordModel>
        {
            new OrderBookRecordModel { CurrencyAmount = 1, EurPrice = 50000 },
            new OrderBookRecordModel { CurrencyAmount = 2, EurPrice = 51000 }
        };

        var cut = RenderComponent<CurrencyInput>(parameters =>
            parameters
                .Add(p => p.Bids, bids)
        );

        // Act
        var inputElement = cut.Find("input");
        cut.Instance.EnteredBtcAmount = 1.8m; // Set a BTC amount that falls between the two bids
        inputElement.Input(1.8m);

        // Assert
        var estimatedFiatAmount = cut.Find(".fiat-amount").TextContent.Trim();
        
        //closest bit is with btc = 2 and EurPrice = 51000, so this price should be get for calculating
        //user's input value (1.8 * 51000 = 91800)
        ClassicAssert.AreEqual("91800,0", estimatedFiatAmount);
    }
    
}