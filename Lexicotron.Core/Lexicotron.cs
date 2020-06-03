using Lexicotron.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Lexicotron.Database.Models;

namespace Lexicotron.Core
{
    /// <summary>
    /// Core class of the program to process the files
    /// </summary>
    public class Lexicotron
    {
        List<Word> lexicon;
        private Dictionary<string, string[]> lexicalFields;
        DALAdapter _dal;

        /// <summary>
        /// the lexic of words, loaded from csv or excel file
        /// </summary>
        public List<Word> Lexicon { get => lexicon; set => lexicon = value; }
        /// <summary>
        /// the lexical field association, word->lexicalfields[], loaded from csv or excel file
        /// </summary>
        public Dictionary<string, string[]> LexicalFields { get => lexicalFields; set => lexicalFields = value; }
        internal DALAdapter Database { get => _dal; set => _dal = value; }
        public string[] LexicalFieldsList { get; set; }

        public Lexicotron()
        {
            _dal = new DALAdapter(new LocalWordDB());           
        }
        /// <summary>
        /// LookRecrusively into each subfolder to process articles and groupe them
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<ArticleGroup> ProcessAllDirectory(string path)
        {
            Console.WriteLine("{0} words in the database", Database.GetWordCount());

            List<ArticleGroup> _articleGroups = new List<ArticleGroup>();


            void ExploreFolder(string localpath, string title)
            {
                string[] localFolders = Directory.GetDirectories(localpath);

                if (Directory.GetFiles(localpath).Length > 0)
                {
                    Console.WriteLine($"Process files from sub-folder : {title}");
                    _articleGroups.Add(new ArticleGroup
                    {
                        Name = title,
                        Articles = ProcessDirectory(localpath)
                    }); 
                    Console.WriteLine("Sub-folder finished. {0} words in the database", Database.GetWordCount());

                }
                else if (localFolders.Length > 0)
                {
                    for (int i = 0; i < localFolders.Length; i++)
                    {
                        ExploreFolder(localFolders[i], title + (String.IsNullOrWhiteSpace(title) ? "" : "-") + GetNameOnlyFromPath(localFolders[i]));
                    }
                }
                else
                {
                    return;
                }
            }

            ExploreFolder(path, "");

            return _articleGroups;
        }

        /// <summary>
        /// Load files from a given directory and process them, this is the reactor core !
        /// </summary>
        /// <param name="path">the given relative path to the directory</param>
        /// <returns></returns>
        public List<Article> ProcessDirectory(string path)//async -> return async enumerable (glup) ?
        {
            if (Lexicon == null) throw new InvalidOperationException("the lexic database is not loaded");
            if (LexicalFields == null) throw new InvalidOperationException("the lexical field database is not loaded");

            List<Article> _articles = new List<Article>();

            
            
            string[] fileEntries = Directory.GetFiles(path, "*.txt");
                    
            foreach (string filePath in fileEntries)
            {
                Console.WriteLine(" + {0}",GetNameOnlyFromPath(filePath));

                Article article = new Article(GetNameOnlyFromPath(filePath));

                //load all the word
                FileReader.ProcessFile(article, filePath);

                //ADD processing operations here
                GetWordsInfo(article);

                //add lexical fields and summarize them by article
                GetLexicalFields(article);

                //merge Lemme
                MergeLemme(article);


                //insert word in db
                //TODO: improve logging
                Console.WriteLine(" | {0} words inserted in database", Database.InsertWords(article.Words.Values));

                //match heterony/hyperonym
                MatchRelation(article);



                article.Words = article.Words.OrderByDescending(w => w.Value.Occurence).ToDictionary(x => x.Key, x => x.Value);

                _articles.Add(article);
            }
            return _articles;
        }

        private void MatchRelation(Article article)
        {
            foreach (WordProcessed word in article.Words.Values)
            {
                WordProcessed returnedWord = Database.GetRelationCount(word);
                word.HyperonymCount = returnedWord.HyperonymCount;
                word.HyponymCount = returnedWord.HyponymCount;
                word.OtherCount = returnedWord.OtherCount;
            }
        }

      
        /*
VER
ADJ
NOM

    */
        /// <summary>
        /// Get the Word information of the WordProcessed in the given article
        /// </summary>
        /// <param name="article">the given article</param>
        private void GetWordsInfo(Article article)
        {

            Dictionary<string, WordProcessed> updatedWords = new Dictionary<string, WordProcessed>();

            foreach (WordProcessed word in article.Words.Values)
            {
                //search the word in lexic
                var c = Lexicon.Where(w => w.ortho == word.Word).FirstOrDefault();

                if(c != null
                    && ( 
                    c.cgram == "VER"
                    || c.cgram =="ADJ"
                    //|| c.cgram.StartsWith("ADJ")
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

            //substitute the words collection of the article by the filtred one
            article.Words = updatedWords;
        }

        /// <summary>
        /// Get the words lexical field from the lexical field dictionnary
        /// TODO : get it from babelNet
        /// </summary>
        /// <param name="article">the given article</param>
        private void GetLexicalFields(Article article)
        {
            foreach (WordProcessed word in article.Words.Values)
            {
                if(LexicalFields.TryGetValue(word.Word, out string[] lexicalFields))
                {

                    for (int i = 0; i < lexicalFields.Length; i++)
                    {
                        word.LexicalFieldCollection.Add(lexicalFields[i]);
                        if(article.LexicalFieldCount.ContainsKey(lexicalFields[i]))
                        {
                            article.LexicalFieldCount[lexicalFields[i]]++;
                        }
                        else
                        {
                            article.LexicalFieldCount.Add(lexicalFields[i], 1);
                        }
                    }
                }
            }
        }

        private void MergeLemme(Article article)
        {
            article.Words = (from WordProcessed w in article.Words.Values.ToList()
                     group w by w.Lemme into nw
                     select new WordProcessed
                     {
                         Word = nw.First().Lemme,
                         Occurence = nw.Sum(a=>a.Occurence),
                         Genre = nw.First().Genre,
                         Nombre = nw.First().Nombre,
                         GrammarCategory = nw.First().GrammarCategory,
                         Lemme = nw.First().Lemme,
                         FrequenceLemme = nw.First().FrequenceLemme,
                         LexicalFieldCollection = nw.SelectMany(item=>item.LexicalFieldCollection).Distinct().ToHashSet()
                     })
                     .ToDictionary(g => g.Word, g => g);
        }


        /// <summary>
        /// Helper to get the name of the file from the path.
        /// </summary>
        /// <param name="path">the given path</param>
        /// <returns>the file name</returns>
        public static string GetNameOnlyFromPath(string path)
        {
            int i = path.Length-1;
            if(i<1) throw new ArgumentException("Not a path provided");

            do
            {
                if(path[i]=='\\')
                {
                    i++;
                    return path.Substring(i, path.Length - i); //(path[i..])[0..^4]; //
                }

                i--;
            } while (i > 0);
            throw new ArgumentException("Not a path provided");
        }

        
    }
}
