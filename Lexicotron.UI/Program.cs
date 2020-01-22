using Lexicotron.Core;
using Lexicotron.IO;
using System;

namespace Lexicotron.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Article article = new Article();
            
            Console.WriteLine("Hello World!");

            Console.WriteLine($"Dictionnary for this article has  { article.Words.Count} element.");

            article.AddWord(Console.ReadLine());

            Console.WriteLine($"Word added, dictionnary for this article has now {article.Words.Count} element. ");

            Console.WriteLine($"Executing folder is {Helpers.GetExecutingDirectoryName()}");

            FileReader.read(Helpers.GetExecutingDirectoryName());

            Console.ReadLine();
        }
    }
}
