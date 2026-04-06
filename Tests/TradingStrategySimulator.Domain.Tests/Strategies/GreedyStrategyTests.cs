using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Exceptions;
using TradingStrategySimulator.Domain.Strategies;
using TradingStrategySimulator.Domain.Tests.Helpers;

namespace TradingStrategySimulator.Domain.Tests.Strategies;

[TestClass]
public sealed class GreedyStrategyTests
{
    private readonly GreedyStrategy _strategy = new();

    [TestMethod]
    public void StrategyType_ShouldBeGreedy()
    {
        // Arrange
        // Act
        var strategyType = _strategy.StrategyType;

        // Assert
        Assert.AreEqual(StrategyType.Greedy, strategyType);
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
    public void GenerateTrades_ShouldThrowArgumentOutOfRangeException_WhenQuantityIsInvalid()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 110m);

        // Act
        void Action() => _strategy.GenerateTrades(asset, priceSeries, -1, TestDataBuilder.CreateDefaultConstraints());

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
    public void GenerateTrades_ShouldReturnEmpty_WhenPricesAreStrictlyDescending()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(120m, 110m, 100m, 90m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.AreEqual(0, trades.Count);
    }

    [TestMethod]
    public void GenerateTrades_ShouldReturnEmpty_WhenPricesAreFlat()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 100m, 100m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.AreEqual(0, trades.Count);
    }

    [TestMethod]
    public void GenerateTrades_ShouldSkipNearbySignals_WhenCooldownIsTwoDays()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(10m, 12m, 11m, 15m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints()).ToList();

        // Assert
        Assert.AreEqual(2, trades.Count);

        Assert.AreEqual(TradeSide.Buy, trades[0].Side);
        Assert.AreEqual(10m, trades[0].Price);
        Assert.AreEqual(TradeSide.Sell, trades[1].Side);
        Assert.AreEqual(12m, trades[1].Price);

    }

    [TestMethod]
    public void GenerateTrades_ShouldRespectQuantityPerTrade()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(10m, 12m, 11m, 15m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 3, TestDataBuilder.CreateDefaultConstraints()).ToList();

        // Assert
        Assert.IsTrue(trades.All(trade => trade.Quantity == 3));
    }

    [TestMethod]
    public void GenerateTrades_ShouldUseConsecutiveDatesForEachPair()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(10m, 12m, 11m, 15m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints()).ToList();

        // Assert
        Assert.AreEqual(2, trades.Count);
        Assert.AreEqual(priceSeries[0].Date, trades[0].TradeDate);
        Assert.AreEqual(priceSeries[1].Date, trades[1].TradeDate);
    }
}
