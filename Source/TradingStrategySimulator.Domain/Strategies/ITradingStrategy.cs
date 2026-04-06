using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Domain.Strategies;

/// <summary>
/// Contract for a trading strategy.
/// 
/// A strategy is responsible for deciding when to buy and when to sell
/// based on the supplied market price series.
/// 
/// It does not calculate the final PnL summary itself.
/// That responsibility belongs to the simulation engine.
/// </summary>
public interface ITradingStrategy
{
    StrategyType StrategyType { get; }

    IReadOnlyCollection<Trade> GenerateTrades(
       Asset asset,
       IReadOnlyList<PricePoint> priceSeries,
       int quantityPerTrade,
       SimulationConstraints constraints);
}