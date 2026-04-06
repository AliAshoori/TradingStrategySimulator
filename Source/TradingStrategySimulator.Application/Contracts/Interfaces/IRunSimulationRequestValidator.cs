using TradingStrategySimulator.Application.Contracts.Requests;

namespace TradingStrategySimulator.Application.Contracts.Interfaces;

/// <summary>
/// Validates incoming application requests before domain orchestration begins.
/// </summary>
public interface IRunSimulationRequestValidator
{
    void Validate(RunSimulationRequest request);
}