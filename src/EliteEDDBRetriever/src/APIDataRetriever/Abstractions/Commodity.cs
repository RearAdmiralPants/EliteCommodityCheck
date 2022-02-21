namespace EliteCommodityAnalysis.Abstractions
{
    public class Commodity
    {
        public uint Id { get; set; }

        public string Name { get; set; }

        public uint CategoryId { get; set; }

        public uint? AveragePrice { get; set; }

        public int IsRare { get; set; }

        public uint? MaxBuyPrice { get; set; }

        public uint? MaxSellPrice { get; set; }

        public uint? MinBuyPrice { get; set; }

        public uint? MinSellPrice { get; set; }

        public uint? BuyPriceLowerAverage { get; set; }

        public uint? SellPriceUpperAverage { get; set; }

        public int IsNonMarketable { get; set; }

        public uint EdId { get; set; }

        public CommodityCategory Category { get; set; }
    }
}
