namespace EliteCommodityAnalysis.Abstractions.EDDB {
    using System;
    public class PopulatedSystem {
        public uint Id {get; set; }

        public uint? EdsmId {get; set;}

        public string Name {get; set;}

        public decimal X {get; set;}

        public decimal Y {get; set;}

        public decimal Z { get; set; }

        public UInt64 Population {get; set; }

        public bool IsPopulated {get; set; }

        public uint? GovernmentId {get; set; }

        public string Government {get; set; }

        public uint? AllegianceId {get; set; }

        public string Allegiance {get; set; }

        public SystemState[] States {get; set; }

        public uint? SecurityId {get; set;}

        public string Security {get; set; }

        public uint? PrimaryEconomyId {get; set; }

        public string PrimaryEconomy {get; set; }

        public string Power {get; set; }

        public string PowerState {get; set; }

        public uint? PowerStateId {get; set; }

        public bool NeedsPermit {get; set; }

        public UInt64 UpdatedAt {get; set; }

        public UInt64? MinorFactionsUpdatedAt {get; set;}

        public string SimbadRef {get; set;}

        public uint? ControllingMinorFactionId {get; set;}

        public string ControllingMinorFaction {get; set; }

        public uint? ReserveTypeId {get; set;}

        public string ReserveType {get; set;}

        public SystemMinorFaction[] MinorFactionPresences {get; set;}
    }
}
