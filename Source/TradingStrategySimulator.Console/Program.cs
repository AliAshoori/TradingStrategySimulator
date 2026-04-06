using Microsoft.Extensions.DependencyInjection;
using TradingStrategySimulator.Application.Contracts.Interfaces;
using TradingStrategySimulator.Application.Contracts.Responses;
using TradingStrategySimulator.ConsoleApp.Factories;
using TradingStrategySimulator.ConsoleApp.Models;
using TradingStrategySimulator.ConsoleApp.Writers;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        try
        {
            IServiceProvider serviceProvider = ServiceRegistration.RegisterServices();

            ISimulationAppService simulationAppService =
                serviceProvider.GetRequiredService<ISimulationAppService>();

            ConsoleResultWriter consoleResultWriter =
                serviceProvider.GetRequiredService<ConsoleResultWriter>();

            ConsoleComparisonWriter consoleComparisonWriter =
                serviceProvider.GetRequiredService<ConsoleComparisonWriter>();

            IReadOnlyCollection<ConsoleScenarioSet> scenarioSets =
                ConsoleScenarioFactory.CreateScenarioSetsForAllDatasets();

            List<SimulationComparisonRow> comparisonRows = [];
            List<(ConsoleScenarioSet ScenarioSet, RunSimulationResponse Response)> detailedResults = [];

            WriteHeader();

            foreach (ConsoleScenarioSet scenarioSet in scenarioSets)
            {
                foreach (var request in scenarioSet.Requests)
                {
                    RunSimulationResponse response = simulationAppService.RunSimulation(request);

                    detailedResults.Add((scenarioSet, response));

                    comparisonRows.Add(new SimulationComparisonRow
                    {
                        DatasetName = scenarioSet.DatasetName,
                        StrategyType = response.StrategyType,
                        InitialCash = response.InitialCash,
                        NetLiquidationValue = response.NetLiquidationValue,
                        RealizedProfitLoss = response.RealizedProfitLoss,
                        UnrealizedProfitLoss = response.UnrealizedProfitLoss,
                        TotalTransactionCost = response.TotalTransactionCost,
                        CompletedTradeCount = response.CompletedTradeCount,
                        WinningTradeCount = response.WinningTradeCount
                    });
                }
            }

            consoleComparisonWriter.Write(comparisonRows);

            foreach (var result in detailedResults)
            {
                WriteDatasetHeader(result.ScenarioSet);
                consoleResultWriter.Write(result.Response);
            }

            WriteFooter();
        }
        catch (Exception exception)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("An unexpected error occurred while running the simulator.");
            System.Console.WriteLine(exception.Message);
            System.Console.ResetColor();
        }
    }

    private static void WriteHeader()
    {
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("==============================================================");
        System.Console.WriteLine("                TRADING STRATEGY SIMULATOR");
        System.Console.WriteLine("==============================================================");
        System.Console.ResetColor();
        System.Console.WriteLine();
    }

    private static void WriteDatasetHeader(ConsoleScenarioSet scenarioSet)
    {
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine("==============================================================");
        System.Console.WriteLine($"Dataset: {scenarioSet.DatasetName}");
        System.Console.WriteLine($"File   : {scenarioSet.DatasetFileName}");
        System.Console.WriteLine("==============================================================");
        System.Console.ResetColor();

        System.Console.WriteLine(scenarioSet.DatasetDescription);
        System.Console.WriteLine();
    }

    private static void WriteFooter()
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.WriteLine("Simulation run completed.");
        System.Console.ResetColor();
    }
}