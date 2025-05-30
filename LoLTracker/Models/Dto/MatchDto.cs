using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LoLTracker.Models.Dto
{
    public class MatchDto
    {
        public long GameDuration { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime GameEndTimestamp { get; set; }

        public long GameId { get; set; }
        public string GameMode { get; set; } = string.Empty;
        public string GameResult { get; set; } = string.Empty;

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime GameStartTimestamp { get; set; }

        public string GameVersion { get; set; } = string.Empty;
        public List<ParticipantDto> Participants { get; set; } = [];
        public List<TeamDto> Teams { get; set; } = [];

        private class MillisecondEpochConverter : DateTimeConverterBase
        {
            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) { return null; }
                return DateTime.UnixEpoch.AddMilliseconds((long)reader.Value);
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                writer.WriteRawValue(((DateTime)value! - DateTime.UnixEpoch).TotalMilliseconds.ToString());
            }
        }
    }
}