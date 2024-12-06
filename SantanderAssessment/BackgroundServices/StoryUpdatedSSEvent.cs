using Newtonsoft.Json;
using SantanderAssessment.JsonConverters;

namespace SantanderAssessment.BackgroundServices
{
    public class StoryUpdatedSSEvent
    {
        public string Path { get; set; }

        [JsonConverter(typeof(StringOrDictionaryConverter))]
        public object? Data { get; set; }
    }
}
