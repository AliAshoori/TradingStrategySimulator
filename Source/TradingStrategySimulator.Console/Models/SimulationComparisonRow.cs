using TradingStrategySimulator.Domain.Enums;

namespace TradingStrategySimulator.ConsoleApp.Models;

internal sealed class SimulationComparisonRow
{
    public required string DatasetName { get; init; }

    public required StrategyType StrategyType { get; init; }

    public required decimal InitialCash { get; init; }

    public required decimal NetLiquidationValue { get; init; }

    public required decimal RealizedProfitLoss { get; init; }

    public required decimal UnrealizedProfitLoss { get; init; }

    public required decimal TotalTransactionCost { get; init; }

    public required int CompletedTradeCount { get; init; }

    public required int WinningTradeCount { get; init; }

    public decimal ReturnPercentage =>
        InitialCash == 0m
            ? 0m
            : ((NetLiquidationValue - InitialCash) / InitialCash) * 100m;

    public decimal AverageProfitLossPerTrade =>
        CompletedTradeCount == 0
            ? 0m
            : RealizedProfitLoss / CompletedTradeCount;

    public decimal FeeImpactPercentage =>
        RealizedProfitLoss == 0m
            ? 0m
            : (TotalTransactionCost / Math.Abs(RealizedProfitLoss)) * 100m;

    public decimal WinRatePercentage =>
        CompletedTradeCount == 0
            ? 0m
            : ((decimal)WinningTradeCount / CompletedTradeCount) * 100m;
}