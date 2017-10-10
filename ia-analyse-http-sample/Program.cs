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
    public class Program
    {
        //Add your chatbot authorization key (For instance: asldkhgaslhdgaljhsdgaljhsgd==)
        private const string AUTHORIZATION_KEY = "your-authorization-key";

        static void Main(string[] args)
        {
            TypeUtil.RegisterDocument<AnalysisResponse>();
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            
            using(var client = new BlipAnalysesHelper(AUTHORIZATION_KEY))
            {
                var text = "Some text to be analysed";

                //Analyse a text (with text record)
                var analysisResponse = await client.Analyse(text);
                Console.WriteLine($"Result: Best Intent[{analysisResponse.Intentions[0].Name}] with score [{analysisResponse.Intentions[0].Score}]");

                //Analyse a text for metrics (without text record)
                var analysisResponseForMetrics = await client.AnalyseForMetrics(text);
                Console.WriteLine($"Result: Best Intent[{analysisResponseForMetrics.Intentions[0].Id}] with score [{analysisResponseForMetrics.Intentions[0].Score}]");
            }
        }
    }
}
