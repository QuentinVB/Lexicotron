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
        readonly static int Goal = 3924;

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
                Console.WriteLine(" 2. Try retrive word data from babel");
                Console.WriteLine(" 3. Try retrieve relation data from babel");
                //4. generate lexical field frequency from lexical field and lexicon
                Console.WriteLine(("").PadRight(44, '-'));
            } while (!Int32.TryParse(Console.ReadLine(), out number));

            switch (number)
            {
                case 1:
                    Console.WriteLine("Explore Mode");
                    ExploreMode(startTimestamp);
                    break;
                case 2:
                    Console.WriteLine("Retrive Words Mode");
                    RetriveWordMode(startTimestamp);
                    break;
                case 3:
                    Console.WriteLine("Retrive Relations Mode");
                    RetriveRelationsMode(startTimestamp);
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

            List<ArticleGroup> articlegroups = lexicotron.ProcessAllDirectory(directory + @"\Input\");//

            //TODO : make it async with progression bar 
            Console.WriteLine("Write to excel file...");

            foreach (ArticleGroup articlegroup in articlegroups)
            {
                FileWriter.PrintArticlesToExcel(articlegroup, lexicotron.LexicalFieldsList, startTimestamp); ;
            }
            

        }

        private static void RetriveWordMode(string startTimestamp)
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

            LocalWordDB database = new LocalWordDB();
            BabelAPICore babelAPI = new BabelAPICore(apikey,database);
            Console.WriteLine("{0} remaining request for today : {1} ",database.GetTodayBabelRequestsCount(),DateTime.Today.ToString());
            
            int amount = 100;
            Console.WriteLine("try to retrieve {0} senses", amount);
            Console.WriteLine("Asking babel...");
            var wordsenses = babelAPI.RetrieveWordSenses(amount);

            //convert words from Sense to DbWord list and log : SOLID VIOLATION !
            HashSet<DbWord> dbwordToUpdate = babelAPI.ParseBabelSenseToDbWord(wordsenses);
            Console.WriteLine("Recieved {0} words to update or insert in the database", dbwordToUpdate.Count);

            //insert retrieved synset into database
            var results = database.UpdateOrAddWordsWithSynset(dbwordToUpdate);
            Console.WriteLine("{0} words inserted, {1} words updated", results.Item1, results.Item2);
            int totalWordCount = database.GetWordCount();
            int wordWithoutSynset = database.GetWordWithoutSynsetCount();
            int wordsDone = totalWordCount - wordWithoutSynset;
            double stat = Math.Round((1-((double)wordWithoutSynset  / (double)totalWordCount))*100.0,2);
            int goalNotCompleted = database.GetWordsNotCompletedCount(Goal);
            double goalRatio = Math.Round((1 - ((double)goalNotCompleted / (double)Goal)) * 100.0,2);
            Console.WriteLine($"Words with synset in database : {wordsDone}/{totalWordCount} ({stat}% completed), {wordWithoutSynset} left. \nGoal: {Goal-goalNotCompleted}/{Goal} ({goalRatio}%)");
        }
        private static void RetriveRelationsMode(string startTimestamp)
        {
            Console.WriteLine("Here be dragonz again");
            string apikey;
            try
            {
                apikey = System.IO.File.ReadAllText(Helpers.GetExecutingDirectoryPath() + @"\babel.apikey");
            }
            catch (Exception)
            {
                throw;
            }

            LocalWordDB database = new LocalWordDB();
            BabelAPICore babelAPI = new BabelAPICore(apikey, database);
            Console.WriteLine("{0} remaining request for today : {1} ", 1000-database.GetTodayBabelRequestsCount(), DateTime.Today.ToString());

            int amount = 100;
            Console.WriteLine("try to retrieve {0} relations", amount);
            Console.WriteLine("Asking babel...");
            var relations = babelAPI.RetrieveRelations(amount);
            Console.WriteLine($"{relations.Count()} responses");

            //convert words from BabelRelation to DbRelation list and log : SOLID VIOLATION !           
            HashSet<DbRelation> dbRelationToUpdate = babelAPI.ParseBabelRelationsToDbRelation(relations);
            Console.WriteLine("Recieved {0} relations to update or insert in the database", dbRelationToUpdate.Count);


            //TODO : insert retrieved relations into database and update the word to said they have relations now
            var results = database.TryAddRelations(dbRelationToUpdate);
            Console.WriteLine("{0} relations inserted", results);


            int wordsWithRelationUpdated = database.UpdateWordRelationStatus(relations.Select(x=>x.Item2), true);
            
            int totalWordCount = database.GetWordCount();
            double stat = Math.Round((1 - ((double)wordsWithRelationUpdated / (double)totalWordCount)) * 100.0, 2);

            int goalNotCompleted = database.GetWordWithoutRelationCount(Goal);
            double goalRatio = Math.Round((1 - ((double)goalNotCompleted / (double)Goal)) * 100.0, 2);

            Console.WriteLine($"Words with relations : {wordsWithRelationUpdated}/{totalWordCount} ({stat}% completed). \n" +
                $"Goal: {Goal - goalNotCompleted}/{Goal} ({goalRatio}%)");


            /*
            int wordWithoutSynset = database.GetWordWithoutSynsetCount();
            int wordsDone = totalWordCount - wordWithoutSynset;
            double stat = Math.Round((1 - ((double)wordWithoutSynset / (double)totalWordCount)) * 100.0, 2);
            int goal = 3924;
            int goalNotCompleted = database.GetWordsNotCompletedCount(goal);
            Console.WriteLine($"Words with synset in database : {wordsDone}/{totalWordCount} ({stat}% completed), {wordWithoutSynset} left. Goal: {goal - goalNotCompleted}/{goal} ({goalRatio}%)");
            */
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
                    Console.WriteLine($"{lexicotron.Lexicon.Count} words from Lexic loaded !");
                }
                else if(finished == loadLexicalField)
                {
                    var result = loadLexicalField.Result;
                    lexicotron.LexicalFields = result.Item1;
                    lexicotron.LexicalFieldsList = result.Item2;

                    Console.WriteLine($"{lexicotron.LexicalFieldsList.Length} lexicals fields loaded !");
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

        private static Task<(Dictionary<string,string[]>,string[])> loadLexicalFieldAsync()
        {
            (Dictionary<string, string[]>, string[]) result;
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
