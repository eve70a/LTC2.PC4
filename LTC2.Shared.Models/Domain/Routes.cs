

using System.Collections.Generic;

namespace LTC2.Shared.Models.Domain
{
    public class Routes
    {
        public bool IsStravaRoute { get; set; }

        public long StravaRouteId { get; set; }

        public LimitInfo LimitInfo { get; set; }

        public List<Route> RouteCollection { get; set; } = new List<Route>();
    }
}
