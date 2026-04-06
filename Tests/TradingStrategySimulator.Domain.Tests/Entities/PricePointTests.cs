using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingStrategySimulator.Domain.Entities;

namespace TradingStrategySimulator.Domain.Tests.Entities;

[TestClass]
public sealed class PricePointTests
{
    [TestMethod]
    public void Constructor_ShouldSetProperties_WhenArgumentsAreValid()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 10);
        const decimal price = 123.45m;

        // Act
        var pricePoint = new PricePoint(date, price);

        // Assert
        Assert.AreEqual(date, pricePoint.Date);
        Assert.AreEqual(price, pricePoint.Price);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenPriceIsZero()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 10);

        // Act
        void Action() => _ = new PricePoint(date, 0m);

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(Action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentOutOfRangeException_WhenPriceIsNegative()
    {
        // Arrange
        var date = new DateOnly(2026, 1, 10);

        // Act
        void Action() => _ = new PricePoint(date, -10m);

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(Action);
    }

    [TestMethod]
    public void ToString_ShouldContainDateAndPrice()
    {
        // Arrange
        var pricePoint = new PricePoint(new DateOnly(2026, 1, 10), 150.25m);

        // Act
        var result = pricePoint.ToString();

        // Assert
        StringAssert.Contains(result, "2026-01-10");
        StringAssert.Contains(result, "150.25");
    }
}