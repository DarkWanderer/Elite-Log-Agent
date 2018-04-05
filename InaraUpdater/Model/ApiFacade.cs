using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InaraUpdater.Model
{
    public class ApiFacade
    {
        private readonly IRestClient client;
        private readonly string apiKey;
        private readonly string commanderName;

        public ApiFacade(IRestClient client, string apiKey, string commanderName)
        {
            this.client = client;
            this.apiKey = apiKey;
            this.commanderName = commanderName;
        }

        private struct ApiInputOutput
        {
            [JsonProperty("header")]
            public Header Header;
            [JsonProperty("events")]
            public ICollection<Event> Events;

        }

        public async Task<Event> ApiCall(Event @event) 
        {
            return (await ApiCall(new[] { @event })).Single();
        }

        public async Task<ICollection<Event>> ApiCall(ICollection<Event> events)
        {
            var inputData = new ApiInputOutput()
            {
                Header = new Header(commanderName, apiKey),
                Events = events
            };
            var inputJson = JsonConvert.SerializeObject(inputData);
            var outputJson = await client.PostAsync(inputJson);
            var outputData = JsonConvert.DeserializeObject<ApiInputOutput>(outputJson);

            // Verify output
            if (outputData.Header.EventStatus != 200)
                throw new ApplicationException($"Error from API: {outputData.Header.EventStatusText}");

            return outputData.Events;
        }
    }
}
