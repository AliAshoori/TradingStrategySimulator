using TradingStrategySimulator.Application.Contracts.Requests;
using TradingStrategySimulator.Application.Contracts.Responses;

namespace TradingStrategySimulator.Application.Contracts.Interfaces
{
    public interface ISimulationAppService
    {
        RunSimulationResponse RunSimulation(RunSimulationRequest request);
    }
}
