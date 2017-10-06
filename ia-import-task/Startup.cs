using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol.Server;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.ArtificialIntelligence;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;

namespace ia_import_task
{
    /// <summary>
    /// Defines a type that is called once during the application initialization.
    /// </summary>
    public class Startup : IStartable
    {
        private readonly ISender _sender;
        private readonly Settings _settings;
        private readonly IArtificialIntelligenceExtension _artificialIntelligenceExtension;

        public Startup(ISender sender, Settings settings, IArtificialIntelligenceExtension artificialIntelligenceExtension)
        {
            _sender = sender;
            _settings = settings;
            _artificialIntelligenceExtension = artificialIntelligenceExtension;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if(_settings.IntentionsFilePath != null)
            {
                Console.WriteLine("Starting import intentions...");
                await ImportIntentions(cancellationToken);
                Console.WriteLine("Intentions imported with success...");
            }

            if (_settings.EntitiesFilePath != null)
            {
                Console.WriteLine("Starting import entities...");
                await ImportEntities(cancellationToken);
                Console.WriteLine("Entities imported with success...");
            }
        }

        private async Task ImportIntentions(CancellationToken cancellationToken)
        {
            var intentionsMap = new Dictionary<string, List<string>>();

            //Get intentions on file
            var csv = new Chilkat.Csv
            {
                //  Prior to loading the CSV file, indicate that the 1st row
                //  should be treated as column names:
                HasColumnNames = true
            };

            //  Load the CSV records from intentions the file:
            bool success = csv.LoadFile(_settings.IntentionsFilePath);
            if (!success)
            {
                Console.WriteLine(csv.LastErrorText);
                return;
            }

            //  Display the contents of the 3rd column (i.e. the country names)
            for (int row = 0; row <= csv.NumRows - 1; row++)
            {
                var question = csv.GetCell(row, 0);
                var intentionName = csv.GetCell(row, 1);

                var questionsList = intentionsMap.ContainsKey(intentionName) ? intentionsMap[intentionName] : new List<string>();

                questionsList.Add(question);
                intentionsMap[intentionName] = questionsList;
            }

            //Add each intention on BLiP IA model
            foreach (var intentionKey in intentionsMap.Keys)
            {

                var intention = new Intention
                {
                    Name = intentionKey,
                };
                var result = await _artificialIntelligenceExtension.SetIntentionAsync(intention, cancellationToken);

                intention.Id = result.Id;

                var questionsList = intentionsMap[intentionKey];
                var questionsArray = questionsList.Select(q => new Question { Text = q }).ToArray();

                await _artificialIntelligenceExtension.SetQuestionsAsync(result.Id, questionsArray, cancellationToken);
            }
        }

        private async Task ImportEntities(CancellationToken cancellationToken)
        {
            var entitiesMap = new Dictionary<string, List<EntityValues>>();

            //Get intentions on file
            var csv = new Chilkat.Csv
            {
                //  Prior to loading the CSV file, indicate that the 1st row
                //  should be treated as column names:
                HasColumnNames = true
            };

            //  Load the CSV records from the entites file:
            var success = csv.LoadFile(_settings.EntitiesFilePath);
            if (!success)
            {
                Console.WriteLine(csv.LastErrorText);
                return;
            }

            //Get entities on file
            //  Display the contents of the 3rd column (i.e. the country names)
            for (int row = 0; row <= csv.NumRows - 1; row++)
            {
                var entityName = csv.GetCell(row, 0);
                var value = csv.GetCell(row, 1);
                var synonymous = csv.GetCell(row, 2);
                var synonymousList = synonymous.Split(';');

                var entitiesValuesList = entitiesMap.ContainsKey(entityName) ? entitiesMap[entityName] : new List<EntityValues>();

                var entity = new EntityValues
                {
                    Name = value,
                    Synonymous = synonymousList.ToArray()
                };

                entitiesValuesList.Add(entity);
                entitiesMap[entityName] = entitiesValuesList;
            }

            //Add each intention on BLiP IA model
            foreach (var entityKey in entitiesMap.Keys)
            {
                var entity = new Entity
                {
                    Name = entityKey,
                    Values = entitiesMap[entityKey].ToArray()
                };
                var result = await _artificialIntelligenceExtension.SetEntityAsync(entity, cancellationToken);
                entity.Id = result.Id;
            }
        }
    }
}
