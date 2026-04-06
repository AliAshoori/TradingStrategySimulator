using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Exceptions;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Domain.Strategies;

/// <summary>
/// Greedy strategy.
/// 
/// Business concept:
/// This strategy attempts to capture every immediate upward move in the price series.
/// Instead of holding the asset for the full period, it repeatedly enters before a rise
/// and exits after that rise materialises.
/// 
/// In other words, it behaves like a participant who is less interested in long-term
/// conviction and more interested in harvesting local gains wherever they appear.
/// 
/// Example:
/// If prices move 10 -> 12 -> 11 -> 15, the strategy will try to benefit from
/// 10 -> 12 and then again from 11 -> 15, rather than sitting through the whole path
/// as a passive investor would.
/// 
/// This strategy is intentionally simple and deterministic, which makes it ideal for:
/// - interview explanation
/// - unit testing
/// - comparing active vs passive behaviour
/// </summary>
public sealed class GreedyStrategy : ITradingStrategy
{
    public StrategyType StrategyType => StrategyType.Greedy;

    public IReadOnlyCollection<Trade> GenerateTrades(
        Asset asset,
        IReadOnlyList<PricePoint> priceSeries,
        int quantityPerTrade,
        SimulationConstraints constraints)
    {
        if (asset is null)
        {
            throw new ArgumentNullException(nameof(asset));
        }

        ArgumentNullException.ThrowIfNull(constraints);

        ValidatePriceSeries(priceSeries);
        ValidateQuantity(quantityPerTrade);

        if (priceSeries.Count < 2)
        {
            return Array.Empty<Trade>();
        }

        List<Trade> trades = [];
        int nextEligibleBuyIndex = 0;

        for (int index = 1; index < priceSeries.Count; index++)
        {
            int buyIndex = index - 1;
            int sellIndex = index;

            PricePoint previous = priceSeries[buyIndex];
            PricePoint current = priceSeries[sellIndex];

            if (buyIndex < nextEligibleBuyIndex)
            {
                continue;
            }

            if (current.Price > previous.Price)
            {
                trades.Add(new Trade(asset, previous.Date, TradeSide.Buy, previous.Price, quantityPerTrade));
                trades.Add(new Trade(asset, current.Date, TradeSide.Sell, current.Price, quantityPerTrade));

                nextEligibleBuyIndex = sellIndex + constraints.CooldownPeriods + 1;
            }
        }

        return trades;
    }

    private static void ValidatePriceSeries(IReadOnlyList<PricePoint> priceSeries)
    {
        if (priceSeries is null)
        {
            throw new ArgumentNullException(nameof(priceSeries));
        }

        if (priceSeries.Count == 0)
        {
            throw new InvalidSimulationException("Price series is required.");
        }
    }

    private static void ValidateQuantity(int quantityPerTrade)
    {
        if (quantityPerTrade <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantityPerTrade), "Quantity per trade must be greater than zero.");
        }
    }
}