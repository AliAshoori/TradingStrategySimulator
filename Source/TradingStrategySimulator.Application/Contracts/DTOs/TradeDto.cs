namespace TradingStrategySimulator.Application.Contracts.DTOs;

/// <summary>
/// DTO representation of an executed trade.
/// </summary>
public sealed record TradeDto
{
    public required string AssetSymbol { get; init; }

    public required string Side { get; init; }

    public required DateOnly TradeDate { get; init; }

    public required decimal Price { get; init; }

    public required int Quantity { get; init; }

    public required decimal GrossAmount { get; init; }

    public required decimal TransactionCost { get; init; }

    public required decimal NetCashAmount { get; init; }
}