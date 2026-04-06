using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingStrategySimulator.Application.Services;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Exceptions;
using TradingStrategySimulator.Domain.Strategies;

namespace TradingStrategySimulator.Application.Tests.Services;

[TestClass]
public sealed class TradingStrategyResolverTests
{
    [TestMethod]
    public void Constructor_ShouldThrowArgumentNullException_WhenStrategiesIsNull()
    {
        // Arrange
        // Act
        void Action() => _ = new TradingStrategyResolver(null!);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(Action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentException_WhenStrategiesIsEmpty()
    {
        // Arrange
        var strategies = Enumerable.Empty<ITradingStrategy>();

        // Act
        void Action() => _ = new TradingStrategyResolver(strategies);

        // Assert
        Assert.ThrowsException<ArgumentException>(Action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentException_WhenStrategiesContainDuplicateStrategyTypes()
    {
        // Arrange
        var duplicateStrategies = new ITradingStrategy[]
        {
            new BuyAndHoldStrategy(),
            new BuyAndHoldStrategy()
        };

        // Act
        void Action() => _ = new TradingStrategyResolver(duplicateStrategies);

        // Assert
        Assert.ThrowsException<ArgumentException>(Action);
    }

    [TestMethod]
    public void Resolve_ShouldReturnBuyAndHoldStrategy_WhenRequested()
    {
        // Arrange
        var buyAndHoldStrategy = new BuyAndHoldStrategy();
        var greedyStrategy = new GreedyStrategy();
        var peakValleyStrategy = new PeakValleyStrategy();

        var resolver = new TradingStrategyResolver(new ITradingStrategy[]
        {
            buyAndHoldStrategy,
            greedyStrategy,
            peakValleyStrategy
        });

        // Act
        var result = resolver.Resolve(StrategyType.BuyAndHold);

        // Assert
        Assert.AreSame(buyAndHoldStrategy, result);
    }

    [TestMethod]
    public void Resolve_ShouldReturnGreedyStrategy_WhenRequested()
    {
        // Arrange
        var buyAndHoldStrategy = new BuyAndHoldStrategy();
        var greedyStrategy = new GreedyStrategy();
        var peakValleyStrategy = new PeakValleyStrategy();

        var resolver = new TradingStrategyResolver(new ITradingStrategy[]
        {
            buyAndHoldStrategy,
            greedyStrategy,
            peakValleyStrategy
        });

        // Act
        var result = resolver.Resolve(StrategyType.Greedy);

        // Assert
        Assert.AreSame(greedyStrategy, result);
    }

    [TestMethod]
    public void Resolve_ShouldReturnPeakValleyStrategy_WhenRequested()
    {
        // Arrange
        var buyAndHoldStrategy = new BuyAndHoldStrategy();
        var greedyStrategy = new GreedyStrategy();
        var peakValleyStrategy = new PeakValleyStrategy();

        var resolver = new TradingStrategyResolver(new ITradingStrategy[]
        {
            buyAndHoldStrategy,
            greedyStrategy,
            peakValleyStrategy
        });

        // Act
        var result = resolver.Resolve(StrategyType.PeakValley);

        // Assert
        Assert.AreSame(peakValleyStrategy, result);
    }

    [TestMethod]
    public void Resolve_ShouldThrowInvalidSimulationException_WhenStrategyTypeIsNotRegistered()
    {
        // Arrange
        var resolver = new TradingStrategyResolver(new ITradingStrategy[]
        {
            new BuyAndHoldStrategy()
        });

        // Act
        void Action() => resolver.Resolve(StrategyType.Greedy);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }
}
