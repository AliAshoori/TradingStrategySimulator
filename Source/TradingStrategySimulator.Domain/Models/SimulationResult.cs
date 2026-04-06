using System.Collections.ObjectModel;
using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Domain.Models;

/// <summary>
/// Represents the final outcome of a simulation run.
/// 
/// Important finance semantics:
/// - Realized PnL comes only from closed buy/sell pairs.
/// - Unrealized PnL represents the mark-to-market result of any open position.
/// - Final cash reflects actual cash movements from executed trades.
/// - Net liquidation value represents the portfolio value if any open position
///   were closed using the latest available market price.
/// </summary>
public sealed class SimulationResult
{
    public Asset Asset { get; }

    public StrategyType StrategyType { get; }

    public decimal InitialCash { get; }

    public decimal FinalCash { get; }

    public decimal RealizedProfitLoss { get; }

    public decimal UnrealizedProfitLoss { get; }

    public decimal OpenPositionMarketValue { get; }

    public decimal NetLiquidationValue { get; }

    public decimal TotalTransactionCost { get; }

    public int CompletedTradeCount { get; }

    public int WinningTradeCount { get; }

    public int LosingTradeCount { get; }

    public bool HasOpenPosition { get; }

    public IReadOnlyCollection<Trade> Trades { get; }

    public SimulationResult(
        Asset asset,
        StrategyType strategyType,
        decimal initialCash,
        decimal finalCash,
        decimal realizedProfitLoss,
        decimal unrealizedProfitLoss,
        decimal openPositionMarketValue,
        decimal netLiquidationValue,
        decimal totalTransactionCost,
        int completedTradeCount,
        int winningTradeCount,
        int losingTradeCount,
        bool hasOpenPosition,
        IReadOnlyCollection<Trade> trades)
    {
        Asset = asset ?? throw new ArgumentNullException(nameof(asset));
        StrategyType = strategyType;
        InitialCash = initialCash;
        FinalCash = finalCash;
        RealizedProfitLoss = realizedProfitLoss;
        UnrealizedProfitLoss = unrealizedProfitLoss;
        OpenPositionMarketValue = openPositionMarketValue;
        NetLiquidationValue = netLiquidationValue;
        TotalTransactionCost = totalTransactionCost;
        CompletedTradeCount = completedTradeCount;
        WinningTradeCount = winningTradeCount;
        LosingTradeCount = losingTradeCount;
        HasOpenPosition = hasOpenPosition;
        Trades = new ReadOnlyCollection<Trade>((trades ?? throw new ArgumentNullException(nameof(trades))).ToList());
    }
}