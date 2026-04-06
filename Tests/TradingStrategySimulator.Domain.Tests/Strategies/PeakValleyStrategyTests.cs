using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Exceptions;
using TradingStrategySimulator.Domain.Strategies;
using TradingStrategySimulator.Domain.Tests.Helpers;

namespace TradingStrategySimulator.Domain.Tests.Strategies;

[TestClass]
public sealed class PeakValleyStrategyTests
{
    private readonly PeakValleyStrategy _strategy = new();

    [TestMethod]
    public void StrategyType_ShouldBePeakValley()
    {
        // Arrange
        // Act
        var strategyType = _strategy.StrategyType;

        // Assert
        Assert.AreEqual(StrategyType.PeakValley, strategyType);
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
    public void GenerateTrades_ShouldReturnEmpty_WhenPricesAreStrictlyDescending()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(10m, 9m, 8m, 7m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.AreEqual(0, trades.Count);
    }

    [TestMethod]
    public void GenerateTrades_ShouldReturnOneTradePair_WhenSeriesHasSingleUpwardSwing()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(10m, 8m, 6m, 9m, 12m, 11m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints()).ToList();

        // Assert
        Assert.AreEqual(2, trades.Count);
        Assert.AreEqual(TradeSide.Buy, trades[0].Side);
        Assert.AreEqual(6m, trades[0].Price);
        Assert.AreEqual(TradeSide.Sell, trades[1].Side);
        Assert.AreEqual(12m, trades[1].Price);
    }

    [TestMethod]
    public void GenerateTrades_ShouldSkipMiddleSwing_WhenCooldownIsTwoDays()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(9m, 7m, 8m, 6m, 10m, 5m, 11m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints()).ToList();

        // Assert
        Assert.AreEqual(4, trades.Count);

        Assert.AreEqual(TradeSide.Buy, trades[0].Side);
        Assert.AreEqual(7m, trades[0].Price);
        Assert.AreEqual(TradeSide.Sell, trades[1].Side);
        Assert.AreEqual(8m, trades[1].Price);

        Assert.AreEqual(TradeSide.Buy, trades[2].Side);
        Assert.AreEqual(5m, trades[2].Price);
        Assert.AreEqual(TradeSide.Sell, trades[3].Side);
        Assert.AreEqual(11m, trades[3].Price);
    }

    [TestMethod]
    public void GenerateTrades_ShouldAlternateBuyAndSell()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(10m, 8m, 12m, 7m, 11m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 1, TestDataBuilder.CreateDefaultConstraints()).ToList();

        // Assert
        Assert.AreEqual(2, trades.Count);
        for (int index = 0; index < trades.Count; index += 2)
        {
            Assert.AreEqual(TradeSide.Buy, trades[index].Side);
            Assert.AreEqual(TradeSide.Sell, trades[index + 1].Side);
        }
    }

    [TestMethod]
    public void GenerateTrades_ShouldRespectQuantityPerTrade()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(10m, 8m, 12m);

        // Act
        var trades = _strategy.GenerateTrades(asset, priceSeries, 4, TestDataBuilder.CreateDefaultConstraints()).ToList();

        // Assert
        Assert.IsTrue(trades.All(trade => trade.Quantity == 4));
    }
}
