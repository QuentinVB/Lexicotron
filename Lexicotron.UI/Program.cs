using Lexicotron.Core;
using System;
using System.Collections.Generic;

namespace Lexicotron.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Article article = new Article();
            
            Console.WriteLine("Hello World!");

            Console.WriteLine($"Executing folder is {Helpers.GetExecutingDirectoryName()}");

            List<Article> articles = Process.ProcessDirectory(Helpers.GetExecutingDirectoryName());

            foreach (Article item in articles)
            {
                Console.WriteLine(item);
            }

            Console.ReadLine();
        }
    }
}
