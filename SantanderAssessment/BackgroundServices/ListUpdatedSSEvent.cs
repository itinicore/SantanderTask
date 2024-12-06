using Newtonsoft.Json;
using SantanderAssessment.JsonConverters;

namespace SantanderAssessment.BackgroundServices
{
    public class ListUpdatedSSEvent
    {
        public string Path { get; set; }
        [JsonConverter(typeof(IntOrListOfIntsConverter))]
        public object? Data { get; set; }
    }
}
