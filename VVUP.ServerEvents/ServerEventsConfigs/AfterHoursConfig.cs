using System.ComponentModel;

namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class AfterHoursConfig
    {
        public string StartEventCassieMessage { get; set; } = "After Hours";
        public string StartEventCassieText { get; set; } = "After Hours. (After Hours event started)";
        public int TeslaActivationChance { get; set; } = 25;
        [Description("Every so often in seconds, how often should the chance be updated")]
        public float TeslaActivationChanceCycle { get; set; } = 5;
        public int IntercomTime { get; set; } = 5;
        [Description("What item does the players start with?")]
        public List<ItemType> StartingItems { get; set; } = new()
        {
            ItemType.Lantern
        };
    }
}