using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTracker.Models.Dto
{
    public class TeamDto
    {
        public MatchTeam TeamId { get; set; }
        public bool Win { get; set; }
        public List<int> Bans { get; set; }

        public enum MatchTeam
        {
            Blue = 100,
            Red = 200,
        }
    }
}
