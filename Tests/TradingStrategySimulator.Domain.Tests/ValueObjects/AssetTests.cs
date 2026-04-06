using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingStrategySimulator.Domain.ValueObjects;

namespace TradingStrategySimulator.Domain.Tests.ValueObjects;

[TestClass]
public sealed class AssetTests
{
    [TestMethod]
    public void Constructor_ShouldSetUppercaseTrimmedSymbol_WhenSymbolIsValid()
    {
        // Arrange
        const string input = "  aapl  ";

        // Act
        var asset = new Asset(input);

        // Assert
        Assert.AreEqual("AAPL", asset.Symbol);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentException_WhenSymbolIsNull()
    {
        // Arrange
        string? symbol = null;

        // Act
        void Action() => _ = new Asset(symbol!);

        // Assert
        Assert.ThrowsException<ArgumentException>(Action);
    }

    [TestMethod]
    public void Constructor_ShouldThrowArgumentException_WhenSymbolIsWhitespace()
    {
        // Arrange
        const string symbol = "   ";

        // Act
        void Action() => _ = new Asset(symbol);

        // Assert
        Assert.ThrowsException<ArgumentException>(Action);
    }

    [TestMethod]
    public void Equals_ShouldReturnTrue_WhenSymbolsMatchIgnoringInputFormatting()
    {
        // Arrange
        var first = new Asset(" msft ");
        var second = new Asset("MSFT");

        // Act
        var result = first.Equals(second);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_ShouldReturnFalse_WhenSymbolsDiffer()
    {
        // Arrange
        var first = new Asset("MSFT");
        var second = new Asset("AAPL");

        // Act
        var result = first.Equals(second);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetHashCode_ShouldBeSame_WhenSymbolsAreEquivalent()
    {
        // Arrange
        var first = new Asset(" tsla ");
        var second = new Asset("TSLA");

        // Act
        var firstHashCode = first.GetHashCode();
        var secondHashCode = second.GetHashCode();

        // Assert
        Assert.AreEqual(firstHashCode, secondHashCode);
    }

    [TestMethod]
    public void ToString_ShouldReturnSymbol()
    {
        // Arrange
        var asset = new Asset("nvda");

        // Act
        var result = asset.ToString();

        // Assert
        Assert.AreEqual("NVDA", result);
    }
}