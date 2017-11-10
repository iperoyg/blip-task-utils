using Lime.Protocol;
using Lime.Protocol.Serialization;
using Lime.Protocol.Serialization.Newtonsoft;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;

namespace IAAnalyseHttpSample
{
    public class BlipAnalysesHelper : IBlipAnalysesHelper, IDisposable
    {
        private string _authorizationKey;
        private HttpClient _client = new HttpClient();

        public BlipAnalysesHelper(string authorizationKey)
        {
            _authorizationKey = authorizationKey;
            _client.BaseAddress = new Uri("https://msging.net");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key", authorizationKey);
        }

        public async Task<AnalysisResponse> Analyse(string analysisRequest)
        {
            if (analysisRequest == null) throw new ArgumentNullException(nameof(analysisRequest));

            try
            {
                var command = new Command
                {
                    Id = EnvelopeId.NewId(),
                    To = Node.Parse("postmaster@ai.msging.net"),
                    Uri = new LimeUri("/analysis"),
                    Method = CommandMethod.Set,
                    Resource = new AnalysisRequest
                    {
                        Text = analysisRequest
                    }
                };

                var documentSerializer = new DocumentSerializer();

                var envelopeSerializer = new JsonNetSerializer();
                var commandString = envelopeSerializer.Serialize(command);

                var httpContent = new StringContent(commandString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("/commands", httpContent);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var envelopeResult = (Command)envelopeSerializer.Deserialize(responseBody);

                return envelopeResult.Resource as AnalysisResponse;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        public async Task<AnalysisResponse> AnalyseForMetrics(string analysisRequest)
        {
            if (analysisRequest == null) throw new ArgumentNullException(nameof(analysisRequest));

            try
            {
                var command = new Command
                {
                    Id = EnvelopeId.NewId(),
                    To = Node.Parse("postmaster@ai.msging.net"),
                    Uri = new LimeUri("/analysis"),
                    Method = CommandMethod.Set,
                    Resource = new AnalysisRequest
                    {
                        TestingRequest = true,
                        Text = analysisRequest
                    }
                };

                var envelopeSerializer = new JsonNetSerializer();
                var commandString = envelopeSerializer.Serialize(command);

                var httpContent = new StringContent(commandString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("/commands", httpContent);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var envelopeResult = (Command)envelopeSerializer.Deserialize(responseBody);

                return envelopeResult.Resource as AnalysisResponse;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }

    public interface IBlipAnalysesHelper
    {
        Task<AnalysisResponse> Analyse(string analysisRequest);

        Task<AnalysisResponse> AnalyseForMetrics(string analysisRequest);
    }
}
