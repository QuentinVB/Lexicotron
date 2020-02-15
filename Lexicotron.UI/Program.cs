using Lexicotron.Core;
using Lexicotron.Database;
using Lexicotron.UI.ConsoleHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicotron.UI
{
    class Program
    {
       
        static void Main(string[] args)
        {
            string startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            Console.WriteLine("Lexicotron v1.0");
            Console.WriteLine("Loading ressources...");
            var spin = new ConsoleSpinner();

            //TODO : load the ressources async
            Core.Lexicotron lexicotron = new Core.Lexicotron();
            var loading = loadRessources(lexicotron);
            /*
            while(!loading.IsCompleted)
            {
                spin.Turn();
            }*/
            Console.WriteLine("Ressources loaded !");

            var directory = Helpers.GetExecutingDirectoryPath();
            Console.WriteLine($"Executing folder is {directory}");
            Console.WriteLine("Processing...");

            List<Article> articles = lexicotron.ProcessDirectory(directory + "\\Input\\");//


            FileWriter.PrintArticlesSummary(articles, startTimestamp); ;
            FileWriter.PrintArticles(articles, startTimestamp);

            
            Console.WriteLine("Finished !");
            foreach (Article item in articles)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }

        static async Task loadRessources(Core.Lexicotron lexicotron)
        {
            var loadLexicon = loadLexiconAsync();

            var allTasks = new List<Task> { loadLexicon };
            while (allTasks.Any())
            {
                Task finished = await Task.WhenAny(allTasks);
                if (finished == loadLexicon)
                {
                    lexicotron.Lexicon = loadLexicon.Result;
                }
                
                allTasks.Remove(finished);
            }
        }

        private static Task<List<Word>> loadLexiconAsync()
        {
            var result = CsvLoader.LoadCSV();
            return Task.FromResult(result);
        }
    }
}
