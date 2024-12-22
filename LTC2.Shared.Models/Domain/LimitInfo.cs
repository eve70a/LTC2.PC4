namespace LTC2.Shared.Models.Domain
{
    public class LimitInfo
    {
        public bool HasLimits { get; set; } = false;

        public int QuarterRateLimit { get; set; } = int.MaxValue;

        public int QuarterRateUsage { get; set; } = int.MaxValue;

        public int DayRateLimit { get; set; } = int.MaxValue;

        public int DayRateUsage { get; set; } = int.MaxValue;

        public bool LimitExceeded { get; set; } = false;
    }
}
