using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Newtonsoft.Json;

namespace LoLTracker.Models.Dto
{
    public enum ChampionPosition
    {
        Top,
        Middle,
        Jungle,
        Bottom,
        Utility,
    }

    public class ParticipantDto
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public long MatchId { get; set; }
        [JsonIgnore]
        public MatchDto Match { get; set; }
        public int ChampionId { get; set; }
        public string ChampionName { get; set; }
        public string Puuid { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Deaths { get; set; }
        public int GoldEarned { get; set; }
        public int DamageDealtToBuildings { get; set; }
        public int DamageDealtToChampions { get; set; }
        public string Role { get; set; }
        public int Level { get; set; }
        public int TimePlayed { get; set; }
        public int DetectorWardsPlaced { get; set; } = 0;
        public int StealthWardsPlaced { get; set; } = 0;
        public int TotalWardsPlaced { get; set; } = 0;
        public int MagicDamageDealt { get; set; } = 0;
        public int PhysicalDamageDealt { get; set; } = 0;
        public int TotalDamageDealt { get; set; } = 0;
        public int ChampExperience { get; set; } = 0;
        public int TotalDamageDealtToChampions { get; set; } = 0;
        public int VisionScore { get; set; } = 0;
        public int TotalMinionsKilled { get; set; } = 0;
        public int DamageSelfMitigated { get; set; } = 0;
        public int TotalHeal { get; set; } = 0;
        public int TurretTakedowns { get; set; } = 0;
        public int DragonKills { get; set; } = 0;
        public int BaronKills { get; set; } = 0;
        public int WardsPlaced { get; set; } = 0;
        public int WardsKilled { get; set; } = 0;
        public string RiotIdGameName { get; set; } = string.Empty;
        public string RiotIdTagLine { get; set; } = string.Empty;
        [JsonConverter(typeof(MyEnumConverter))]
        public ChampionPosition? TeamPosition { get; set; }
        public TeamDto.MatchTeam TeamId { get; set; }

        public class MyEnumConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ChampionPosition?);
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                {
                    return null;
                }

                var enumString = reader.Value?.ToString();
                if (string.IsNullOrEmpty(enumString))
                {
                    return null;
                }

                return Enum.TryParse<ChampionPosition>(enumString, true, out var result) ? result : null;
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                writer.WriteValue(value?.ToString());
            }
        }
    }
}
