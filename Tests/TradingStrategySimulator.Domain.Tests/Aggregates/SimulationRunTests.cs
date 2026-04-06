using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingStrategySimulator.Domain.Aggregates;
using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Models;
using TradingStrategySimulator.Domain.Tests.Helpers;

namespace TradingStrategySimulator.Domain.Tests.Aggregates;

[TestClass]
public sealed class SimulationRunTests
{
    [TestMethod]
    public void Constructor_ShouldSetProperties_WhenArgumentsAreValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 110m);

        // Act
        var simulationRun = new SimulationRun(
            id,
            asset,
            StrategyType.BuyAndHold,
            1000m,
            2,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.AreEqual(id, simulationRun.Id);
        Assert.AreEqual(asset, simulationRun.Asset);
        Assert.AreEqual(StrategyType.BuyAndHold, simulationRun.StrategyType);
        Assert.AreEqual(1000m, simulationRun.InitialCash);
        Assert.AreEqual(2, simulationRun.QuantityPerTrade);
        Assert.AreEqual(2, simulationRun.PriceSeries.Count);
        Assert.AreEqual(0, simulationRun.Trades.Count);
        Assert.IsNull(simulationRun.Result);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 110m);

        // Act
        void action() => _ = new SimulationRun(
            Guid.Empty,
            asset,
            StrategyType.BuyAndHold,
            1000m,
            1,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentNullException_WhenAssetIsNull()
    {
        // Arrange
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 110m);

        // Act
        void action() => _ = new SimulationRun(
            Guid.NewGuid(),
            null!,
            StrategyType.BuyAndHold,
            1000m,
            1,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.ThrowsException<ArgumentNullException>(action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenInitialCashIsNegative()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 110m);

        // Act
        void action() => _ = new SimulationRun(
            Guid.NewGuid(),
            asset,
            StrategyType.BuyAndHold,
            -1m,
            1,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenQuantityPerTradeIsInvalid()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 110m);

        // Act
        void action() => _ = new SimulationRun(
            Guid.NewGuid(),
            asset,
            StrategyType.BuyAndHold,
            1000m,
            0,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentNullException_WhenPriceSeriesIsNull()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();

        // Act
        void action() => _ = new SimulationRun(
            Guid.NewGuid(),
            asset,
            StrategyType.BuyAndHold,
            1000m,
            1,
            null!,
            TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.ThrowsException<ArgumentNullException>(action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentException_WhenPriceSeriesIsEmpty()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();

        // Act
        void action() => _ = new SimulationRun(
            Guid.NewGuid(),
            asset,
            StrategyType.BuyAndHold,
            1000m,
            1,
            Enumerable.Empty<PricePoint>(),
            TestDataBuilder.CreateDefaultConstraints());

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [TestMethod]
    public void SetTrades_ShouldReplaceExistingTrades()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 110m, 120m);

        var simulationRun = new SimulationRun(
            Guid.NewGuid(),
            asset,
            StrategyType.BuyAndHold,
            1000m,
            1,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());

        var firstTradeSet = new List<Trade>
        {
            new(asset, priceSeries[0].Date, TradeSide.Buy, 100m, 1),
            new(asset, priceSeries[1].Date, TradeSide.Sell, 110m, 1)
        };

        var secondTradeSet = new List<Trade>
        {
            new(asset, priceSeries[1].Date, TradeSide.Buy, 110m, 1),
            new(asset, priceSeries[2].Date, TradeSide.Sell, 120m, 1)
        };

        simulationRun.SetTrades(firstTradeSet);

        // Act
        simulationRun.SetTrades(secondTradeSet);

        // Assert
        Assert.AreEqual(2, simulationRun.Trades.Count);
        Assert.AreEqual(priceSeries[1].Date, simulationRun.Trades.First().TradeDate);
        Assert.AreEqual(priceSeries[2].Date, simulationRun.Trades.Last().TradeDate);
    }

    [TestMethod]
    public void SetTrades_ShouldThrowArgumentNullException_WhenTradesAreNull()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 110m);

        var simulationRun = new SimulationRun(
            Guid.NewGuid(),
            asset,
            StrategyType.BuyAndHold,
            1000m,
            1,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());

        // Act
        void action() => simulationRun.SetTrades(null!);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(action);
    }

    [TestMethod]
    public void Complete_ShouldSetResult()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 120m);

        var simulationRun = new SimulationRun(
            Guid.NewGuid(),
            asset,
            StrategyType.BuyAndHold,
            1000m,
            1,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());

        var result = new SimulationResult(
            asset,
            StrategyType.BuyAndHold,
            1000m,
            1020m,
            20m,
            0m,
            0m,
            1020m,
            0.2m,
            1,
            1,
            0,
            false,
            new List<Trade>());

        // Act
        simulationRun.Complete(result);

        // Assert
        Assert.IsNotNull(simulationRun.Result);
        Assert.AreEqual(20m, simulationRun.Result.RealizedProfitLoss);
        Assert.AreEqual(0m, simulationRun.Result.UnrealizedProfitLoss);
        Assert.AreEqual(1020m, simulationRun.Result.NetLiquidationValue);
    }

    [TestMethod]
    public void Complete_ShouldThrowArgumentNullException_WhenResultIsNull()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset();
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 120m);

        var simulationRun = new SimulationRun(
            Guid.NewGuid(),
            asset,
            StrategyType.BuyAndHold,
            1000m,
            1,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());

        // Act
        void action() => simulationRun.Complete(null!);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(action);
    }
}
