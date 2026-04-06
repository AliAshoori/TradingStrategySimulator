using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TradingStrategySimulator.Application.Contracts.Interfaces;
using TradingStrategySimulator.Application.Services;
using TradingStrategySimulator.Application.Tests.Helpers;
using TradingStrategySimulator.Domain.Aggregates;
using TradingStrategySimulator.Domain.Entities;
using TradingStrategySimulator.Domain.Enums;
using TradingStrategySimulator.Domain.Models;
using TradingStrategySimulator.Domain.Services;
using TradingStrategySimulator.Domain.Strategies;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Application.Tests.Services;

[TestClass]
public sealed class SimulationAppServiceTests
{
    private Mock<ITradingStrategyResolver> _tradingStrategyResolverMock = null!;
    private Mock<IRunSimulationRequestValidator> _runSimulationRequestValidatorMock = null!;
    private Mock<ISimulationEngine> _simulationEngineMock = null!;
    private Mock<ITradingStrategy> _tradingStrategyMock = null!;

    private SimulationAppService _simulationAppService = null!;

    [TestInitialize]
    public void Initialize()
    {
        // Arrange
        _tradingStrategyResolverMock = new Mock<ITradingStrategyResolver>(MockBehavior.Strict);
        _runSimulationRequestValidatorMock = new Mock<IRunSimulationRequestValidator>(MockBehavior.Strict);
        _simulationEngineMock = new Mock<ISimulationEngine>(MockBehavior.Strict);
        _tradingStrategyMock = new Mock<ITradingStrategy>(MockBehavior.Strict);

        _simulationAppService = new SimulationAppService(
            _tradingStrategyResolverMock.Object,
            _runSimulationRequestValidatorMock.Object,
            _simulationEngineMock.Object);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentNullException_WhenTradingStrategyResolverIsNull()
    {
        // Arrange
        IRunSimulationRequestValidator validator = new Mock<IRunSimulationRequestValidator>().Object;
        ISimulationEngine simulationEngine = new Mock<ISimulationEngine>().Object;

        // Act
        void Action() => _ = new SimulationAppService(
            null!,
            validator,
            simulationEngine);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(Action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentNullException_WhenRunSimulationRequestValidatorIsNull()
    {
        // Arrange
        ITradingStrategyResolver tradingStrategyResolver = new Mock<ITradingStrategyResolver>().Object;
        ISimulationEngine simulationEngine = new Mock<ISimulationEngine>().Object;

        // Act
        void Action() => _ = new SimulationAppService(
            tradingStrategyResolver,
            null!,
            simulationEngine);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(Action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentNullException_WhenSimulationEngineIsNull()
    {
        // Arrange
        ITradingStrategyResolver tradingStrategyResolver = new Mock<ITradingStrategyResolver>().Object;
        IRunSimulationRequestValidator validator = new Mock<IRunSimulationRequestValidator>().Object;

        // Act
        void Action() => _ = new SimulationAppService(
            tradingStrategyResolver,
            validator,
            null!);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(Action);
    }

    [TestMethod]
    public void RunSimulation_ShouldCallValidatorOnce()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest(StrategyType.BuyAndHold);
        Asset asset = new(request.AssetSymbol);

        List<Trade> trades = CreateClosedTrades(asset);

        SimulationResult simulationResult = CreateClosedSimulationResult(
            asset,
            request.StrategyType,
            request.InitialCash,
            trades);

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request));

        _tradingStrategyResolverMock
            .Setup(resolver => resolver.Resolve(request.StrategyType))
            .Returns(_tradingStrategyMock.Object);

        _tradingStrategyMock
            .Setup(strategy => strategy.GenerateTrades(
                It.IsAny<Asset>(),
                It.IsAny<IReadOnlyList<PricePoint>>(),
                request.QuantityPerTrade,
                It.IsAny<SimulationConstraints>()))
            .Returns(trades);

        _simulationEngineMock
            .Setup(engine => engine.Execute(It.IsAny<SimulationRun>()))
            .Returns(simulationResult);

        // Act
        _simulationAppService.RunSimulation(request);

        // Assert
        _runSimulationRequestValidatorMock.Verify(
            validator => validator.Validate(request),
            Times.Once);
    }

    [TestMethod]
    public void RunSimulation_ShouldResolveRequestedStrategyOnce()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest(StrategyType.Greedy);
        Asset asset = new(request.AssetSymbol);

        List<Trade> trades = CreateClosedTrades(asset);

        SimulationResult simulationResult = CreateClosedSimulationResult(
            asset,
            request.StrategyType,
            request.InitialCash,
            trades);

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request));

        _tradingStrategyResolverMock
            .Setup(resolver => resolver.Resolve(StrategyType.Greedy))
            .Returns(_tradingStrategyMock.Object);

        _tradingStrategyMock
            .Setup(strategy => strategy.GenerateTrades(
                It.IsAny<Asset>(),
                It.IsAny<IReadOnlyList<PricePoint>>(),
                request.QuantityPerTrade,
                It.IsAny<SimulationConstraints>()))
            .Returns(trades);

        _simulationEngineMock
            .Setup(engine => engine.Execute(It.IsAny<SimulationRun>()))
            .Returns(simulationResult);

        // Act
        _simulationAppService.RunSimulation(request);

        // Assert
        _tradingStrategyResolverMock.Verify(
            resolver => resolver.Resolve(StrategyType.Greedy),
            Times.Once);
    }

    [TestMethod]
    public void RunSimulation_ShouldCallGenerateTrades_WithMappedDomainArguments()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest(StrategyType.PeakValley);
        Asset asset = new(request.AssetSymbol);

        List<Trade> trades = CreateClosedTrades(asset);

        SimulationResult simulationResult = CreateClosedSimulationResult(
            asset,
            request.StrategyType,
            request.InitialCash,
            trades);

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request));

        _tradingStrategyResolverMock
            .Setup(resolver => resolver.Resolve(request.StrategyType))
            .Returns(_tradingStrategyMock.Object);

        _tradingStrategyMock
            .Setup(strategy => strategy.GenerateTrades(
                It.Is<Asset>(mappedAsset => mappedAsset.Symbol == request.AssetSymbol),
                It.Is<IReadOnlyList<PricePoint>>(priceSeries =>
                    priceSeries.Count == request.PriceSeries.Count &&
                    priceSeries[0].Date == request.PriceSeries.First().Date &&
                    priceSeries[0].Price == request.PriceSeries.First().Price),
                request.QuantityPerTrade,
                It.Is<SimulationConstraints>(constraints =>
                    constraints.TransactionCostRate == request.TransactionCostRate &&
                    constraints.CooldownPeriods == request.CooldownPeriods)))
            .Returns(trades);

        _simulationEngineMock
            .Setup(engine => engine.Execute(It.IsAny<SimulationRun>()))
            .Returns(simulationResult);

        // Act
        _simulationAppService.RunSimulation(request);

        // Assert
        _tradingStrategyMock.Verify(
            strategy => strategy.GenerateTrades(
                It.Is<Asset>(mappedAsset => mappedAsset.Symbol == request.AssetSymbol),
                It.Is<IReadOnlyList<PricePoint>>(priceSeries =>
                    priceSeries.Count == request.PriceSeries.Count),
                request.QuantityPerTrade,
                It.Is<SimulationConstraints>(constraints =>
                    constraints.TransactionCostRate == request.TransactionCostRate &&
                    constraints.CooldownPeriods == request.CooldownPeriods)),
            Times.Once);
    }

    [TestMethod]
    public void RunSimulation_ShouldCallSimulationEngineOnce()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest();
        Asset asset = new(request.AssetSymbol);

        List<Trade> trades = CreateClosedTrades(asset);

        SimulationResult simulationResult = CreateClosedSimulationResult(
            asset,
            request.StrategyType,
            request.InitialCash,
            trades);

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request));

        _tradingStrategyResolverMock
            .Setup(resolver => resolver.Resolve(request.StrategyType))
            .Returns(_tradingStrategyMock.Object);

        _tradingStrategyMock
            .Setup(strategy => strategy.GenerateTrades(
                It.IsAny<Asset>(),
                It.IsAny<IReadOnlyList<PricePoint>>(),
                request.QuantityPerTrade,
                It.IsAny<SimulationConstraints>()))
            .Returns(trades);

        _simulationEngineMock
            .Setup(engine => engine.Execute(It.IsAny<SimulationRun>()))
            .Returns(simulationResult);

        // Act
        _simulationAppService.RunSimulation(request);

        // Assert
        _simulationEngineMock.Verify(
            engine => engine.Execute(It.IsAny<SimulationRun>()),
            Times.Once);
    }

    [TestMethod]
    public void RunSimulation_ShouldReturnMappedResponse_WhenExecutionSucceedsForClosedTrade()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest(StrategyType.BuyAndHold);
        Asset asset = new(request.AssetSymbol);

        List<Trade> trades =
        [
            new Trade(asset, new DateOnly(2026, 1, 1), TradeSide.Buy, 100m, 2),
            new Trade(asset, new DateOnly(2026, 1, 3), TradeSide.Sell, 120m, 2)
        ];

        SimulationResult simulationResult = new(
            asset,
            request.StrategyType,
            request.InitialCash,
            10_040m,
            40m,
            0m,
            0m,
            10_040m,
            0.8m,
            1,
            1,
            0,
            false,
            trades);

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request));

        _tradingStrategyResolverMock
            .Setup(resolver => resolver.Resolve(request.StrategyType))
            .Returns(_tradingStrategyMock.Object);

        _tradingStrategyMock
            .Setup(strategy => strategy.GenerateTrades(
                It.IsAny<Asset>(),
                It.IsAny<IReadOnlyList<PricePoint>>(),
                request.QuantityPerTrade,
                It.IsAny<SimulationConstraints>()))
            .Returns(trades);

        _simulationEngineMock
            .Setup(engine => engine.Execute(It.IsAny<SimulationRun>()))
            .Returns(simulationResult);

        // Act
        var response = _simulationAppService.RunSimulation(request);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(request.AssetSymbol, response.AssetSymbol);
        Assert.AreEqual(request.StrategyType, response.StrategyType);
        Assert.AreEqual(request.InitialCash, response.InitialCash);
        Assert.AreEqual(10_040m, response.FinalCash);
        Assert.AreEqual(40m, response.RealizedProfitLoss);
        Assert.AreEqual(0m, response.UnrealizedProfitLoss);
        Assert.AreEqual(0m, response.OpenPositionMarketValue);
        Assert.AreEqual(10_040m, response.NetLiquidationValue);
        Assert.AreEqual(request.QuantityPerTrade, response.QuantityPerTrade);
        Assert.AreEqual(1, response.CompletedTradeCount);
        Assert.AreEqual(1, response.WinningTradeCount);
        Assert.AreEqual(0, response.LosingTradeCount);
        Assert.IsFalse(response.HasOpenPosition);
        Assert.AreEqual(2, response.Trades.Count);
        Assert.AreEqual(3, response.PriceSeries.Count);
        Assert.AreEqual("Buy", response.Trades.First().Side);
        Assert.AreEqual("Sell", response.Trades.Last().Side);
    }

    [TestMethod]
    public void RunSimulation_ShouldReturnMappedResponse_WhenExecutionSucceedsForOpenPosition()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest(StrategyType.BuyAndHold);
        Asset asset = new(request.AssetSymbol);

        List<Trade> trades =
        [
            new Trade(asset, new DateOnly(2026, 1, 1), TradeSide.Buy, 100m, 2)
        ];

        SimulationResult simulationResult = new(
            asset,
            request.StrategyType,
            request.InitialCash,
            9_800m,
            0m,
            40m,
            240m,
            10_040m,
            0.2m,
            0,
            0,
            0,
            true,
            trades);

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request));

        _tradingStrategyResolverMock
            .Setup(resolver => resolver.Resolve(request.StrategyType))
            .Returns(_tradingStrategyMock.Object);

        _tradingStrategyMock
            .Setup(strategy => strategy.GenerateTrades(
                It.IsAny<Asset>(),
                It.IsAny<IReadOnlyList<PricePoint>>(),
                request.QuantityPerTrade,
                It.IsAny<SimulationConstraints>()))
            .Returns(trades);

        _simulationEngineMock
            .Setup(engine => engine.Execute(It.IsAny<SimulationRun>()))
            .Returns(simulationResult);

        // Act
        var response = _simulationAppService.RunSimulation(request);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(9_800m, response.FinalCash);
        Assert.AreEqual(0m, response.RealizedProfitLoss);
        Assert.AreEqual(40m, response.UnrealizedProfitLoss);
        Assert.AreEqual(240m, response.OpenPositionMarketValue);
        Assert.AreEqual(10_040m, response.NetLiquidationValue);
        Assert.IsTrue(response.HasOpenPosition);
        Assert.AreEqual(0, response.CompletedTradeCount);
        Assert.AreEqual(1, response.Trades.Count);
    }

    [TestMethod]
    public void RunSimulation_ShouldPropagateException_WhenValidatorThrows()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest();

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request))
            .Throws(new InvalidOperationException("Validation failed."));

        // Act
        void Action() => _simulationAppService.RunSimulation(request);

        // Assert
        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(Action);
        Assert.AreEqual("Validation failed.", exception.Message);
    }

    [TestMethod]
    public void RunSimulation_ShouldNotResolveStrategy_WhenValidatorThrows()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest();

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request))
            .Throws(new InvalidOperationException("Validation failed."));

        // Act
        void Action() => _simulationAppService.RunSimulation(request);

        // Assert
        Assert.ThrowsException<InvalidOperationException>(Action);
        _tradingStrategyResolverMock.Verify(
            resolver => resolver.Resolve(It.IsAny<StrategyType>()),
            Times.Never);
        _simulationEngineMock.Verify(
            engine => engine.Execute(It.IsAny<SimulationRun>()),
            Times.Never);
    }

    [TestMethod]
    public void RunSimulation_ShouldPropagateException_WhenResolverThrows()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest();

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request));

        _tradingStrategyResolverMock
            .Setup(resolver => resolver.Resolve(request.StrategyType))
            .Throws(new InvalidOperationException("Resolver failed."));

        // Act
        void Action() => _simulationAppService.RunSimulation(request);

        // Assert
        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(Action);
        Assert.AreEqual("Resolver failed.", exception.Message);
    }

    [TestMethod]
    public void RunSimulation_ShouldPropagateException_WhenStrategyThrows()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest();

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request));

        _tradingStrategyResolverMock
            .Setup(resolver => resolver.Resolve(request.StrategyType))
            .Returns(_tradingStrategyMock.Object);

        _tradingStrategyMock
            .Setup(strategy => strategy.GenerateTrades(
                It.IsAny<Asset>(),
                It.IsAny<IReadOnlyList<PricePoint>>(),
                request.QuantityPerTrade,
                It.IsAny<SimulationConstraints>()))
            .Throws(new InvalidOperationException("Strategy failed."));

        // Act
        void Action() => _simulationAppService.RunSimulation(request);

        // Assert
        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(Action);
        Assert.AreEqual("Strategy failed.", exception.Message);
    }

    [TestMethod]
    public void RunSimulation_ShouldPropagateException_WhenSimulationEngineThrows()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest();
        Asset asset = new(request.AssetSymbol);

        List<Trade> trades = CreateClosedTrades(asset);

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request));

        _tradingStrategyResolverMock
            .Setup(resolver => resolver.Resolve(request.StrategyType))
            .Returns(_tradingStrategyMock.Object);

        _tradingStrategyMock
            .Setup(strategy => strategy.GenerateTrades(
                It.IsAny<Asset>(),
                It.IsAny<IReadOnlyList<PricePoint>>(),
                request.QuantityPerTrade,
                It.IsAny<SimulationConstraints>()))
            .Returns(trades);

        _simulationEngineMock
            .Setup(engine => engine.Execute(It.IsAny<SimulationRun>()))
            .Throws(new InvalidOperationException("Engine failed."));

        // Act
        void Action() => _simulationAppService.RunSimulation(request);

        // Assert
        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(Action);
        Assert.AreEqual("Engine failed.", exception.Message);
    }

    [TestMethod]
    public void RunSimulation_ShouldPassSimulationRun_WithGeneratedTradesToEngine()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest();
        Asset asset = new(request.AssetSymbol);

        List<Trade> trades =
        [
            new Trade(asset, new DateOnly(2026, 1, 1), TradeSide.Buy, 100m, 2),
            new Trade(asset, new DateOnly(2026, 1, 3), TradeSide.Sell, 120m, 2)
        ];

        SimulationResult simulationResult = CreateClosedSimulationResult(
            asset,
            request.StrategyType,
            request.InitialCash,
            trades);

        _runSimulationRequestValidatorMock
            .Setup(validator => validator.Validate(request));

        _tradingStrategyResolverMock
            .Setup(resolver => resolver.Resolve(request.StrategyType))
            .Returns(_tradingStrategyMock.Object);

        _tradingStrategyMock
            .Setup(strategy => strategy.GenerateTrades(
                It.IsAny<Asset>(),
                It.IsAny<IReadOnlyList<PricePoint>>(),
                request.QuantityPerTrade,
                It.IsAny<SimulationConstraints>()))
            .Returns(trades);

        _simulationEngineMock
            .Setup(engine => engine.Execute(It.Is<SimulationRun>(simulationRun =>
                simulationRun.Asset.Symbol == request.AssetSymbol &&
                simulationRun.StrategyType == request.StrategyType &&
                simulationRun.InitialCash == request.InitialCash &&
                simulationRun.QuantityPerTrade == request.QuantityPerTrade &&
                simulationRun.Trades.Count == 2)))
            .Returns(simulationResult);

        // Act
        _simulationAppService.RunSimulation(request);

        // Assert
        _simulationEngineMock.Verify(
            engine => engine.Execute(It.Is<SimulationRun>(simulationRun =>
                simulationRun.Asset.Symbol == request.AssetSymbol &&
                simulationRun.Trades.Count == 2)),
            Times.Once);
    }

    private static List<Trade> CreateClosedTrades(Asset asset)
    {
        return
        [
            new Trade(asset, new DateOnly(2026, 1, 1), TradeSide.Buy, 100m, 2),
            new Trade(asset, new DateOnly(2026, 1, 3), TradeSide.Sell, 120m, 2)
        ];
    }

    private static SimulationResult CreateClosedSimulationResult(
        Asset asset,
        StrategyType strategyType,
        decimal initialCash,
        IReadOnlyCollection<Trade> trades)
    {
        return new SimulationResult(
            asset,
            strategyType,
            initialCash,
            initialCash + 40m,
            40m,
            0m,
            0m,
            initialCash + 40m,
            0.8m,
            1,
            1,
            0,
            false,
            trades);
    }
}
