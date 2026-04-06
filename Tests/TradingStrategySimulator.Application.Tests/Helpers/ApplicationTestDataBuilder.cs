using TradingStrategySimulator.Application.Contracts.DTOs;
using TradingStrategySimulator.Application.Contracts.Requests;
using TradingStrategySimulator.Domain.Aggregates;
using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Models;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Application.Tests.Helpers;

internal static class ApplicationTestDataBuilder
{
    public static SimulationConstraints CreateDefaultConstraints()
    {
        return new SimulationConstraints(0.0025m, 2);
    }

    public static RunSimulationRequest CreateValidRequest(
        StrategyType strategyType = StrategyType.BuyAndHold)
    {
        return new RunSimulationRequest
        {
            AssetSymbol = "AAPL",
            StrategyType = strategyType,
            InitialCash = 10_000m,
            QuantityPerTrade = 2,
            TransactionCostRate = 0.0025m,
            CooldownPeriods = 2,
            PriceSeries = new List<PricePointDto>
            {
                new() { Date = new DateOnly(2026, 1, 1), Price = 100m },
                new() { Date = new DateOnly(2026, 1, 2), Price = 110m },
                new() { Date = new DateOnly(2026, 1, 3), Price = 120m }
            }
        };
    }

    public static Asset CreateAsset(string symbol = "AAPL")
    {
        return new Asset(symbol);
    }

    public static List<PricePoint> CreateDomainPriceSeries(params decimal[] prices)
    {
        var result = new List<PricePoint>();
        var startDate = new DateOnly(2026, 1, 1);

        for (int index = 0; index < prices.Length; index++)
        {
            result.Add(new PricePoint(startDate.AddDays(index), prices[index]));
        }

        return result;
    }

    public static SimulationRun CreateSimulationRun(
        Asset? asset = null,
        StrategyType strategyType = StrategyType.BuyAndHold,
        decimal initialCash = 10_000m,
        int quantityPerTrade = 2,
        IEnumerable<PricePoint>? priceSeries = null,
        SimulationConstraints? constraints = null)
    {
        asset ??= CreateAsset();
        priceSeries ??= CreateDomainPriceSeries(100m, 110m, 120m);
        constraints ??= CreateDefaultConstraints();

        return new SimulationRun(
            Guid.NewGuid(),
            asset,
            strategyType,
            initialCash,
            quantityPerTrade,
            priceSeries,
            constraints);
    }

    public static SimulationResult CreateSimulationResult(
        Asset? asset = null,
        StrategyType strategyType = StrategyType.BuyAndHold,
        decimal initialCash = 10_000m,
        decimal finalCash = 10_040m,
        decimal realizedProfitLoss = 40m,
        decimal unrealizedProfitLoss = 0m,
        decimal openPositionMarketValue = 0m,
        decimal netLiquidationValue = 10_040m,
        int completedTradeCount = 1,
        int winningTradeCount = 1,
        int losingTradeCount = 0,
        bool hasOpenPosition = false,
        IReadOnlyCollection<Trade>? trades = null)
    {
        asset ??= CreateAsset();
        trades ??= new List<Trade>
        {
            new(asset, new DateOnly(2026, 1, 1), TradeSide.Buy, 100m, 2),
            new(asset, new DateOnly(2026, 1, 3), TradeSide.Sell, 120m, 2)
        };

        return new SimulationResult(
            asset,
            strategyType,
            initialCash,
            finalCash,
            realizedProfitLoss,
            unrealizedProfitLoss,
            openPositionMarketValue,
            netLiquidationValue,
            0.5m,
            completedTradeCount,
            winningTradeCount,
            losingTradeCount,
            hasOpenPosition,
            trades);
    }
}
