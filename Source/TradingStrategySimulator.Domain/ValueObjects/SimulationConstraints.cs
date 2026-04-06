namespace TradingStrategySimulator.Domain.ValueObjects;

/// <summary>
/// Version 2 simulation constraints that influence how trades are generated
/// and how their economics are evaluated.
///
/// Business meaning:
/// - TransactionCostRate is applied to each executed trade side as a percentage
///   of the gross trade amount.
/// - CooldownPeriods controls how many price points must pass after a SELL
///   before the strategy can open the next BUY.
/// </summary>
public sealed class SimulationConstraints
{
    public decimal TransactionCostRate { get; }

    public int CooldownPeriods { get; }

    public SimulationConstraints(decimal transactionCostRate, int cooldownPeriods)
    {
        if (transactionCostRate < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(transactionCostRate),
                "Transaction cost rate cannot be negative.");
        }

        if (transactionCostRate >= 1m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(transactionCostRate),
                "Transaction cost rate must be less than 1. Example: 0.001 = 0.1%.");
        }

        if (cooldownPeriods < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cooldownPeriods),
                "Cooldown periods cannot be negative.");
        }

        TransactionCostRate = transactionCostRate;
        CooldownPeriods = cooldownPeriods;
    }
}