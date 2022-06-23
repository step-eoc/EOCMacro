namespace EOCMacro
{
    public class MacroModel
    {
        public int Acceleration { get; set; }
        public string CreationTime { get; set; }
        public bool DoNotShowWindowOnFinish { get; set; }
        public List<MacroEvent> Events { get; set; }
        public int LoopDuration { get; set; }
        public int LoopInterval { get; set; }
        public int LoopIterations { get; set; }
        public string LoopType { get; set; }
        public int MacroSchemaVersion { get; set; }
        public List<Mconf> MergeConfigurations { get; set; }
        public bool RestartPlayer { get; set; }
        public int RestartPlayerAfterMinutes { get; set; }
        public string Shortcut { get; set; }

        public MacroModel()
        {
            Acceleration = 1;
            CreationTime = "20220514T215858";
            DoNotShowWindowOnFinish = false;
            Events = new List<MacroEvent>();
            LoopDuration = 0;
            LoopInterval = 0;
            LoopIterations = 1;
            LoopType = "TillLoopNumber";
            MacroSchemaVersion = 3;
            MergeConfigurations = new List<Mconf>();
            RestartPlayer = false;
            RestartPlayerAfterMinutes = 60;
            Shortcut = "";
        }

    }

    public class MacroEvent
    {
        public int? Delta { get; set; }
        public string EventType { get; set; }
        public string? KeyName { get; set; }
        public int Timestamp { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }

        public MacroEvent(string eventType, int timestamp, string? keyname = null, int? delta = null, double? x = null, double? y= null)
        {
            Delta = delta;
            EventType = eventType;
            KeyName = keyname;
            Timestamp = timestamp;
            X = x;
            Y = y;
        }
    }

    public class Mconf
    {
        public string mergeConfiguration { get; set; }
    }
}
