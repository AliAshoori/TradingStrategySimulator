using TradingStrategySimulator.Application.Contracts.Interfaces;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Exceptions;
using TradingStrategySimulator.Domain.Strategies;

namespace TradingStrategySimulator.Application.Services;

/// <summary>
/// Resolves a concrete strategy from the registered domain strategies.
/// 
/// This keeps the application service independent from switch statements
/// and makes the solution easier to extend when new strategies are added.
/// </summary>
public sealed class TradingStrategyResolver : ITradingStrategyResolver
{
    private readonly IReadOnlyDictionary<StrategyType, ITradingStrategy> _strategies;

    public TradingStrategyResolver(IEnumerable<ITradingStrategy> strategies)
    {
        if (strategies is null)
        {
            throw new ArgumentNullException(nameof(strategies));
        }

        var strategyList = strategies.ToList();

        if (strategyList.Count == 0)
        {
            throw new ArgumentException("At least one trading strategy must be registered.", nameof(strategies));
        }

        _strategies = strategyList.ToDictionary(
            strategy => strategy.StrategyType,
            strategy => strategy);
    }

    public ITradingStrategy Resolve(StrategyType strategyType)
    {
        if (_strategies.TryGetValue(strategyType, out var strategy))
        {
            return strategy;
        }

        throw new InvalidSimulationException($"No strategy has been registered for strategy type '{strategyType}'.");
    }
}