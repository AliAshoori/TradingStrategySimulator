using Microsoft.Extensions.DependencyInjection;
using TradingStrategySimulator.Application.DependencyInjection;
using TradingStrategySimulator.ConsoleApp.Writers;

internal static class ServiceRegistration
{
    internal static IServiceProvider RegisterServices()
    {
        ServiceCollection services = new();

        services.AddTradingStrategySimulatorApplication();
        services.AddSingleton<ConsoleResultWriter>();
        services.AddSingleton<ConsoleComparisonWriter>();

        return services.BuildServiceProvider();
    }
}