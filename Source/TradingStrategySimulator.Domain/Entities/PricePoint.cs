namespace TradingStrategySimulator.Domain.Entities;

/// <summary>
/// Represents a market price observation for a given date.
/// 
/// For this simulator, a price point is the input signal that strategies
/// inspect in order to decide whether to enter or exit a position.
/// </summary>
public sealed class PricePoint
{
    public DateOnly Date { get; }
    public decimal Price { get; }

    public PricePoint(DateOnly date, decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be greater than zero.");
        }

        Date = date;
        Price = price;
    }

    public override string ToString()
    {
        return $"{Date:yyyy-MM-dd} | {Price}";
    }
}