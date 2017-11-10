using System;
using System.Collections.Generic;
using System.Text;

namespace IAImportTask
{
    /// <summary>
    /// Define a simple settings file that can be injected to the receivers.
    /// This type and its values must be registered in the application.json file.
    /// </summary>
    public class Settings
    {
        public string IntentionsFilePath { get; set; }

        public string EntitiesFilePath { get; set; }
    }
}
