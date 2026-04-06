namespace TradingStrategySimulator.Application.Contracts.DTOs;

/// <summary>
/// DTO representation of a market price point.
/// 
/// This sits at the application boundary and allows the UI to submit
/// and receive price data without taking a dependency on domain entities.
/// </summary>
public sealed record PricePointDto
{
    public DateOnly Date { get; init; }

    public decimal Price { get; init; }
}