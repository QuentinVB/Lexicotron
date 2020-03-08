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
        /*
         var spin = new ConsoleSpinner();

             //TODO : load the ressources async
             
             //lexicotron.Lexicon = CsvLoader.LoadCSV();


             while(!loading.IsCompleted)
             {
                 spin.Turn();
             }*/
        static void Main(string[] args)
        {
            string startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            Console.WriteLine("Lexicotron v1.2");
            Console.WriteLine("Loading ressources...");

            Core.Lexicotron lexicotron = new Core.Lexicotron();

            var loading = loadRessources(lexicotron);          
            
            Console.WriteLine("Ressources loaded !");

            var directory = Helpers.GetExecutingDirectoryPath();
            Console.WriteLine($"Executing folder is {directory}");
            Console.WriteLine("Processing...");

            List<Article> articles = lexicotron.ProcessDirectory(directory + @"\Input\");//

            FileWriter.PrintArticlesSummary(articles, startTimestamp); ;
            FileWriter.PrintArticles(articles, startTimestamp);

            foreach (Article item in articles)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("Finished !");
            
            
            Console.ReadLine();
        }

        static async Task loadRessources(Core.Lexicotron lexicotron)
        {
            var loadLexicon = loadLexiconAsync();
            var loadLexicalField = loadLexicalFieldAsync();

            //TODO: Add hyperonymie ressources

            var allTasks = new List<Task> { loadLexicon , loadLexicalField };
            while (allTasks.Any())
            {
                Task finished = await Task.WhenAny(allTasks);
                if (finished == loadLexicon)
                {
                    lexicotron.Lexicon = loadLexicon.Result;
                    Console.WriteLine("Lexic loaded !");
                }
                else if(finished == loadLexicalField)
                {
                    lexicotron.LexicalField = loadLexicalField.Result;
                    Console.WriteLine("Lexical fields loaded !");
                }

                allTasks.Remove(finished);
            }
        }

        private static Task<List<Word>> loadLexiconAsync()
        {
            List<Word> result;

            try
            {
                result = CsvLoader.LoadCSV();

            }
            catch (Exception)
            {

                throw ;
            }
            return Task.FromResult(result);
        }

        private static Task<Dictionary<string,string[]>> loadLexicalFieldAsync()
        {
            Dictionary<string, string[]> result;
            try
            {
                result = ExcelLoader.LoadLexicalField(Helpers.GetExecutingDirectoryPath() + @"\Data\ChampsLexicaux.xlsx");
            }
            catch (Exception)
            {

                throw;
            }
            return Task.FromResult(result);
        }
    }
}
