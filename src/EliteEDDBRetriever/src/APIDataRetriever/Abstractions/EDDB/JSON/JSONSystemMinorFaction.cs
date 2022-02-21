namespace EliteCommodityAnalysis.Abstractions.EDDB.JSON {
    using System;

    public class JSONSystemMinorFaction {
        public uint? happiness_id {get; set;}

        public uint minor_faction_id {get; set;}

        public decimal? influence {get; set;}

        public JSONSystemMinorFactionActiveState[] active_states {get; set;}

        public JSONSystemMinorFactionPendingState[] pending_states {get; set;}

        public JSONSystemMinorFactionRecoveringState[] recovering_states {get; set;}

        public UInt64 ed_system_address {get; set; }
    }
}