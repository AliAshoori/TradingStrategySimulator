using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Exceptions;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Domain.Strategies;

/// <summary>
/// Peak-valley strategy.
/// 
/// Business concept:
/// This strategy tries to identify clearer swing moves rather than reacting
/// to every small day-to-day increase.
/// 
/// The idea is to buy near a local low point (a valley) and sell near a local
/// high point (a peak). Compared with the greedy strategy, this produces fewer,
/// more intentional trade pairs and better reflects the thinking of someone who
/// wants to ride a trend rather than scalp every short move.
/// 
/// In practical terms:
/// - a valley is where the market stops falling and begins rising
/// - a peak is where the market stops rising and begins falling
/// 
/// This is still a simplified model, but conceptually it is closer to how many
/// human traders think about swing trading:
/// enter after weakness, exit after strength.
/// </summary>
public sealed class PeakValleyStrategy : ITradingStrategy
{
    public StrategyType StrategyType => StrategyType.PeakValley;

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
        int index = 0;
        int nextEligibleBuyIndex = 0;

        while (index < priceSeries.Count - 1)
        {
            if (index < nextEligibleBuyIndex)
            {
                index = nextEligibleBuyIndex;
                continue;
            }

            while (index < priceSeries.Count - 1 &&
                   priceSeries[index + 1].Price <= priceSeries[index].Price)
            {
                index++;
            }

            if (index >= priceSeries.Count - 1)
            {
                break;
            }

            if (index < nextEligibleBuyIndex)
            {
                continue;
            }

            PricePoint valley = priceSeries[index];
            trades.Add(new Trade(asset, valley.Date, TradeSide.Buy, valley.Price, quantityPerTrade));

            while (index < priceSeries.Count - 1 &&
                   priceSeries[index + 1].Price >= priceSeries[index].Price)
            {
                index++;
            }

            PricePoint peak = priceSeries[index];
            trades.Add(new Trade(asset, peak.Date, TradeSide.Sell, peak.Price, quantityPerTrade));

            nextEligibleBuyIndex = index + constraints.CooldownPeriods + 1;
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