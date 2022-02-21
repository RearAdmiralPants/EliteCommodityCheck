namespace EliteCommodityAnalysis.Abstractions.EDDB.JSON {
    using System;
    public class JSONPopulatedSystem {
        public uint id {get; set; }

        public uint? edsm_id {get; set;}

        public string name {get; set;}

        public decimal x {get; set;}

        public decimal y {get; set;}

        public decimal z { get; set; }

        public UInt64 population {get; set; }

        public bool is_populated {get; set; }

        public uint? government_id {get; set; }

        public string government {get; set; }

        public uint? allegiance_id {get; set; }

        public string allegiance {get; set; }

        public JSONSystemState[] states {get; set; }

        public uint? security_id {get; set;}

        public string security {get; set; }

        public uint? primary_economy_id {get; set; }

        public string primary_economy {get; set; }

        public string power {get; set; }

        public string power_state {get; set; }

        public uint? power_state_id {get; set; }

        public bool needs_permit {get; set; }

        public UInt64 updated_at {get; set; }

        public UInt64? minor_factions_updated_at {get; set;}

        public string simbad_ref {get; set;}

        public uint? controlling_minor_faction_id {get; set;}

        public string controlling_minor_faction {get; set; }

        public uint? reserve_type_id {get; set;}

        public string reserve_type {get; set;}

        public JSONSystemMinorFaction[] minor_faction_presences {get; set;}
    }
}