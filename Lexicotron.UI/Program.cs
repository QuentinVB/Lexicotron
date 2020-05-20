using Lexicotron.BabelAPI;
using Lexicotron.Core;
using Lexicotron.Database;
using Lexicotron.Database.Models;
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

            Console.WriteLine("Lexicotron v1.6");
            Console.WriteLine(("").PadRight(44, '-'));
            int number;
            do
            {
                Console.WriteLine("Choose mode :");
                Console.WriteLine(" 1. Explore articles words from \"input\" folder");
                Console.WriteLine(" 2. Try retrive data from babel");
                Console.WriteLine(("").PadRight(44, '-'));
            } while (!Int32.TryParse(Console.ReadLine(), out number));

            switch (number)
            {
                case 1:
                    Console.WriteLine("Explore Mode");
                    ExploreMode(startTimestamp);
                    break;
                case 2:
                    Console.WriteLine("Retrive Mode");

                    RetriveDataMode(startTimestamp);
                    break;
                default:
                    break;
            }
            Console.WriteLine("Finished !");

            Console.ReadLine();
        }

        public static void ExploreMode(string startTimestamp)
        {
            Console.WriteLine("Loading ressources...");

            Core.Lexicotron lexicotron = new Core.Lexicotron();

            //var spin = new ConsoleSpinner();

            var loading = loadRessources(lexicotron);
            //while (!loading.IsCompleted)
            //{
            //    spin.Turn();
            //}
            Console.WriteLine("Ressources loaded !");

            var directory = Helpers.GetExecutingDirectoryPath();
            Console.WriteLine($"Executing folder is {directory}");
            Console.WriteLine("Processing...");

            List<ArticleGroup> articles = lexicotron.ProcessAllDirectory(directory + @"\Input\");//

            //TODO : make it async with progression bar 
            Console.WriteLine("Write to excel file...");

            foreach (ArticleGroup article in articles)
            {
                FileWriter.PrintArticlesToExcel(article, startTimestamp); ;
            }
            

        }

        private static void RetriveDataMode(string startTimestamp)
        {
            Console.WriteLine("Here be dragonz");
            string apikey;
            try
            {
                apikey = System.IO.File.ReadAllText(Helpers.GetExecutingDirectoryPath() + @"\babel.apikey");
            }
            catch (Exception)
            {
                throw;
            }


            BabelAPICore babelAPI = new BabelAPICore(apikey);

            babelAPI.RetrieveWordSense(5);

        }

        private static async Task loadRessources(Core.Lexicotron lexicotron)
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
