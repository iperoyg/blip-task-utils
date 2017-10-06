using System;
using Take.Blip.Client.Console;

namespace ia_import_task
{
    class Program
    {
        static int Main(string[] args) => ConsoleRunner.RunAsync(args).GetAwaiter().GetResult();
    }
}