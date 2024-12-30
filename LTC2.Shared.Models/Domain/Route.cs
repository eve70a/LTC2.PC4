using System.Collections.Generic;

namespace LTC2.Shared.Models.Domain
{
    public class Route
    {
        public List<List<double>> Coordinates { get; set; } = new List<List<double>>();

        public List<string> Places { get; set; } = new List<string>();
    }
}
