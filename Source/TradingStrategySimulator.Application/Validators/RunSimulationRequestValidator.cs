using TradingStrategySimulator.Application.Contracts.Interfaces;
using TradingStrategySimulator.Application.Contracts.Requests;
using TradingStrategySimulator.Domain.Exceptions;

namespace TradingStrategySimulator.Application.Validators;

public sealed class RunSimulationRequestValidator : IRunSimulationRequestValidator
{
    public void Validate(RunSimulationRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.AssetSymbol))
        {
            throw new InvalidSimulationException("Asset symbol is required.");
        }

        if (request.InitialCash < 0)
        {
            throw new InvalidSimulationException("Initial cash cannot be negative.");
        }

        if (request.QuantityPerTrade <= 0)
        {
            throw new InvalidSimulationException("Quantity per trade must be greater than zero.");
        }

        if (request.TransactionCostRate < 0m)
        {
            throw new InvalidSimulationException("Transaction cost rate cannot be negative.");
        }

        if (request.TransactionCostRate >= 1m)
        {
            throw new InvalidSimulationException("Transaction cost rate must be less than 1. Example: 0.001 = 0.1%.");
        }

        if (request.CooldownPeriods < 0)
        {
            throw new InvalidSimulationException("Cooldown periods cannot be negative.");
        }

        if (request.PriceSeries is null)
        {
            throw new InvalidSimulationException("Price series is required.");
        }

        if (request.PriceSeries.Count == 0)
        {
            throw new InvalidSimulationException("Price series must contain at least one price point.");
        }

        foreach (var pricePoint in request.PriceSeries)
        {
            if (pricePoint.Price <= 0)
            {
                throw new InvalidSimulationException(
                    $"Invalid price point detected for date {pricePoint.Date:yyyy-MM-dd}. Price must be greater than zero.");
            }
        }

        bool hasDuplicateDates = request.PriceSeries
            .GroupBy(pricePoint => pricePoint.Date)
            .Any(group => group.Count() > 1);

        if (hasDuplicateDates)
        {
            throw new InvalidSimulationException("Price series cannot contain duplicate dates.");
        }

        bool isNotSortedAscending = request.PriceSeries
            .Zip(request.PriceSeries.Skip(1), (current, next) => current.Date > next.Date)
            .Any(result => result);

        if (isNotSortedAscending)
        {
            throw new InvalidSimulationException("Price series must be ordered by date in ascending order.");
        }
    }
}