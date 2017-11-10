using System;
using Take.Blip.Client.Console;

namespace IAImportTask
{
    class Program
    {
        static int Main(string[] args) => ConsoleRunner.RunAsync(args).GetAwaiter().GetResult();
    }
}