using System.Net.WebSockets;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OrderBook.Models;
using OrderBook.Pages;
using OrderBook.Services;

namespace OrderBook.UnitTests;

[TestFixture]
public class WebSocketServiceUnitTests
{
    private WebSocketService _webSocketServiceUnit;
    private const string WebSocketUrl = "wss://ws.bitstamp.net";

    [SetUp]
    public void Setup()
    {
        _webSocketServiceUnit = new WebSocketService();
    }
    
    [Test]
    public async Task ConnectAsync_ValidUri_CanConnectToWebSocket()
    {
        // Arrange
        var uri = new Uri(WebSocketUrl);
        var cancellationToken = CancellationToken.None;

        // Act
        await _webSocketServiceUnit.ConnectAsync(uri, cancellationToken);

        // Assert
        ClassicAssert.IsTrue(_webSocketServiceUnit.WebSocketIsOpen());
        await _webSocketServiceUnit.DisconnectAsync();
    }
    
    [Test]
    public async Task DisconnectAsync_AfterConnecting_CanDisconnectFromWebSocket()
    {
        // Arrange
        var uri = new Uri(WebSocketUrl);
        var cancellationToken = CancellationToken.None;
        await _webSocketServiceUnit.ConnectAsync(uri, cancellationToken);

        // Act
        await _webSocketServiceUnit.DisconnectAsync();

        // Assert
        ClassicAssert.IsFalse(_webSocketServiceUnit.WebSocketIsOpen());
    }
    
    [Test]
    public async Task SendAsync_MessageIsSent()
    {
        // Arrange
        var uri = new Uri(WebSocketUrl);
        var cancellationToken = CancellationToken.None;
        await _webSocketServiceUnit.ConnectAsync(uri, cancellationToken);

        var data = new byte[] { 1, 2, 3 };
        var messageType = WebSocketMessageType.Binary;

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _webSocketServiceUnit.SendAsync(data, messageType, cancellationToken));
    }
    
    [Test]
    public async Task ReceiveAsync_ValidData_MessageIsReceived()
    {
        // Arrange
        var uri = new Uri(WebSocketUrl);
        var cancellationToken = CancellationToken.None;
        await _webSocketServiceUnit.ConnectAsync(uri, cancellationToken);

        var originalData = new byte[] { 1, 2, 3 };
        var messageType = WebSocketMessageType.Binary;
        
        // Act
        await _webSocketServiceUnit.SendAsync(originalData, messageType, cancellationToken);
        var buffer = new byte[1024];
        var result = await _webSocketServiceUnit.ReceiveAsync(buffer, cancellationToken);
        
        // Assert
        ClassicAssert.Greater(result.Count, 0);
    }
}