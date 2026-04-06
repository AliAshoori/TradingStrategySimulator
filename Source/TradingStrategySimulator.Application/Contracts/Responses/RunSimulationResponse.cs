using TradingStrategySimulator.Application.Contracts.DTOs;
using TradingStrategySimulator.Domain.Enums;

namespace TradingStrategySimulator.Application.Contracts.Responses;

public sealed record RunSimulationResponse
{
    public required Guid SimulationRunId { get; init; }

    public required string AssetSymbol { get; init; }

    public required StrategyType StrategyType { get; init; }

    public required decimal InitialCash { get; init; }

    public required decimal FinalCash { get; init; }

    public required decimal RealizedProfitLoss { get; init; }

    public required decimal UnrealizedProfitLoss { get; init; }

    public required decimal OpenPositionMarketValue { get; init; }

    public required decimal NetLiquidationValue { get; init; }

    public required decimal TotalTransactionCost { get; init; }

    public required int QuantityPerTrade { get; init; }

    public required decimal TransactionCostRate { get; init; }

    public required int CooldownPeriods { get; init; }

    public required int CompletedTradeCount { get; init; }

    public required int WinningTradeCount { get; init; }

    public required int LosingTradeCount { get; init; }

    public required bool HasOpenPosition { get; init; }

    public required IReadOnlyCollection<PricePointDto> PriceSeries { get; init; }

    public required IReadOnlyCollection<TradeDto> Trades { get; init; }
}