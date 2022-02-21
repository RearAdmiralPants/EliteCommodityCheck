namespace EliteCommodityAnalysis.Abstractions
{
    using System;
    public class Station
    {
        public uint Id { get; set; }

        public string Name { get; set; }

        public uint SystemId { get; set; }

        public UInt64 UpdatedAt { get; set; }

        public string MaxLandingPadSize { get; set; }

        public uint? DistanceToStar { get; set; }

        public uint? GovernmentId { get; set; }

        public string Allegiance { get; set; }

        public StationState[] States { get; set; }

        public uint? TypeId { get; set; }

        public string Type { get; set; }

        public bool HasBlackmarket { get; set; }

        public bool HasMarket { get; set; }

        public bool HasRefuel { get; set; }

        public bool HasRepair { get; set; }

        public bool HasRearm { get; set; }

        public bool HasOutfitting { get; set; }

        public bool HasShipyard { get; set; }

        public bool HasDocking { get; set; }

        public bool HasCommodities { get; set; }

        public bool HasMaterialTrader { get; set; }

        public bool HasTechnologyBroker { get; set; }

        public bool HasCarrierVendor { get; set; }

        public bool HasCarrierAdministration { get; set; }

        public bool HasInterstellarFactors { get; set; }

        public bool HasUniversalCartographics { get; set; }

        public string[] ImportCommodities { get; set; }

        public string[] ExportCommodities { get; set; }

        public string[] ProhibitedCommodities { get; set; }

        public string[] Economies { get; set; }

        public UInt64? ShipyardUpdatedAt { get; set; }

        public UInt64? OutfittingUpdatedAt { get; set; }

        public UInt64? MarketUpdatedAt { get; set; }

        public bool IsPlanetary { get; set; }

        public string[] SellingShips { get; set; }

        public uint[] SellingModules { get; set; }

        public int? SettlementSizeId { get; set; }

        public string SettlementSize { get; set; }

        public int? SettlementSecurityId { get; set; }

        public string SettlementSecurity { get; set; }

        public uint? BodyId { get; set; }

        public uint? ControllingMinorFactionId { get; set; }

        public UInt64? EdMarketId { get; set; }
    }
}