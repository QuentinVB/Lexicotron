using Lexicotron.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lexicotron.Core
{
    public class Lexicotron
    {
        public static List<Article> ProcessDirectory(string path, IEnumerable<Word> wordlexic)//async -> return async enumerable (glup)
        {           
            string[] fileEntries = Directory.GetFiles(path,"*.txt");

            List<Article> _articles = new List<Article>(fileEntries.Length);

            foreach (string filePath in fileEntries)
            {
                //if (filePath.EndsWith(".txt"))
                    //TODO : strip path + use array instead of list
                    Article article = new Article(GetNameOnlyFromPath(filePath));
                    _articles.Add(article);
                    FileReader.ProcessFile(article, filePath);                  
            }

            return _articles;
        }

        public static string GetNameOnlyFromPath(string path)
        {
            int i = path.Length-1;
            do
            {
                if(path[i]=='\\')
                {
                    i++;
                    return (path[i..])[0..^4]; //substitute path.Substring(i, path.Length - i); :)
                }

                i--;
            } while (i > 0);
            throw new ArgumentException("Not a path provided");
        }
    }
}
