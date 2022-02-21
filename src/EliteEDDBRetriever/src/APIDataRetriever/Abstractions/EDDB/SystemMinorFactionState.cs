namespace EliteCommodityAnalysis.Abstractions.EDDB {

    public class SystemMinorFactionState {
        public uint Id {get; set;}

        public string Name {get; set;}

        private string _strStateType;
        public MinorFactionType StateType {get {
            return (MinorFactionType)System.Enum.Parse(typeof(MinorFactionType), this._strStateType);
        }
        set {
            this._strStateType = value.ToString();
        }}

        public string StrStateType {get {
            return this._strStateType;
        }
        set {
            if (this._strStateType == null) {
                this.StateType = MinorFactionType.None;
            }
            else {
                this.StateType = (MinorFactionType)System.Enum.Parse(typeof(MinorFactionType), this._strStateType);
            }
        }}
    }
}