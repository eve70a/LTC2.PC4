using LTC2.Shared.StravaConnector.Models;

namespace LTC2.Desktopclients.WindowsClient.Models
{
    public class ActivityTypeDescription
    {
        public string Value { get; set; }

        public string Description { get; set; }

        public StravaActivityType ActivityType
        {
            get
            {
                return (StravaActivityType)Enum.Parse(typeof(StravaActivityType), Value);
            }
        }
    }
}
