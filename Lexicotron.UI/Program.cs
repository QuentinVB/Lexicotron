using Lexicotron.Core;
using System;
using System.Collections.Generic;

namespace Lexicotron.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Lexicotron v1.0");

            Console.WriteLine($"Executing folder is {Helpers.GetExecutingDirectoryPath()}");

            List<Article> articles = Process.ProcessDirectory(Helpers.GetExecutingDirectoryPath()+"\\Input\\");//

            FileWriter.PrintArticlesSummary(articles);

            foreach (Article item in articles)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }
    }
}
