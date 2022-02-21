namespace EliteCommodityAnalysis.Abstractions.EDDB {
    using System;

    public class SystemMinorFaction {
        public uint? HappinessId {get; set;}

        public uint MinorFactionId {get; set;}

        public decimal? Influence {get; set;}

        public SystemMinorFactionState[] States {get; set;}

        public SystemMinorFactionActiveState[] ActiveStates {get; set;}

        public SystemMinorFactionPendingState[] PendingStates {get; set;}

        public SystemMinorFactionRecoveringState[] RecoveringStates {get; set;}

        public UInt64 EdSystemAddress {get; set; }
    }
}