using Lexicotron.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Lexicotron.Core
{
    public class Lexicotron
    {
        public static List<Article> ProcessDirectory(string path, List<Word> wordlexic)//async -> return async enumerable (glup)
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
                
                GetWordsInfo(article, wordlexic);

                //TODO : search for lexical field

                article.Words.OrderBy(w => w.Value.Occurence);
            }

            return _articles;
        }


        /*
         VER
         ADJ
         NOM
             
             */

        private static void GetWordsInfo(Article article, List<Word> wordlexic)
        {
            Dictionary<string, WordProcessed> updatedWords = new Dictionary<string, WordProcessed>();

            foreach (WordProcessed word in article.Words.Values)
            {
                var c = wordlexic.Where(w => w.ortho == word.Word).FirstOrDefault();
                if(c != null
                    && ( 
                    c.cgram == "VER"
                    || c.cgram.StartsWith("ADJ")
                    || c.cgram == "NOM")
                    )
                {
                    updatedWords.Add(
                        word.Word,
                        new WordProcessed() {
                            Word = word.Word,
                            Occurence = word.Occurence,
                            Genre = c.genre,
                            Nombre = c.nombre,
                            GrammarCategory = c.cgram,
                            Lemme = c.lemme,
                            FrequenceLemme = c.freqlemfilms
                        });
                }       
            }

            article.Words = updatedWords;
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
