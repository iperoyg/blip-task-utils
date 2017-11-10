using Lime.Protocol;
using Lime.Protocol.Serialization;
using Lime.Protocol.Serialization.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BucketImportTask
{
    public class BucketHelper : IBucketHelper, IDisposable
    {
        private string _authorizationKey;
        private HttpClient _client = new HttpClient();

        public BucketHelper(string authorizationKey)
        {
            _authorizationKey = authorizationKey;
            _client.BaseAddress = new Uri("https://msging.net");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key", authorizationKey);
        }

        public async Task AddAsync(string key, Document document)
        {
            try
            {
                var command = new Command
                {
                    Id = EnvelopeId.NewId(),
                    To = Node.Parse("postmaster@msging.net"),
                    Uri = new LimeUri($"/buckets/{key}"),
                    Method = CommandMethod.Set,
                    Resource = document
                };

                var documentSerializer = new DocumentSerializer();

                var envelopeSerializer = new JsonNetSerializer();
                var commandString = envelopeSerializer.Serialize(command);

                var httpContent = new StringContent(commandString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("/commands", httpContent);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                //var envelopeResult = (Command)envelopeSerializer.Deserialize(responseBody);

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<DocumentCollection> GetAllDocumentKeysAsync(BucketNamespace bucketNamespace = BucketNamespace.Document)
        {
            string @namespace;
            switch (bucketNamespace)
            {
                case BucketNamespace.Document:
                    @namespace = "bucket";
                    break;

                case BucketNamespace.Resource:
                    @namespace = "resources";
                    break;

                case BucketNamespace.Profile:
                    @namespace = "profile";
                    break;

                default:
                    @namespace = "bucket";
                    break;
            }

            try
            {
                var command = new Command
                {
                    Id = EnvelopeId.NewId(),
                    To = Node.Parse("postmaster@msging.net"),
                    Uri = new LimeUri($"/{@namespace}/"),
                    Method = CommandMethod.Get
                };

                var documentSerializer = new DocumentSerializer();

                var envelopeSerializer = new JsonNetSerializer();
                var commandString = envelopeSerializer.Serialize(command);

                var httpContent = new StringContent(commandString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("/commands", httpContent);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var envelopeResult = (Command)envelopeSerializer.Deserialize(responseBody);

                return envelopeResult.Resource as DocumentCollection;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<KeyValuePair<string, Document>>> GetAllDocumentsAsync(DocumentCollection keysCollection)
        {
            try
            {
                var pairsCollection = new List<KeyValuePair<string, Document>>();

                foreach (var key in keysCollection)
                {
                    var command = new Command
                    {
                        Id = EnvelopeId.NewId(),
                        To = Node.Parse("postmaster@msging.net"),
                        Uri = new LimeUri($"/buckets/{key}"),
                        Method = CommandMethod.Get
                    };

                    var documentSerializer = new DocumentSerializer();

                    var envelopeSerializer = new JsonNetSerializer();
                    var commandString = envelopeSerializer.Serialize(command);

                    var httpContent = new StringContent(commandString, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await _client.PostAsync("/commands", httpContent);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var envelopeResult = (Command)envelopeSerializer.Deserialize(responseBody);
                    var document = envelopeResult.Resource;

                    pairsCollection.Add(new KeyValuePair<string, Document>(key.ToString(), document));
                }

                return pairsCollection;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }

    public interface IBucketHelper
    {
        Task<DocumentCollection> GetAllDocumentKeysAsync(BucketNamespace bucketNamespace);

        Task<IEnumerable<KeyValuePair<string, Document>>> GetAllDocumentsAsync(DocumentCollection keysCollection);

        Task AddAsync(string key, Document document);
    }
}
