using MediatR;
using SantanderAssessment.BackgroundServices;

namespace SantanderAssessment.Notifications
{
    public class StoriesListUpdatedNotification : INotification
    {

        public enum Operation
        {
            LoadList,
            AddElement,
            RemoveElement,
            UpdateElement
        }

        public String Path { get; set; }
        public List<int> StoryIds { get; set; }
        public int StoryIndex { get; set; }
        public Operation Op { get; set; }

        public StoriesListUpdatedNotification(ListUpdatedSSEvent sseEvent)
        {
            var path = sseEvent.Path;
            var data = sseEvent.Data;


            if (path == "/")
            {
                if (data == null)
                {
                    Op = Operation.LoadList;
                    StoryIds = new List<int>();
                    return;
                }

                if (data is IEnumerable<int> array)
                {
                    Op = Operation.LoadList;
                    StoryIds = array.ToList();
                }
                else
                {
                    Op = Operation.AddElement;
                    StoryIds = new List<int>();
                    var dictionary = data as Dictionary<string, int>;
                    StoryIds = dictionary.Values.ToList();
                }
            }
            else
            {
                var index = Int32.Parse(path.Substring(1));
                if (data == null)
                {
                    Op = Operation.RemoveElement;
                    StoryIndex = index;
                }
                else
                {
                    Op = Operation.UpdateElement;
                    var newId = (int)data;
                    StoryIndex = index;
                    StoryIds = new List<int>() { newId };

                }
            }
        }
    }
}
