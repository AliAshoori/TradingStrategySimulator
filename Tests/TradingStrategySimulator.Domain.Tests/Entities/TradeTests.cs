using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Tests.Helpers;

namespace TradingStrategySimulator.Domain.Tests.Entities;

[TestClass]
public sealed class TradeTests
{
    [TestMethod]
    public void Constructor_ShouldSetProperties_WhenArgumentsAreValid()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset("AAPL");
        var tradeDate = new DateOnly(2026, 1, 5);

        // Act
        var trade = new Trade(asset, tradeDate, TradeSide.Buy, 100m, 3);

        // Assert
        Assert.AreEqual(asset, trade.Asset);
        Assert.AreEqual(tradeDate, trade.TradeDate);
        Assert.AreEqual(TradeSide.Buy, trade.Side);
        Assert.AreEqual(100m, trade.Price);
        Assert.AreEqual(3, trade.Quantity);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentNullException_WhenAssetIsNull()
    {
        // Arrange
        var tradeDate = new DateOnly(2026, 1, 5);

        // Act
        void Action() => _ = new Trade(null!, tradeDate, TradeSide.Buy, 100m, 1);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(Action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenPriceIsZero()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();

        // Act
        void Action() => _ = new Trade(asset, new DateOnly(2026, 1, 5), TradeSide.Buy, 0m, 1);

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(Action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenPriceIsNegative()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();

        // Act
        void Action() => _ = new Trade(asset, new DateOnly(2026, 1, 5), TradeSide.Buy, -10m, 1);

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(Action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenQuantityIsZero()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();

        // Act
        void Action() => _ = new Trade(asset, new DateOnly(2026, 1, 5), TradeSide.Buy, 100m, 0);

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(Action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenQuantityIsNegative()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();

        // Act
        void Action() => _ = new Trade(asset, new DateOnly(2026, 1, 5), TradeSide.Buy, 100m, -1);

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(Action);
    }

    [TestMethod]
    public void GrossAmount_ShouldReturnPriceMultipliedByQuantity()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var trade = new Trade(asset, new DateOnly(2026, 1, 5), TradeSide.Sell, 125.50m, 4);

        // Act
        var grossAmount = trade.GrossAmount;

        // Assert
        Assert.AreEqual(502.00m, grossAmount);
    }

    [TestMethod]
    public void ToString_ShouldContainRelevantTradeInformation()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset("MSFT");
        var trade = new Trade(asset, new DateOnly(2026, 1, 5), TradeSide.Sell, 250m, 2);

        // Act
        var result = trade.ToString();

        // Assert
        StringAssert.Contains(result, "2026-01-05");
        StringAssert.Contains(result, "Sell");
        StringAssert.Contains(result, "MSFT");
        StringAssert.Contains(result, "Qty=2");
        StringAssert.Contains(result, "Px=250");
    }
}