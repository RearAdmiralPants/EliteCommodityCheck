namespace EliteCommodityAnalysis.Abstractions.EDDB.JSON {

    public class JSONCommodity {
        public uint id { get; set; }

        public string name { get; set; }

        public uint category_id { get; set; }

        public uint? average_price { get; set; }

        public int is_rare { get; set; }

        public uint? max_buy_price { get; set; }

        public uint? max_sell_price { get; set; }

        public uint? min_buy_price { get; set; }

        public uint? min_sell_price { get; set; }

        public uint buy_price_lower_average {get; set; }

        public uint sell_price_upper_average {get; set; }

        public int is_non_marketable { get; set; }

        public uint ed_id { get; set; }

        public JSONCommodityCategory category { get; set; }
    }
}