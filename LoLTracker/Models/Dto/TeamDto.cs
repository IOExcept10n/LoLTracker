using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LoLTracker.Models.Dto
{
    public class TeamDto
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public long MatchId { get; set; }
        [JsonIgnore]
        public MatchDto Match { get; set; }

        [JsonProperty("bans")]
        public List<BanDto> Bans { get; set; } = [];

        [JsonProperty("teamId")]
        public MatchTeam TeamId { get; set; }

        [JsonProperty("win")]
        public bool Win { get; set; }

        public enum MatchTeam
        {
            Blue = 100,
            Red = 200,
        }
    }

    public class BanDto
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }

        [JsonProperty("championId")]
        public int ChampionId { get; set; }

        [JsonProperty("pickTurn")]
        public int PickTurn { get; set; }
    }
}
