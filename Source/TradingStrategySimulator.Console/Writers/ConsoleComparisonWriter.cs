using TradingStrategySimulator.ConsoleApp.Models;

namespace TradingStrategySimulator.ConsoleApp.Writers;

internal sealed class ConsoleComparisonWriter
{
    public void Write(IReadOnlyCollection<SimulationComparisonRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        if (rows.Count == 0)
        {
            System.Console.WriteLine("No comparison rows were available.");
            System.Console.WriteLine();
            return;
        }

        List<SimulationComparisonRow> orderedRows = rows
            .OrderBy(row => row.DatasetName)
            .ThenBy(row => row.StrategyType.ToString())
            .ToList();

        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("================================================================================================================================================");
        System.Console.WriteLine("                                                        STRATEGY COMPARISON");
        System.Console.WriteLine("================================================================================================================================================");
        System.Console.ResetColor();

        System.Console.WriteLine(
            $"{PadRight("Dataset", 22)}" +
            $"{PadRight("Strategy", 15)}" +
            $"{PadLeft("Return %", 12)}" +
            $"{PadLeft("Net Value", 14)}" +
            $"{PadLeft("Realized", 14)}" +
            $"{PadLeft("Unrealized", 14)}" +
            $"{PadLeft("Avg PnL/Trade", 16)}" +
            $"{PadLeft("Fees", 12)}" +
            $"{PadLeft("Fee Impact %", 14)}" +
            $"{PadLeft("Win Rate %", 12)}" +
            $"{PadLeft("Trades", 10)}");

        System.Console.WriteLine(new string('-', 155));

        foreach (SimulationComparisonRow row in orderedRows)
        {
            System.Console.WriteLine(
                $"{PadRight(row.DatasetName, 22)}" +
                $"{PadRight(row.StrategyType.ToString(), 15)}" +
                $"{PadLeft(row.ReturnPercentage.ToString("N2"), 12)}" +
                $"{PadLeft(row.NetLiquidationValue.ToString("N2"), 14)}" +
                $"{PadLeft(row.RealizedProfitLoss.ToString("N2"), 14)}" +
                $"{PadLeft(row.UnrealizedProfitLoss.ToString("N2"), 14)}" +
                $"{PadLeft(row.AverageProfitLossPerTrade.ToString("N2"), 16)}" +
                $"{PadLeft(row.TotalTransactionCost.ToString("N4"), 12)}" +
                $"{PadLeft(row.FeeImpactPercentage.ToString("N2"), 14)}" +
                $"{PadLeft(row.WinRatePercentage.ToString("N2"), 12)}" +
                $"{PadLeft(row.CompletedTradeCount.ToString(), 10)}");
        }

        System.Console.WriteLine();
    }

    private static string PadRight(string value, int totalWidth)
    {
        return value.Length >= totalWidth
            ? value[..(totalWidth - 1)] + " "
            : value.PadRight(totalWidth);
    }

    private static string PadLeft(string value, int totalWidth)
    {
        return value.Length >= totalWidth
            ? value[..(totalWidth - 1)] + " "
            : value.PadLeft(totalWidth);
    }
}