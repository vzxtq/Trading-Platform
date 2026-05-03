namespace TradingEngine.Domain.Entities
{
    // Temporary directory of tradeable symbols until external API integration
    public class SymbolDomain
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        private SymbolDomain() { }
    }
}