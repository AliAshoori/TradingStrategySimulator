using TradingStrategySimulator.Application.Contracts.Interfaces;
using TradingStrategySimulator.Application.Contracts.Requests;
using TradingStrategySimulator.Application.Contracts.Responses;
using TradingStrategySimulator.Application.Mappers;
using TradingStrategySimulator.Domain.Aggregates;
using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Services;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Application.Services;

public sealed class SimulationAppService : ISimulationAppService
{
    private readonly ITradingStrategyResolver _tradingStrategyResolver;
    private readonly IRunSimulationRequestValidator _runSimulationRequestValidator;
    private readonly ISimulationEngine _simulationEngine;

    public SimulationAppService(
        ITradingStrategyResolver tradingStrategyResolver,
        IRunSimulationRequestValidator runSimulationRequestValidator,
        ISimulationEngine simulationEngine)
    {
        _tradingStrategyResolver = tradingStrategyResolver ?? throw new ArgumentNullException(nameof(tradingStrategyResolver));
        _runSimulationRequestValidator = runSimulationRequestValidator ?? throw new ArgumentNullException(nameof(runSimulationRequestValidator));
        _simulationEngine = simulationEngine ?? throw new ArgumentNullException(nameof(simulationEngine));
    }

    public RunSimulationResponse RunSimulation(RunSimulationRequest request)
    {
        _runSimulationRequestValidator.Validate(request);

        Asset asset = new(request.AssetSymbol);

        List<PricePoint> priceSeries = request.PriceSeries
            .Select(pricePoint => new PricePoint(pricePoint.Date, pricePoint.Price))
            .ToList();

        SimulationConstraints constraints = new(
            request.TransactionCostRate,
            request.CooldownPeriods);

        SimulationRun simulationRun = new(
            Guid.NewGuid(),
            asset,
            request.StrategyType,
            request.InitialCash,
            request.QuantityPerTrade,
            priceSeries,
            constraints);

        var strategy = _tradingStrategyResolver.Resolve(request.StrategyType);

        var trades = strategy.GenerateTrades(
            asset,
            priceSeries,
            request.QuantityPerTrade,
            constraints);

        simulationRun.SetTrades(trades);

        var simulationResult = _simulationEngine.Execute(simulationRun);

        simulationRun.Complete(simulationResult);

        return SimulationResponseMapper.Map(simulationRun, simulationResult);
    }
}