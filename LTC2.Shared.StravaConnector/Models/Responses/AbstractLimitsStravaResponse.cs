namespace LTC2.Shared.StravaConnector.Models.Responses
{
    public class AbstractLimitsStravaResponse: AbstractStravaResponse
    {
        public AbstractLimitsStravaResponse() : base()
        {
        }

        public AbstractLimitsStravaResponse(string limits, string usage) : base(limits, usage)
        {
        }

        public AbstractLimitsStravaResponse(LimitsOnlyResponse limitsOnlyResponse)
        {
            if (limitsOnlyResponse.HasLimits)
            {
                HasLimits = true;

                QuarterRateLimit = limitsOnlyResponse.QuarterRateLimit;
                QuarterRateUsage = limitsOnlyResponse.QuarterRateUsage;
                DayRateLimit = limitsOnlyResponse.DayRateLimit;
                DayRateUsage = limitsOnlyResponse.DayRateUsage;
            }

            LimitsExceeded = true;
        }

        public bool LimitsExceeded { get; set; }
    }
}
