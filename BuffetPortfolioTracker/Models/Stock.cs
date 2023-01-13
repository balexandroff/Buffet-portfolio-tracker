namespace BuffetPortfolioTracker.Models
{
    public class Stock : IEquatable<Stock>
    {
        public string Name { get; set; }
        public string Ticker { get; set; }
        public string Holdings { get; set; }
        public string Stake { get; set; }
        public string MarketPrice { get; set; }
        public string Value { get; set; }
        public string PercentOfPortfolio { get; set; }
        public string Action { get; set; }

        public bool Equals(Stock? other)
        {
            return string.Compare(this.Name, other.Name, true) == 0 &&
                string.Compare(this.Ticker, other.Ticker, true) == 0 &&
                string.Compare(this.Holdings, other.Holdings, true) == 0 &&
                string.Compare(this.Stake, other.Stake, true) == 0 &&
                string.Compare(this.MarketPrice, other.MarketPrice, true) == 0 &&
                string.Compare(this.Value, other.Value, true) == 0 &&
                string.Compare(this.PercentOfPortfolio, other.PercentOfPortfolio, true) == 0;
        }
    }
}
