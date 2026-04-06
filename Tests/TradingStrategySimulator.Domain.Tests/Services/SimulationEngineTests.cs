using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingStrategySimulator.Domain.Aggregates;
using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Exceptions;
using TradingStrategySimulator.Domain.Services;
using TradingStrategySimulator.Domain.Tests.Helpers;

namespace TradingStrategySimulator.Domain.Tests.Services;

[TestClass]
public sealed class SimulationEngineTests
{
    private readonly SimulationEngine _simulationEngine = new();

    [TestMethod]
    public void Execute_ShouldThrowArgumentNullException_WhenSimulationRunIsNull()
    {
        // Arrange

        // Act
        void action() => _simulationEngine.Execute(null!);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(action);
    }

    [TestMethod]
    public void Execute_ShouldReturnZeroProfitAndZeroUnrealized_WhenThereAreNoTrades()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);

        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(1000m, result.InitialCash);
        Assert.AreEqual(1000m, result.FinalCash);
        Assert.AreEqual(0m, result.RealizedProfitLoss);
        Assert.AreEqual(0m, result.UnrealizedProfitLoss);
        Assert.AreEqual(0m, result.OpenPositionMarketValue);
        Assert.AreEqual(1000m, result.NetLiquidationValue);
        Assert.AreEqual(0, result.CompletedTradeCount);
        Assert.AreEqual(0, result.WinningTradeCount);
        Assert.AreEqual(0, result.LosingTradeCount);
        Assert.IsFalse(result.HasOpenPosition);
        Assert.AreEqual(0, result.Trades.Count);
    }

    [TestMethod]
    public void Execute_ShouldCalculateRealizedProfitCorrectly_ForSingleWinningTrade()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var priceSeries = simulationRun.PriceSeries.ToList();

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, priceSeries[0].Date, TradeSide.Buy, 100m, 1),
            new(asset, priceSeries[1].Date, TradeSide.Sell, 120m, 1)
        });

        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(1019.78m, result.FinalCash);
        Assert.AreEqual(19.78m, result.RealizedProfitLoss);
        Assert.AreEqual(0m, result.UnrealizedProfitLoss);
        Assert.AreEqual(0m, result.OpenPositionMarketValue);
        Assert.AreEqual(1019.78m, result.NetLiquidationValue);
        Assert.AreEqual(1, result.CompletedTradeCount);
        Assert.AreEqual(1, result.WinningTradeCount);
        Assert.AreEqual(0, result.LosingTradeCount);
        Assert.IsFalse(result.HasOpenPosition);
    }

    [TestMethod]
    public void Execute_ShouldCalculateRealizedLossCorrectly_ForSingleLosingTrade()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var priceSeries = simulationRun.PriceSeries.ToList();

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, priceSeries[0].Date, TradeSide.Buy, 120m, 1),
            new(asset, priceSeries[1].Date, TradeSide.Sell, 100m, 1)
        });

        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(979.78m, result.FinalCash);
        Assert.AreEqual(-20.22m, result.RealizedProfitLoss);
        Assert.AreEqual(0m, result.UnrealizedProfitLoss);
        Assert.AreEqual(0m, result.OpenPositionMarketValue);
        Assert.AreEqual(979.78m, result.NetLiquidationValue);
        Assert.AreEqual(1, result.CompletedTradeCount);
        Assert.AreEqual(0, result.WinningTradeCount);
        Assert.AreEqual(1, result.LosingTradeCount);
        Assert.IsFalse(result.HasOpenPosition);
    }

    [TestMethod]
    public void Execute_ShouldCalculateAggregateRealizedProfitCorrectly_ForMultipleClosedTrades()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var priceSeries = simulationRun.PriceSeries.ToList();

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, priceSeries[0].Date, TradeSide.Buy, 100m, 1),
            new(asset, priceSeries[1].Date, TradeSide.Sell, 120m, 1),
            new(asset, priceSeries[2].Date, TradeSide.Buy, 90m, 1),
            new(asset, priceSeries[3].Date, TradeSide.Sell, 110m, 1)
        });

        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(1039.58m, result.FinalCash);
        Assert.AreEqual(39.58m, result.RealizedProfitLoss);
        Assert.AreEqual(0m, result.UnrealizedProfitLoss);
        Assert.AreEqual(0m, result.OpenPositionMarketValue);
        Assert.AreEqual(1039.58m, result.NetLiquidationValue);
        Assert.AreEqual(2, result.CompletedTradeCount);
        Assert.AreEqual(2, result.WinningTradeCount);
        Assert.AreEqual(0, result.LosingTradeCount);
        Assert.IsFalse(result.HasOpenPosition);
    }

    [TestMethod]
    public void Execute_ShouldTrackWinningAndLosingTradesCorrectly_ForClosedTradesOnly()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var priceSeries = simulationRun.PriceSeries.ToList();

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, priceSeries[0].Date, TradeSide.Buy, 100m, 1),
            new(asset, priceSeries[1].Date, TradeSide.Sell, 120m, 1),
            new(asset, priceSeries[2].Date, TradeSide.Buy, 90m, 1),
            new(asset, priceSeries[3].Date, TradeSide.Sell, 80m, 1)
        });

        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(9.61m, result.RealizedProfitLoss);
        Assert.AreEqual(0m, result.UnrealizedProfitLoss);
        Assert.AreEqual(1, result.WinningTradeCount);
        Assert.AreEqual(1, result.LosingTradeCount);
        Assert.AreEqual(2, result.CompletedTradeCount);
        Assert.IsFalse(result.HasOpenPosition);
    }

    [TestMethod]
    public void Execute_ShouldTreatEqualPriceTradeAsLosing_AfterTransactionCosts()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var priceSeries = simulationRun.PriceSeries.ToList();

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, priceSeries[0].Date, TradeSide.Buy, 100m, 1),
            new(asset, priceSeries[1].Date, TradeSide.Sell, 100m, 1)
        });

        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(999.8m, result.FinalCash);
        Assert.AreEqual(-0.2m, result.RealizedProfitLoss);
        Assert.AreEqual(0m, result.UnrealizedProfitLoss);
        Assert.AreEqual(0m, result.OpenPositionMarketValue);
        Assert.AreEqual(999.8m, result.NetLiquidationValue);
        Assert.AreEqual(1, result.CompletedTradeCount);
        Assert.AreEqual(0, result.WinningTradeCount);
        Assert.AreEqual(1, result.LosingTradeCount);
        Assert.IsFalse(result.HasOpenPosition);
    }

    [TestMethod]
    public void Execute_ShouldReturnUnrealizedProfit_WhenPositionRemainsOpenAndLatestPriceIsHigher()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var firstDate = simulationRun.PriceSeries.First().Date;

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, firstDate, TradeSide.Buy, 100m, 1)
        });

        // Latest price in CreateSimulationRun series is 110m.
        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(899.9m, result.FinalCash);
        Assert.AreEqual(0m, result.RealizedProfitLoss);
        Assert.AreEqual(9.9m, result.UnrealizedProfitLoss);
        Assert.AreEqual(110m, result.OpenPositionMarketValue);
        Assert.AreEqual(1009.9m, result.NetLiquidationValue);
        Assert.AreEqual(0, result.CompletedTradeCount);
        Assert.AreEqual(0, result.WinningTradeCount);
        Assert.AreEqual(0, result.LosingTradeCount);
        Assert.IsTrue(result.HasOpenPosition);
    }

    [TestMethod]
    public void Execute_ShouldReturnUnrealizedLoss_WhenPositionRemainsOpenAndLatestPriceIsLower()
    {
        // Arrange
        var asset = TestDataBuilder.CreateAsset("AAPL");
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 95m, 90m);

        var simulationRun = new SimulationRun(
            Guid.NewGuid(),
            asset,
            StrategyType.BuyAndHold,
            1000m,
            1,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, priceSeries[0].Date, TradeSide.Buy, 100m, 1)
        });

        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(899.9m, result.FinalCash);
        Assert.AreEqual(0m, result.RealizedProfitLoss);
        Assert.AreEqual(-10.1m, result.UnrealizedProfitLoss);
        Assert.AreEqual(90m, result.OpenPositionMarketValue);
        Assert.AreEqual(989.9m, result.NetLiquidationValue);
        Assert.IsTrue(result.HasOpenPosition);
    }

    [TestMethod]
    public void Execute_ShouldThrowInvalidSimulationException_WhenSellOccursBeforeBuy()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var firstDate = simulationRun.PriceSeries.First().Date;

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, firstDate, TradeSide.Sell, 120m, 1)
        });

        // Act
        void action() => _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(action);
    }

    [TestMethod]
    public void Execute_ShouldThrowInvalidSimulationException_WhenTwoBuysOccurWithoutClosingTheFirst()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var dates = simulationRun.PriceSeries.Select(pricePoint => pricePoint.Date).ToList();

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, dates[0], TradeSide.Buy, 100m, 1),
            new(asset, dates[1], TradeSide.Buy, 110m, 1)
        });

        // Act
        void action() => _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(action);
    }

    [TestMethod]
    public void Execute_ShouldThrowInvalidSimulationException_WhenSellQuantityDoesNotMatchOpenBuyQuantity()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var dates = simulationRun.PriceSeries.Select(pricePoint => pricePoint.Date).ToList();

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, dates[0], TradeSide.Buy, 100m, 2),
            new(asset, dates[1], TradeSide.Sell, 120m, 1)
        });

        // Act
        void action() => _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(action);
    }

    [TestMethod]
    public void Execute_ShouldHandleQuantitiesGreaterThanOne_ForClosedTrades()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var dates = simulationRun.PriceSeries.Select(pricePoint => pricePoint.Date).ToList();

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, dates[0], TradeSide.Buy, 100m, 2),
            new(asset, dates[1], TradeSide.Sell, 120m, 2)
        });

        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(1039.56m, result.FinalCash);
        Assert.AreEqual(39.56m, result.RealizedProfitLoss);
        Assert.AreEqual(0m, result.UnrealizedProfitLoss);
        Assert.AreEqual(0m, result.OpenPositionMarketValue);
        Assert.AreEqual(1039.56m, result.NetLiquidationValue);
        Assert.AreEqual(1, result.CompletedTradeCount);
    }

    [TestMethod]
    public void Execute_ShouldHandleQuantitiesGreaterThanOne_ForOpenPosition()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var firstDate = simulationRun.PriceSeries.First().Date;

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, firstDate, TradeSide.Buy, 100m, 2)
        });

        // Latest price is 110m.
        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(799.8m, result.FinalCash);
        Assert.AreEqual(0m, result.RealizedProfitLoss);
        Assert.AreEqual(19.8m, result.UnrealizedProfitLoss);
        Assert.AreEqual(220m, result.OpenPositionMarketValue);
        Assert.AreEqual(1019.8m, result.NetLiquidationValue);
        Assert.IsTrue(result.HasOpenPosition);
    }

    [TestMethod]
    public void Execute_ShouldPreserveTradeListInResult()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var dates = simulationRun.PriceSeries.Select(pricePoint => pricePoint.Date).ToList();

        var trades = new List<Trade>
        {
            new(asset, dates[0], TradeSide.Buy, 100m, 1),
            new(asset, dates[1], TradeSide.Sell, 120m, 1)
        };

        simulationRun.SetTrades(trades);

        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(2, result.Trades.Count);
        Assert.AreEqual(TradeSide.Buy, result.Trades.First().Side);
        Assert.AreEqual(TradeSide.Sell, result.Trades.Last().Side);
    }

    [TestMethod]
    public void Execute_ShouldEvaluateTradesByTradeDate_WhenTradesAreProvidedOutOfOrder()
    {
        // Arrange
        var simulationRun = CreateSimulationRun(1000m);
        var asset = simulationRun.Asset;
        var dates = simulationRun.PriceSeries.Select(pricePoint => pricePoint.Date).ToList();

        simulationRun.SetTrades(new List<Trade>
        {
            new(asset, dates[1], TradeSide.Sell, 120m, 1),
            new(asset, dates[0], TradeSide.Buy, 100m, 1)
        });

        // Act
        var result = _simulationEngine.Execute(simulationRun);

        // Assert
        Assert.AreEqual(1019.78m, result.FinalCash);
        Assert.AreEqual(19.78m, result.RealizedProfitLoss);
        Assert.AreEqual(1, result.CompletedTradeCount);
        Assert.AreEqual(1, result.WinningTradeCount);
        Assert.AreEqual(0, result.LosingTradeCount);
        Assert.IsFalse(result.HasOpenPosition);
        Assert.AreEqual(TradeSide.Buy, result.Trades.First().Side);
        Assert.AreEqual(TradeSide.Sell, result.Trades.Last().Side);
    }

    private static SimulationRun CreateSimulationRun(decimal initialCash)
    {
        var asset = TestDataBuilder.CreateAsset("AAPL");
        var priceSeries = TestDataBuilder.CreatePriceSeries(100m, 120m, 90m, 110m);

        return new SimulationRun(
            Guid.NewGuid(),
            asset,
            StrategyType.Greedy,
            initialCash,
            1,
            priceSeries,
            TestDataBuilder.CreateDefaultConstraints());
    }
}
