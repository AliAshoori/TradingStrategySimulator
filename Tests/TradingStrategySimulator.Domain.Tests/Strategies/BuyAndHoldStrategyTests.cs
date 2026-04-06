using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Exceptions;
using TradingStrategySimulator.Domain.Strategies;
using TradingStrategySimulator.Domain.Tests.Helpers;

namespace TradingStrategySimulator.Domain.Tests.Strategies;

[TestClass]
public sealed class BuyAndHoldStrategyTests
{
    private readonly BuyAndHoldStrategy _strategy = new();

    [TestMethod]
    public void StrategyType_ShouldBeBuyAndHold()
    {
        // Arrange
        // Act
        var strategyType = _strategy.StrategyType;

        // Assert
        Assert.AreEqual(StrategyType.BuyAndHold, strategyType);
    }

    [TestMethod]
    public void GenerateTrades_ShouldThrowArgumentNullException_WhenAssetIsNull()
    {
        // Arrange
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 110m);

        // Act
        void Action() => _strategy.GenerateTrades(null!, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.ThrowsException<ArgumentNullException>(Action);
    }

    [TestMethod]
    public void GenerateTrades_ShouldThrowArgumentNullException_WhenPriceSeriesIsNull()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();

        // Act
        void Action() => _strategy.GenerateTrades(asset, null!, 1, TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.ThrowsException<ArgumentNullException>(Action);
    }

    [TestMethod]
    public void GenerateTrades_ShouldThrowInvalidSimulationException_WhenPriceSeriesIsEmpty()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = new List<TradingStrategySimulator.Domain.Entities.PricePoint>();

        // Act
        void Action() => _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void GenerateTrades_ShouldThrowArgumentOutOfRangeException_WhenQuantityIsZero()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 110m);

        // Act
        void Action() => _strategy.GenerateTrades(asset, priceSeries, 0, TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(Action);
    }

    [TestMethod]
    public void GenerateTrades_ShouldReturnEmpty_WhenPriceSeriesHasOnePoint()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.AreEqual(0, trades.Count);
    }

    [TestMethod]
    public void GenerateTrades_ShouldReturnBuyThenSell_WhenPriceSeriesHasAtLeastTwoPoints()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 105m, 120m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 2, TestDataBuilder.CreateDefaultConstraints()).ToList();

        // Assert
        Assert.AreEqual(2, trades.Count);

        Assert.AreEqual(TradeSide.Buy, trades[0].Side);
        Assert.AreEqual(priceSeries[0].Date, trades[0].TradeDate);
        Assert.AreEqual(100m, trades[0].Price);
        Assert.AreEqual(2, trades[0].Quantity);

        Assert.AreEqual(TradeSide.Sell, trades[1].Side);
        Assert.AreEqual(priceSeries[^1].Date, trades[1].TradeDate);
        Assert.AreEqual(120m, trades[1].Price);
        Assert.AreEqual(2, trades[1].Quantity);
    }

    [TestMethod]
    public void GenerateTrades_ShouldUseSameAssetForAllTrades()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset("TSLA");
        var priceSeries = TestDataBuilder.CreatePriceSeries(200m, 220m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints()).ToList();

        // Assert
        Assert.IsTrue(trades.All(trade => trade.Asset.Equals(asset)));
    }
}