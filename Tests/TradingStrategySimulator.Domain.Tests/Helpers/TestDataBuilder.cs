using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Domain.Tests.Helpers;

internal static class TestDataBuilder
{
    public static SimulationConstraints CreateDefaultConstraints()
    {
        return new SimulationConstraints(0.001m, 2);
    }

    public static Asset CreateAsset(string symbol = "AAPL")
    {
        return new Asset(symbol);
    }

    public static List<PricePoint> CreatePriceSeries(params decimal[] prices)
    {
        var result = new List<PricePoint>();
        var startDate = new DateOnly(2026, 1, 1);

        for (int index = 0; index < prices.Length; index++)
        {
            result.Add(new PricePoint(startDate.AddDays(index), prices[index]));
        }

        return result;
    }
}
