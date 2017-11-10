using Lime.Protocol;
using Lime.Protocol.Serialization;
using System;
using System.Threading.Tasks;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;

namespace BucketImportTask
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            args = new string[]
            {
                "d2ViaG9vazF0ZXN0OlpRQ0gxUWU2TUlIazQ4TzJCSjVh",
                "d2ViaG9vazJ0ZXN0OlRUM3dFZ2tpNXEzMlNWVzdYZVRo"
            };

            TypeUtil.RegisterDocument<AnalysisResponse>();

            if (args.Length < 2) {
                Console.WriteLine("Error: You must pass at least two parameters: [sourceKey] and [targetKey]\nPress some key to finish...");
                Console.ReadLine();
                return;
            };

            var sourceKey = args[0];
            var targetKey = args[1];

            DocumentCollection resourcesKeysToCopy;
            DocumentCollection documentsKeysToCopy;
            DocumentCollection profileKeysToCopy;

            //1. Get all Documents from the source bot
            using (var client = new BucketHelper(sourceKey))
            {
                resourcesKeysToCopy = await client.GetAllDocumentKeysAsync(BucketNamespace.Resource);
                documentsKeysToCopy = await client.GetAllDocumentKeysAsync(BucketNamespace.Document);
                profileKeysToCopy = await client.GetAllDocumentKeysAsync(BucketNamespace.Profile);

                Console.WriteLine($"[{resourcesKeysToCopy.Total}] resources found on the source bot");
                Console.WriteLine($"[{documentsKeysToCopy.Total}] documents found on the source bot");
                Console.WriteLine($"[{profileKeysToCopy.Total}] profile informations found on the source bot");
            }

            //2. Copy all Documents to the target bot
            using (var client = new BucketHelper(targetKey))
            {
                var resourcePairsToCopy = await client.GetAllDocumentsAsync(resourcesKeysToCopy);
                var documentPairsToCopy = await client.GetAllDocumentsAsync(documentsKeysToCopy);

                foreach (var resourcePair in resourcePairsToCopy)
                {
                    await client.AddAsync(resourcePair.Key, resourcePair.Value);
                }

                foreach (var documentPair in documentPairsToCopy)
                {
                    await client.AddAsync(documentPair.Key, documentPair.Value);
                }
            }
        }
    }
}
