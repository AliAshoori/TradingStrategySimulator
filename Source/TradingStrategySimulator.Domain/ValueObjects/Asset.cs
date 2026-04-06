namespace TradingStrategySimulator.Domain.ValueObjects;

/// <summary>
/// Represents the tradable asset being simulated.
/// 
/// In Version 1, the simulator works with a single asset only.
/// This value object keeps that concept explicit and validated.
/// </summary>
public sealed class Asset : IEquatable<Asset>
{
    public string Symbol { get; }

    public Asset(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Asset symbol is required.", nameof(symbol));
        }

        Symbol = symbol.Trim().ToUpperInvariant();
    }

    public bool Equals(Asset? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(Symbol, other.Symbol, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return obj is Asset other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Symbol.GetHashCode(StringComparison.Ordinal);
    }

    public override string ToString()
    {
        return Symbol;
    }
}