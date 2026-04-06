using TradingStrategySimulator.Application.Contracts.DTOs;
using TradingStrategySimulator.Application.Contracts.Responses;

namespace TradingStrategySimulator.ConsoleApp.Writers;

internal sealed class ConsoleResultWriter
{
    public void Write(RunSimulationResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        WriteScenarioHeader(response);
        WriteSummary(response);
        WritePriceSeries(response);
        WriteTrades(response);
        WriteSeparator();
    }

    private static void WriteScenarioHeader(RunSimulationResponse response)
    {
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine($"Simulation Run Id: {response.SimulationRunId}");
        System.Console.WriteLine($"Asset: {response.AssetSymbol}");
        System.Console.WriteLine($"Strategy: {response.StrategyType}");
        System.Console.ResetColor();
        System.Console.WriteLine();
    }

    private static void WriteSummary(RunSimulationResponse response)
    {
        System.Console.WriteLine("Summary");
        System.Console.WriteLine($"  Initial Cash           : {response.InitialCash:N2}");
        System.Console.WriteLine($"  Final Cash             : {response.FinalCash:N2}");
        System.Console.WriteLine($"  Realized PnL           : {response.RealizedProfitLoss:N2}");
        System.Console.WriteLine($"  Unrealized PnL         : {response.UnrealizedProfitLoss:N2}");
        System.Console.WriteLine($"  Open Position Value    : {response.OpenPositionMarketValue:N2}");
        System.Console.WriteLine($"  Net Liquidation Value  : {response.NetLiquidationValue:N2}");
        System.Console.WriteLine($"  Total Transaction Cost : {response.TotalTransactionCost:N4}");
        System.Console.WriteLine($"  Quantity Per Trade     : {response.QuantityPerTrade}");
        System.Console.WriteLine($"  Transaction Cost Rate  : {response.TransactionCostRate:P4}");
        System.Console.WriteLine($"  Cooldown Periods       : {response.CooldownPeriods}");
        System.Console.WriteLine($"  Completed Trades       : {response.CompletedTradeCount}");
        System.Console.WriteLine($"  Winning Trades         : {response.WinningTradeCount}");
        System.Console.WriteLine($"  Losing Trades          : {response.LosingTradeCount}");
        System.Console.WriteLine($"  Open Position          : {response.HasOpenPosition}");
        System.Console.WriteLine();
    }

    private static void WritePriceSeries(RunSimulationResponse response)
    {
        System.Console.WriteLine("Price Series");

        if (response.PriceSeries.Count == 0)
        {
            System.Console.WriteLine("  No price series data was supplied.");
            System.Console.WriteLine();
            return;
        }

        foreach (PricePointDto pricePoint in response.PriceSeries.OrderBy(pricePoint => pricePoint.Date))
        {
            System.Console.WriteLine($"  {pricePoint.Date:yyyy-MM-dd} | Price: {pricePoint.Price:N2}");
        }

        System.Console.WriteLine();
    }

    private static void WriteTrades(RunSimulationResponse response)
    {
        System.Console.WriteLine("Trades");

        if (response.Trades.Count == 0)
        {
            System.Console.WriteLine("  No trades were generated.");
            System.Console.WriteLine();
            return;
        }

        foreach (TradeDto trade in response.Trades.OrderBy(trade => trade.TradeDate))
        {
            System.Console.WriteLine(
                $"  {trade.TradeDate:yyyy-MM-dd} | {trade.Side,-4} | Qty: {trade.Quantity:N0} | Price: {trade.Price:N2} | Gross: {trade.GrossAmount:N2} | Fee: {trade.TransactionCost:N4} | Net Cash: {trade.NetCashAmount:N4}");
        }

        System.Console.WriteLine();
    }

    private static void WriteSeparator()
    {
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.WriteLine("--------------------------------------------------------------");
        System.Console.ResetColor();
        System.Console.WriteLine();
    }
}