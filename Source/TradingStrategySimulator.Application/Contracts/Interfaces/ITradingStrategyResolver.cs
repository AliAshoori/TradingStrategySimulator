using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Strategies;

namespace TradingStrategySimulator.Application.Contracts.Interfaces;

/// <summary>
/// Resolves the correct domain strategy implementation for the requested strategy type.
/// </summary>
public interface ITradingStrategyResolver
{
    ITradingStrategy Resolve(StrategyType strategyType);
}