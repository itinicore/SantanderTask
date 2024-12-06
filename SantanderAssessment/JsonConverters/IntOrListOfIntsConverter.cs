using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SantanderAssessment.JsonConverters
{
    public class IntOrListOfIntsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if (token == null)
                return null;

            // Handle integer
            if (token.Type == JTokenType.Integer)
            {
                return token.ToObject<int>();
            }
            // Handle list of integers
            else if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<int>>();
            }
            // Handle map (dictionary) of string to integer
            else if (token.Type == JTokenType.Object)
            {
                return token.ToObject<Dictionary<string, int>>();
            }

            // Unsupported types
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Writing JSON is not implemented");
        }
    }
}