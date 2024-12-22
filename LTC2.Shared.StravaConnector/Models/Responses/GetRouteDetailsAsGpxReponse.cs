namespace LTC2.Shared.StravaConnector.Models.Responses
{
    public class GetRouteDetailsAsGpxReponse: AbstractLimitsStravaResponse
    {
        public GetRouteDetailsAsGpxReponse() : base()
        {
        }

        public GetRouteDetailsAsGpxReponse(string limits, string usage) : base(limits, usage)
        {
        }

        public GetRouteDetailsAsGpxReponse(LimitsOnlyResponse limitsOnlyResponse): base(limitsOnlyResponse)
        {
        }

        public string Gpx { get; set; }
    }
}
