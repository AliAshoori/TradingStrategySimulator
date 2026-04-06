using TradingStrategySimulator.Application.Contracts.Requests;

namespace TradingStrategySimulator.ConsoleApp.Factories;

/// <summary>
/// Represents one dataset plus the simulation requests generated for it.
/// </summary>
internal sealed class ConsoleScenarioSet
{
    public required string DatasetName { get; init; }

    public required string DatasetDescription { get; init; }

    public required string DatasetFileName { get; init; }

    public required IReadOnlyCollection<RunSimulationRequest> Requests { get; init; }
}