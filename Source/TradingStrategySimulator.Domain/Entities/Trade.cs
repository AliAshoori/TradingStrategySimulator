using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Domain.Entities;

/// <summary>
/// Represents a single executed trade in the simulation.
///
/// In Version 2, each trade can also carry its transaction cost so the
/// simulator can display fee-aware execution details in the output.
/// </summary>
public sealed class Trade
{
    public Asset Asset { get; }

    public DateOnly TradeDate { get; }

    public TradeSide Side { get; }

    public decimal Price { get; }

    public int Quantity { get; }

    public decimal TransactionCost { get; }

    public Trade(
        Asset asset,
        DateOnly tradeDate,
        TradeSide side,
        decimal price,
        int quantity,
        decimal transactionCost = 0m)
    {
        Asset = asset ?? throw new ArgumentNullException(nameof(asset));

        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Trade price must be greater than zero.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Trade quantity must be greater than zero.");
        }

        if (transactionCost < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(transactionCost), "Transaction cost cannot be negative.");
        }

        TradeDate = tradeDate;
        Side = side;
        Price = price;
        Quantity = quantity;
        TransactionCost = transactionCost;
    }

    public decimal GrossAmount => Price * Quantity;

    public decimal NetCashAmount =>
        Side == TradeSide.Buy
            ? GrossAmount + TransactionCost
            : GrossAmount - TransactionCost;

    public Trade WithTransactionCost(decimal transactionCost)
    {
        return new Trade(Asset, TradeDate, Side, Price, Quantity, transactionCost);
    }

    public override string ToString()
    {
        return $"{TradeDate:yyyy-MM-dd} | {Side} | {Asset.Symbol} | Qty={Quantity} | Px={Price} | Fee={TransactionCost}";
    }
}