using System.Collections.Generic;

namespace LTC2.Shared.StravaConnector.Models.Responses
{
    public class GetRoutesResponse : AbstractLimitsStravaResponse
    {
        public GetRoutesResponse() : base()
        {
        }

        public GetRoutesResponse(string limits, string usage) : base(limits, usage)
        {
        }

        public GetRoutesResponse(LimitsOnlyResponse limitsOnlyResponse): base(limitsOnlyResponse)
        {
        }

        public List<StravaRoute> Routes { get; set; } = new List<StravaRoute>();    
    }
}
