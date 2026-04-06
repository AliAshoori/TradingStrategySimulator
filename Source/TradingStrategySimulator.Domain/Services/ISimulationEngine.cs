using TradingStrategySimulator.Domain.Aggregates;
using TradingStrategySimulator.Domain.Models;

namespace TradingStrategySimulator.Domain.Services;

/// <summary>
/// Executes a simulation run and returns the calculated result.
/// </summary>
public interface ISimulationEngine
{
    SimulationResult Execute(SimulationRun simulationRun);
}