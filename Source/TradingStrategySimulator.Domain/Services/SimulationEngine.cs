using TradingStrategySimulator.Domain.Aggregates;
using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Exceptions;
using TradingStrategySimulator.Domain.Models;

namespace TradingStrategySimulator.Domain.Services;

/// <summary>
/// Evaluates the financial outcome of a simulation run.
/// 
/// Responsibilities:
/// - apply executed trades to cash
/// - calculate realized PnL from closed positions
/// - calculate unrealized PnL for any remaining open position
/// - calculate net liquidation value using the latest available market price
/// </summary>
public sealed class SimulationEngine : ISimulationEngine
{
    public SimulationResult Execute(SimulationRun simulationRun)
    {
        if (simulationRun is null)
        {
            throw new ArgumentNullException(nameof(simulationRun));
        }

        if (simulationRun.PriceSeries.Count == 0)
        {
            throw new InvalidSimulationException("Simulation run must contain at least one price point.");
        }

        decimal cash = simulationRun.InitialCash;
        decimal realizedProfitLoss = 0m;
        decimal totalTransactionCost = 0m;

        bool hasOpenPosition = false;
        Trade? openBuyTrade = null;

        int completedTradeCount = 0;
        int winningTradeCount = 0;
        int losingTradeCount = 0;

        List<Trade> evaluatedTrades = [];

        foreach (Trade trade in simulationRun.Trades.OrderBy(trade => trade.TradeDate))
        {
            decimal transactionCost = CalculateTransactionCost(
                trade.GrossAmount,
                simulationRun.Constraints.TransactionCostRate);

            Trade evaluatedTrade = trade.WithTransactionCost(transactionCost);
            evaluatedTrades.Add(evaluatedTrade);

            totalTransactionCost += transactionCost;

            switch (evaluatedTrade.Side)
            {
                case TradeSide.Buy:
                    if (hasOpenPosition)
                    {
                        throw new InvalidSimulationException("Cannot execute a buy trade while another position is already open.");
                    }

                    cash -= evaluatedTrade.NetCashAmount;
                    hasOpenPosition = true;
                    openBuyTrade = evaluatedTrade;
                    break;

                case TradeSide.Sell:
                    if (!hasOpenPosition || openBuyTrade is null)
                    {
                        throw new InvalidSimulationException("Cannot execute a sell trade without an open buy position.");
                    }

                    if (evaluatedTrade.Quantity != openBuyTrade.Quantity)
                    {
                        throw new InvalidSimulationException("Sell quantity must match the currently open buy quantity in Version 1.");
                    }

                    cash += evaluatedTrade.NetCashAmount;
                    completedTradeCount++;

                    decimal tradeProfitLoss = evaluatedTrade.NetCashAmount - openBuyTrade.NetCashAmount;
                    realizedProfitLoss += tradeProfitLoss;

                    if (tradeProfitLoss > 0)
                    {
                        winningTradeCount++;
                    }
                    else if (tradeProfitLoss < 0)
                    {
                        losingTradeCount++;
                    }

                    hasOpenPosition = false;
                    openBuyTrade = null;
                    break;

                default:
                    throw new InvalidSimulationException($"Unsupported trade side '{evaluatedTrade.Side}'.");
            }
        }

        decimal unrealizedProfitLoss = 0m;
        decimal openPositionMarketValue = 0m;

        if (hasOpenPosition && openBuyTrade is not null)
        {
            PricePoint latestPricePoint = simulationRun.PriceSeries.MaxBy(pricePoint => pricePoint.Date)
                ?? throw new InvalidSimulationException("Latest market price could not be determined.");

            openPositionMarketValue = latestPricePoint.Price * openBuyTrade.Quantity;
            unrealizedProfitLoss = openPositionMarketValue - openBuyTrade.NetCashAmount;
        }

        decimal netLiquidationValue = cash + openPositionMarketValue;

        return new SimulationResult(
            simulationRun.Asset,
            simulationRun.StrategyType,
            simulationRun.InitialCash,
            cash,
            realizedProfitLoss,
            unrealizedProfitLoss,
            openPositionMarketValue,
            netLiquidationValue,
            totalTransactionCost,
            completedTradeCount,
            winningTradeCount,
            losingTradeCount,
            hasOpenPosition,
            evaluatedTrades);
    }

    private static decimal CalculateTransactionCost(decimal grossAmount, decimal transactionCostRate)
    {
        return decimal.Round(grossAmount * transactionCostRate, 4, MidpointRounding.AwayFromZero);
    }
}