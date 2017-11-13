using Lime.Protocol;
using Lime.Protocol.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
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
            //args = new string[]
            //{
            //    "d2ViaG9vazF0ZXN0OlpRQ0gxUWU2TUlIazQ4TzJCSjVh",
            //    "d2ViaG9vazJ0ZXN0OlRUM3dFZ2tpNXEzMlNWVzdYZVRo"
            //};

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

            IEnumerable<KeyValuePair<string, Document>> resourcePairsToCopy;
            IEnumerable<KeyValuePair<string, Document>> documentPairsToCopy;
            IEnumerable<KeyValuePair<string, Document>> profilePairsToCopy;

            //1. Get all Documents from the source bot
            using (var client = new BucketHelper(sourceKey))
            {
                resourcesKeysToCopy = await client.GetAllDocumentKeysAsync(BucketNamespace.Resource) ?? new DocumentCollection();
                documentsKeysToCopy = await client.GetAllDocumentKeysAsync(BucketNamespace.Document) ?? new DocumentCollection();
                profileKeysToCopy = await client.GetAllDocumentKeysAsync(BucketNamespace.Profile) ?? new DocumentCollection();

                Console.WriteLine($"[{resourcesKeysToCopy.Total}] resources found on the source bot");
                Console.WriteLine($"[{documentsKeysToCopy.Total}] documents found on the source bot");
                Console.WriteLine($"[{profileKeysToCopy.Total}] profile informations found on the source bot");

                resourcePairsToCopy = await client.GetAllDocumentsAsync(resourcesKeysToCopy, BucketNamespace.Resource);
                documentPairsToCopy = await client.GetAllDocumentsAsync(documentsKeysToCopy, BucketNamespace.Document);
                profilePairsToCopy = await client.GetAllDocumentsAsync(profileKeysToCopy, BucketNamespace.Profile);
            }

            //2. Copy all Documents to the target bot
            using (var client = new BucketHelper(targetKey))
            {
                if (resourcePairsToCopy != null)
                {
                    foreach (var resourcePair in resourcePairsToCopy)
                    {
                        await client.AddAsync(resourcePair.Key, resourcePair.Value, BucketNamespace.Resource);
                    }
                }

                if (documentPairsToCopy != null)
                {
                    foreach (var documentPair in documentPairsToCopy)
                    {
                        await client.AddAsync(documentPair.Key, documentPair.Value, BucketNamespace.Document);
                    }
                }

                if (profilePairsToCopy != null)
                {
                    foreach (var profilePair in profilePairsToCopy)
                    {
                        await client.AddAsync(profilePair.Key, profilePair.Value, BucketNamespace.Profile);
                    }
                }
            }
        }
    }
}
