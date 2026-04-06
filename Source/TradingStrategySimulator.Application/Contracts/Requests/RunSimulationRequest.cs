using TradingStrategySimulator.Application.Contracts.DTOs;
using TradingStrategySimulator.Domain.Enums;

namespace TradingStrategySimulator.Application.Contracts.Requests;

public sealed record RunSimulationRequest
{
    public required string AssetSymbol { get; init; }

    public required StrategyType StrategyType { get; init; }

    public required decimal InitialCash { get; init; }

    public required int QuantityPerTrade { get; init; }

    public required decimal TransactionCostRate { get; init; }

    public required int CooldownPeriods { get; init; }

    public required IReadOnlyCollection<PricePointDto> PriceSeries { get; init; }
}