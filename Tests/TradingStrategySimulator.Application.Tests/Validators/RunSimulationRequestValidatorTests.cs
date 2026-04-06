using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingStrategySimulator.Application.Contracts.DTOs;
using TradingStrategySimulator.Application.Tests.Helpers;
using TradingStrategySimulator.Application.Validators;
using TradingStrategySimulator.Domain.Exceptions;

namespace TradingStrategySimulator.Application.Tests.Validators;

[TestClass]
public sealed class RunSimulationRequestValidatorTests
{
    private readonly RunSimulationRequestValidator _validator = new();

    [TestMethod]
    public void Validate_ShouldNotThrow_WhenRequestIsValid()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest();

        // Act
        _validator.Validate(request);

        // Assert
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Validate_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        // Act
        void Action() => _validator.Validate(null!);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenAssetSymbolIsEmpty()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            AssetSymbol = string.Empty
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenAssetSymbolIsWhitespace()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            AssetSymbol = "   "
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenInitialCashIsNegative()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            InitialCash = -1m
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenQuantityPerTradeIsZero()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            QuantityPerTrade = 0
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenQuantityPerTradeIsNegative()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            QuantityPerTrade = -5
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldNotThrow_WhenTransactionCostRateIsZero()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            TransactionCostRate = 0m
        };

        // Act
        _validator.Validate(request);

        // Assert
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Validate_ShouldNotThrow_WhenTransactionCostRateIsJustBelowOne()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            TransactionCostRate = 0.9999m
        };

        // Act
        _validator.Validate(request);

        // Assert
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenTransactionCostRateIsNegative()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            TransactionCostRate = -0.001m
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenTransactionCostRateIsOne()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            TransactionCostRate = 1m
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenCooldownPeriodsIsNegative()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            CooldownPeriods = -1
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenPriceSeriesIsNull()
    {
        // Arrange
        var request = new Contracts.Requests.RunSimulationRequest
        {
            AssetSymbol = "AAPL",
            StrategyType = Domain.Enums.StrategyType.BuyAndHold,
            InitialCash = 10_000m,
            QuantityPerTrade = 1,
            TransactionCostRate = 0.003m,
            CooldownPeriods = 2,
            PriceSeries = null!
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenPriceSeriesIsEmpty()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            PriceSeries = new List<PricePointDto>()
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenAnyPriceIsZero()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            PriceSeries = new List<PricePointDto>
            {
                new() { Date = new DateOnly(2026, 1, 1), Price = 100m },
                new() { Date = new DateOnly(2026, 1, 2), Price = 0m }
            }
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenAnyPriceIsNegative()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            PriceSeries = new List<PricePointDto>
            {
                new() { Date = new DateOnly(2026, 1, 1), Price = 100m },
                new() { Date = new DateOnly(2026, 1, 2), Price = -10m }
            }
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenPriceSeriesContainsDuplicateDates()
    {
        // Arrange
        var duplicateDate = new DateOnly(2026, 1, 1);

        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            PriceSeries = new List<PricePointDto>
            {
                new() { Date = duplicateDate, Price = 100m },
                new() { Date = duplicateDate, Price = 110m }
            }
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }

    [TestMethod]
    public void Validate_ShouldThrowInvalidSimulationException_WhenPriceSeriesIsNotSortedAscending()
    {
        // Arrange
        var request = ApplicationTestDataBuilder.CreateValidRequest() with
        {
            PriceSeries = new List<PricePointDto>
            {
                new() { Date = new DateOnly(2026, 1, 2), Price = 110m },
                new() { Date = new DateOnly(2026, 1, 1), Price = 100m }
            }
        };

        // Act
        void Action() => _validator.Validate(request);

        // Assert
        Assert.ThrowsException<InvalidSimulationException>(Action);
    }
}
