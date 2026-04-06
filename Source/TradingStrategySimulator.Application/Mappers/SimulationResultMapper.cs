using TradingStrategySimulator.Application.Contracts.DTOs;
using TradingStrategySimulator.Application.Contracts.Responses;
using TradingStrategySimulator.Domain.Aggregates;
using TradingStrategySimulator.Domain.Models;

namespace TradingStrategySimulator.Application.Mappers;

public static class SimulationResponseMapper
{
    public static RunSimulationResponse Map(
        SimulationRun simulationRun,
        SimulationResult simulationResult)
    {
        ArgumentNullException.ThrowIfNull(simulationRun);
        ArgumentNullException.ThrowIfNull(simulationResult);

        return new RunSimulationResponse
        {
            SimulationRunId = simulationRun.Id,
            AssetSymbol = simulationResult.Asset.Symbol,
            StrategyType = simulationResult.StrategyType,
            InitialCash = simulationResult.InitialCash,
            FinalCash = simulationResult.FinalCash,
            RealizedProfitLoss = simulationResult.RealizedProfitLoss,
            UnrealizedProfitLoss = simulationResult.UnrealizedProfitLoss,
            OpenPositionMarketValue = simulationResult.OpenPositionMarketValue,
            NetLiquidationValue = simulationResult.NetLiquidationValue,
            TotalTransactionCost = simulationResult.TotalTransactionCost,
            QuantityPerTrade = simulationRun.QuantityPerTrade,
            TransactionCostRate = simulationRun.Constraints.TransactionCostRate,
            CooldownPeriods = simulationRun.Constraints.CooldownPeriods,
            CompletedTradeCount = simulationResult.CompletedTradeCount,
            WinningTradeCount = simulationResult.WinningTradeCount,
            LosingTradeCount = simulationResult.LosingTradeCount,
            HasOpenPosition = simulationResult.HasOpenPosition,
            PriceSeries = simulationRun.PriceSeries
                .Select(pricePoint => new PricePointDto
                {
                    Date = pricePoint.Date,
                    Price = pricePoint.Price
                })
                .ToList(),
            Trades = simulationResult.Trades
                .Select(trade => new TradeDto
                {
                    AssetSymbol = trade.Asset.Symbol,
                    Side = trade.Side.ToString(),
                    TradeDate = trade.TradeDate,
                    Price = trade.Price,
                    Quantity = trade.Quantity,
                    GrossAmount = trade.GrossAmount,
                    TransactionCost = trade.TransactionCost,
                    NetCashAmount = trade.NetCashAmount
                })
                .ToList()
        };
    }
}