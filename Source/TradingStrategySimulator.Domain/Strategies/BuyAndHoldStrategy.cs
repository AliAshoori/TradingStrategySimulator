using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Exceptions;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Domain.Strategies;

/// <summary>
/// Buy-and-hold strategy.
/// 
/// Business concept:
/// This is the simplest long-only investment behaviour.
/// The participant buys the asset at the beginning of the observation period
/// and keeps the position open until the very end, regardless of any short-term
/// fluctuations in between.
/// 
/// In practice, this represents a passive view:
/// "I believe the asset will appreciate over the whole period, so I will not
/// attempt to time intermediate highs and lows."
/// 
/// This strategy is useful in the simulator because it acts as a baseline.
/// More active strategies should be compared against it to determine whether
/// extra trading activity actually produced better results.
/// </summary>
public sealed class BuyAndHoldStrategy : ITradingStrategy
{
    public StrategyType StrategyType => StrategyType.BuyAndHold;

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

        PricePoint firstPoint = priceSeries[0];
        PricePoint lastPoint = priceSeries[^1];

        if (firstPoint.Date == lastPoint.Date)
        {
            return Array.Empty<Trade>();
        }

        List<Trade> trades =
        [
            new Trade(asset, firstPoint.Date, TradeSide.Buy, firstPoint.Price, quantityPerTrade),
            new Trade(asset, lastPoint.Date, TradeSide.Sell, lastPoint.Price, quantityPerTrade)
        ];

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