using Microsoft.Extensions.DependencyInjection;
using TradingStrategySimulator.Application.Contracts.Interfaces;
using TradingStrategySimulator.Application.Services;
using TradingStrategySimulator.Application.Validators;
using TradingStrategySimulator.Domain.Services;
using TradingStrategySimulator.Domain.Strategies;

namespace TradingStrategySimulator.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTradingStrategySimulatorApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IRunSimulationRequestValidator, RunSimulationRequestValidator>();
        services.AddSingleton<ISimulationEngine, SimulationEngine>();

        services.AddSingleton<ITradingStrategy, BuyAndHoldStrategy>();
        services.AddSingleton<ITradingStrategy, GreedyStrategy>();
        services.AddSingleton<ITradingStrategy, PeakValleyStrategy>();

        services.AddSingleton<ITradingStrategyResolver, TradingStrategyResolver>();
        services.AddSingleton<ISimulationAppService, SimulationAppService>();

        return services;
    }
}