using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LoLTracker.Models.Dto
{
    public class BanDto
    {
        [JsonProperty("championId")]
        public int ChampionId { get; set; }

        public Guid Id { get; set; }

        [JsonProperty("pickTurn")]
        public int PickTurn { get; set; }

        public Guid TeamId { get; set; }
    }

    public class TeamDto
    {
        public enum MatchTeam
        {
            Blue = 100,
            Red = 200,
        }

        [JsonProperty("bans")]
        public List<BanDto> Bans { get; set; } = [];

        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public MatchDto? Match { get; set; }

        [JsonIgnore]
        public long MatchId { get; set; }

        [JsonProperty("teamId")]
        public MatchTeam TeamId { get; set; }

        [JsonProperty("win")]
        public bool Win { get; set; }
    }
}