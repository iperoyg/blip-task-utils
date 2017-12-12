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
        private const string AUTHORIZATION_KEY = "dGVzdGV3ZWJob29raGFuZ291dHM6S0tqNm93RWJiTUFvSDExbVdCdUM=";

        static void Main(string[] args)
        {
            TypeUtil.RegisterDocument<AnalysisResponse>();
            TypeUtil.RegisterDocument<Intention>();
            TypeUtil.RegisterDocument<Entity>();
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            using (var client = new BlipAnalysesHelper(AUTHORIZATION_KEY))
            {

                // ***************************
                // Step 1: Preparing the model
                // ***************************

                //Create an intent
                var intentId = await client.AddIntent("abacate");

                //Add questions for the created intent
                var sampleQuestions = new Question[] { new Question { Text = "Quer comer abacate ?" } };
                await client.AddQuestions(intentId, sampleQuestions);

                //Delete some intent
                await client.DeleteIntent(intentId);

                //Train created model
                await client.TrainModel();

                //Get created models Id
                var models = await client.GetModels();

                //Get correct model id, for example:
                // NOTE: Pay attention if you are getting nearsty model
                var modelId = models[0].Id;

                //Publish some model
                var result = await client.PublishModel(modelId);


                // **********************
                // Step 2: Analysing text
                // **********************

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
