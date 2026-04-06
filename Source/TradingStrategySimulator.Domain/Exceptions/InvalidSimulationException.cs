namespace TradingStrategySimulator.Domain.Exceptions
{

    /// <summary>
    /// Thrown when a simulation request or a simulation state is invalid
    /// from a business perspective.
    /// </summary>
    public sealed class InvalidSimulationException : Exception
    {
        public InvalidSimulationException(string message)
            : base(message)
        {
        }

        public InvalidSimulationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}