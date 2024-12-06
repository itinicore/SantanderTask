using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SantanderAssessment.Notifications;

namespace SantanderAssessment.BackgroundServices
{
    public class HackerNewsObserver : IHackerNewsObserver
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HackerNewsObserver> _logger;

        public HackerNewsObserver(IMediator mediator, ILogger<HackerNewsObserver> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task ObserveNewsList(CancellationToken stoppingToken)
        {
            //string firebaseUrl = "https://santander-c9419-default-rtdb.europe-west1.firebasedatabase.app/beststories.json";
            string firebaseUrl = "https://hacker-news.firebaseio.com/v0/beststories.json";

            using (var httpClient = new HttpClient())
            {
                // Ustaw nagłówek akceptacji dla SSE
                httpClient.DefaultRequestHeaders.Add("Accept", "text/event-stream");

                using (var response = await httpClient.GetAsync(firebaseUrl, HttpCompletionOption.ResponseHeadersRead, stoppingToken))
                {
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync(stoppingToken))
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        StringBuilder eventData = new StringBuilder();

                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            stoppingToken.ThrowIfCancellationRequested();

                            if (string.IsNullOrWhiteSpace(line))
                            {
                                // Pusta linia oznacza koniec wydarzenia w SSE
                                if (eventData.Length > 0)
                                {
                                    var settings = new JsonSerializerSettings
                                    {
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    };


                                    var sanitizedEventData = eventData.ToString().Replace("\r", "").Replace("\n", "");

                                    if (sanitizedEventData != "null")
                                    {
                                        _logger.LogInformation(
                                            $"Received information about list: {sanitizedEventData}");
                                        var sseEvent =
                                            JsonConvert.DeserializeObject<ListUpdatedSSEvent>(sanitizedEventData,
                                                settings);

                                        if (sseEvent == null) continue;




                                        // Publish the notification
                                        await _mediator.Publish(
                                            new StoriesListUpdatedNotification(sseEvent), stoppingToken);
                                    }
                                    
                                    eventData.Clear();
                                }
                            }
                            else if (line.StartsWith("data: "))
                            {
                                // Dodaj linie do danych wydarzenia
                                eventData.AppendLine(line.Substring(6)); // Usuwa prefix "data: "
                            }
                        }
                    }
                }
            }
        }

        public async Task ObserveSingleStory(int storyId, CancellationToken stoppingToken)
        {
            //string storyUrl = $"https://santander-c9419-default-rtdb.europe-west1.firebasedatabase.app/item/{storyId}.json";
            string storyUrl = $"https://hacker-news.firebaseio.com/v0/item/{storyId}.json";

            using (var httpClient = new HttpClient())
            {
                // Ustaw nagłówek akceptacji dla SSE
                httpClient.DefaultRequestHeaders.Add("Accept", "text/event-stream");

                using (var response = await httpClient.GetAsync(storyUrl, HttpCompletionOption.ResponseHeadersRead, stoppingToken))
                {
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync(stoppingToken))
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        StringBuilder eventData = new StringBuilder();

                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            stoppingToken.ThrowIfCancellationRequested();

                            if (string.IsNullOrWhiteSpace(line))
                            {
                                if (eventData.Length > 0)
                                {
                                    Console.WriteLine($"Story {storyId} Update:");
                                    Console.WriteLine(eventData.ToString());

                                    var sanitizedEventData = eventData.ToString().Replace("\r", "").Replace("\n", "");
                                    var settings = new JsonSerializerSettings
                                    {
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    };

                                    var sseEvent =
                                        JsonConvert.DeserializeObject<StoryUpdatedSSEvent>(sanitizedEventData,
                                            settings);


                                    if (sseEvent?.Data != null)
                                    {
                                        Dictionary<string, object> dataDictionary;
                                        if (sseEvent.Data is Dictionary<string, object> dictionary)
                                        {
                                            dataDictionary = dictionary;
                                        }
                                        else
                                        {
                                            // Create a dictionary with `path` as the key and `data` as the value
                                            dataDictionary = new Dictionary<string, object>
                                            {
                                                { sseEvent.Path.Substring(1), sseEvent.Data }
                                            };
                                        }

                                        // Publish the notification
                                        await _mediator.Publish(
                                            new StoryUpdatedNotification(storyId, dataDictionary), stoppingToken);
                                    }

                                    eventData.Clear();
                                }
                            }
                            else if (line.StartsWith("data: "))
                            {
                                eventData.AppendLine(line.Substring(6));
                            }
                        }
                    }
                }
            }
        }
    }
}
