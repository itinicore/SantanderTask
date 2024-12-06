
using System.Text.Json.Serialization;

namespace SantanderAssessment.Models
{
    public class Story
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
        public string PostedBy { get; set; }
        public string Time { get; set; }
        public int Score { get; set; }
        public int CommentCount { get; set; }

        public void ApplyFirebaseUpdate(Dictionary<string, object> data)
        {
            foreach (var pair in data)
            {
                switch (pair.Key)
                {
                    case "title":
                        Title = pair.Value.ToString();
                        break;

                    case "url":
                        Uri = pair.Value.ToString();
                        break;

                    case "by":
                        PostedBy = pair.Value.ToString();
                        break;

                    case "score":
                        Score = Int32.Parse(pair.Value.ToString());
                        break;
                    case "time":
                        var unixTime = Int32.Parse((string)pair.Value.ToString());
                        DateTime time = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
                        Time = time.ToString();
                        break;

                    case "descendants":
                        CommentCount = Int32.Parse((string)pair.Value.ToString()); ;
                        break;
                }
            }
        }
    }
}
