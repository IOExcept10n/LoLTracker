using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace LoLTracker.Models.Dto
{
    public class MatchDto
    {
        public long GameId { get; set; }
        public string GameMode { get; set; } = string.Empty;
        public string GameResult { get; set; } = string.Empty;
        public long GameDuration { get; set; }
        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime GameStartTimestamp { get; set; }
        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime GameEndTimestamp { get; set; }
        public List<TeamDto> Teams { get; set; } = [];
        public List<ParticipantDto> Participants { get; set; } = [];
        public string GameVersion { get; set; } = string.Empty;
        private class MillisecondEpochConverter : DateTimeConverterBase
        {
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                writer.WriteRawValue(((DateTime)value! - DateTime.UnixEpoch).TotalMilliseconds.ToString());
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) { return null; }
                return DateTime.UnixEpoch.AddMilliseconds((long)reader.Value);
            }
        }
    }
}
