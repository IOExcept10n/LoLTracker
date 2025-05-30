using System;
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
        public int Assists { get; set; }

        public int BaronKills { get; set; } = 0;

        public int ChampExperience { get; set; } = 0;

        public int ChampionId { get; set; }

        public string ChampionName { get; set; } = string.Empty;

        public int DamageDealtToBuildings { get; set; }

        public int DamageDealtToChampions { get; set; }

        public int DamageSelfMitigated { get; set; } = 0;

        public int Deaths { get; set; }

        public int DetectorWardsPlaced { get; set; } = 0;

        public int DragonKills { get; set; } = 0;

        public int GoldEarned { get; set; }

        [JsonIgnore]
        public Guid Id { get; set; }

        public int Kills { get; set; }

        public int Level { get; set; }

        public int MagicDamageDealt { get; set; } = 0;

        [JsonIgnore]
        public MatchDto? Match { get; set; }

        [JsonIgnore]
        public long MatchId { get; set; }

        public int PhysicalDamageDealt { get; set; } = 0;
        public string Puuid { get; set; } = string.Empty;
        public string RiotIdGameName { get; set; } = string.Empty;
        public string RiotIdTagLine { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int StealthWardsPlaced { get; set; } = 0;
        public TeamDto.MatchTeam TeamId { get; set; }

        [JsonConverter(typeof(MyEnumConverter))]
        public ChampionPosition? TeamPosition { get; set; }

        public int TimePlayed { get; set; }
        public int TotalDamageDealt { get; set; } = 0;
        public int TotalDamageDealtToChampions { get; set; } = 0;
        public int TotalHeal { get; set; } = 0;
        public int TotalMinionsKilled { get; set; } = 0;
        public int TotalWardsPlaced { get; set; } = 0;
        public int TurretTakedowns { get; set; } = 0;
        public int VisionScore { get; set; } = 0;
        public int WardsKilled { get; set; } = 0;
        public int WardsPlaced { get; set; } = 0;

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