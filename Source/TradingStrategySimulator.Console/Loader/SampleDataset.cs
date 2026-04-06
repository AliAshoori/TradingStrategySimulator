using TradingStrategySimulator.Application.Contracts.DTOs;

namespace TradingStrategySimulator.ConsoleApp.Loaders;

internal sealed class SampleDataset
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public List<PricePointDto> PriceSeries { get; init; } = [];

    public required string SourceFileName { get; init; }
}