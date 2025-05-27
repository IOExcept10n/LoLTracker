using System.Collections.Generic;

namespace LoLTracker.Services
{
    public class TeamStats
    {
        public double TotalEfficiency { get; set; }

        public List<PlayerStats> Players { get; set; }
    }
}