using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SantanderAssessment.JsonConverters
{
    public class StringOrDictionaryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if (token.Type == JTokenType.String || token.Type == JTokenType.Integer)
            {
                return token.ToObject<string>();
            }
            else if (token.Type == JTokenType.Object)
            {
                return token.ToObject<Dictionary<string, object>>();
            }
            else
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Writing JSON is not implemented");
        }
    }
}
