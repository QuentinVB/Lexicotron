using Lexicotron.Core;
using Lexicotron.Database;
using Lexicotron.UI.ConsoleHelper;
using System;
using System.Collections.Generic;

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
            /*
             make it async
            while (true)
            {
                spin.Turn();
            }*/

            List<Word> wordlexic= CsvLoader.LoadCSV();
            Console.WriteLine("Ressources loaded !");



            var directory = Helpers.GetExecutingDirectoryPath();
            Console.WriteLine($"Executing folder is {directory}");

            List<Article> articles = Core.Lexicotron.ProcessDirectory(directory + "\\Input\\", wordlexic);//

            FileWriter.PrintArticlesSummary(articles, startTimestamp); ;
            FileWriter.PrintArticles(articles, startTimestamp);

            foreach (Article item in articles)
            {
                Console.WriteLine(item);
            }
            
            Console.ReadLine();
        }
    }
}
