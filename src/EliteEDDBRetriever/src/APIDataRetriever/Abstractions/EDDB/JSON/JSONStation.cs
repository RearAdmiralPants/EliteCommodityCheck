namespace EliteCommodityAnalysis.Abstractions.EDDB.JSON
{
    using System;
    public class JSONStation
    {
        public uint id { get; set; }

        public string name { get; set; }

        public uint system_id { get; set; }

        public UInt64 updated_at { get; set; }

        public string max_landing_pad_size { get; set; }

        public uint? distance_to_star { get; set; }

        public uint? government_id { get; set; }

        public string allegiance { get; set; }

        public JSONStationState[] states { get; set; }

        public uint? type_id { get; set; }

        public string type { get; set; }

        public bool has_blackmarket { get; set; }

        public bool has_market { get; set; }

        public bool has_refuel { get; set; }

        public bool has_repair { get; set; }

        public bool has_rearm { get; set; }

        public bool has_outfitting { get; set; }

        public bool has_shipyard { get; set; }

        public bool has_docking { get; set; }

        public bool has_commodities { get; set; }

        public bool has_material_trader { get; set; }

        public bool has_technology_broker { get; set; }

        public bool has_carrier_vendor { get; set; }

        public bool has_carrier_administration { get; set; }

        public bool has_interstellar_factors { get; set; }

        public bool has_universal_cartographics { get; set; }

        public string[] import_commodities {get; set; }

        public string[] export_commodities {get; set; }

        public string[] prohibited_commodities {get; set; }

        public string[] economies { get; set; }

        public UInt64? shipyard_updated_at { get; set; }

        public UInt64? outfitting_updated_at { get; set; }

        public UInt64? market_updated_at { get; set; }

        public bool is_planetary { get; set; }

        public string[] selling_ships { get; set; }

        public uint[] selling_modules { get; set; }

        public int? settlement_size_id { get; set; }

        public string settlement_size { get; set; }

        public int? settlement_security_id { get; set; }

        public string settlement_security { get; set; }

        public uint? body_id { get; set; }

        public uint? controlling_minor_faction_id { get; set; }

        public UInt64? ed_market_id { get; set; }
    }
}
