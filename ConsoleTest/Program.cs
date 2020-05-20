using Lexicotron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            Console.WriteLine("begin test");

            //FileWriter.PrintArticlesToExcel(new List<Article>() { Article.Dummy(1), Article.Dummy(2) }, startTimestamp);

            Console.Read();
        }
    }
}
