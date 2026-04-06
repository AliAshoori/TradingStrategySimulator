using TradingStrategySimulator.Application.Contracts.DTOs;
using TradingStrategySimulator.Application.Contracts.Requests;
using TradingStrategySimulator.ConsoleApp.Loaders;
using TradingStrategySimulator.Domain.Enums;

namespace TradingStrategySimulator.ConsoleApp.Factories;

internal static class ConsoleScenarioFactory
{
    private const string DefaultAssetSymbol = "AAPL";
    private const decimal DefaultInitialCash = 10_000m;
    private const int DefaultQuantityPerTrade = 10;
    private const decimal DefaultTransactionCostRate = 0.001m; // 0.1%
    private const int DefaultCooldownPeriods = 1;
    private const string DefaultDatasetFileName = "volatile.json";

    public static IReadOnlyCollection<RunSimulationRequest> CreateDefaultScenarios()
    {
        SampleDataset dataset = SampleDatasetLoader.Load(DefaultDatasetFileName);

        return CreateRequestsFromDataset(
            dataset,
            DefaultAssetSymbol,
            DefaultInitialCash,
            DefaultQuantityPerTrade,
            DefaultTransactionCostRate,
            DefaultCooldownPeriods);
    }

    public static ConsoleScenarioSet CreateDefaultScenarioSet()
    {
        return CreateScenarioSetForDataset(DefaultDatasetFileName);
    }

    public static IReadOnlyCollection<RunSimulationRequest> CreateScenariosForDataset(string datasetFileName)
    {
        SampleDataset dataset = SampleDatasetLoader.Load(datasetFileName);

        return CreateRequestsFromDataset(
            dataset,
            DefaultAssetSymbol,
            DefaultInitialCash,
            DefaultQuantityPerTrade,
            DefaultTransactionCostRate,
            DefaultCooldownPeriods);
    }

    public static ConsoleScenarioSet CreateScenarioSetForDataset(string datasetFileName)
    {
        SampleDataset dataset = SampleDatasetLoader.Load(datasetFileName);

        return CreateScenarioSet(dataset);
    }

    public static IReadOnlyCollection<ConsoleScenarioSet> CreateScenarioSetsForAllDatasets()
    {
        IReadOnlyCollection<SampleDataset> datasets = SampleDatasetLoader.LoadAll();

        return datasets
            .Select(CreateScenarioSet)
            .ToList();
    }

    private static ConsoleScenarioSet CreateScenarioSet(SampleDataset dataset)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        IReadOnlyCollection<RunSimulationRequest> requests = CreateRequestsFromDataset(
            dataset,
            DefaultAssetSymbol,
            DefaultInitialCash,
            DefaultQuantityPerTrade,
            DefaultTransactionCostRate,
            DefaultCooldownPeriods);

        return new ConsoleScenarioSet
        {
            DatasetName = dataset.Name,
            DatasetDescription = dataset.Description,
            DatasetFileName = dataset.SourceFileName,
            Requests = requests
        };
    }

    private static IReadOnlyCollection<RunSimulationRequest> CreateRequestsFromDataset(
        SampleDataset dataset,
        string assetSymbol,
        decimal initialCash,
        int quantityPerTrade,
        decimal transactionCostRate,
        int cooldownPeriods)
    {
        ArgumentNullException.ThrowIfNull(dataset);

        return
        [
            CreateScenario(
                assetSymbol,
                StrategyType.BuyAndHold,
                initialCash,
                quantityPerTrade,
                transactionCostRate,
                cooldownPeriods,
                dataset.PriceSeries),

            CreateScenario(
                assetSymbol,
                StrategyType.Greedy,
                initialCash,
                quantityPerTrade,
                transactionCostRate,
                cooldownPeriods,
                dataset.PriceSeries),

            CreateScenario(
                assetSymbol,
                StrategyType.PeakValley,
                initialCash,
                quantityPerTrade,
                transactionCostRate,
                cooldownPeriods,
                dataset.PriceSeries)
        ];
    }

    private static RunSimulationRequest CreateScenario(
        string assetSymbol,
        StrategyType strategyType,
        decimal initialCash,
        int quantityPerTrade,
        decimal transactionCostRate,
        int cooldownPeriods,
        IReadOnlyCollection<PricePointDto> priceSeries)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assetSymbol);
        ArgumentNullException.ThrowIfNull(priceSeries);

        return new RunSimulationRequest
        {
            AssetSymbol = assetSymbol,
            StrategyType = strategyType,
            InitialCash = initialCash,
            QuantityPerTrade = quantityPerTrade,
            TransactionCostRate = transactionCostRate,
            CooldownPeriods = cooldownPeriods,
            PriceSeries = priceSeries.ToList()
        };
    }
}