using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;

namespace Lexicotron.Core
{
    /// <summary>
    /// Describe an article to process and the inherent functionnality
    /// </summary>
    public class Article
    {
        readonly string _filename;
        int _totalWordCount=0;
        Dictionary<string,WordProcessed> _words;

       

        /// <summary>
        /// Create a blank article
        /// </summary>
        public Article()
            :this(new Guid().ToString())
        { }
        /// <summary>
        /// Create an article based on a given file name
        /// </summary>
        /// <param name="filename"></param>
        public Article(string filename)
            :this(filename,new Dictionary<string, WordProcessed>())
        {}

        /// <summary>
        /// Create an article based on a given file name and on a dictionnary of word
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="words"></param>
        public Article(string filename, Dictionary<string, WordProcessed> words)
        {
            _filename = filename;
            _words = words;
        }
        /// <summary>
        /// The collection of words contained in the article. The reactor uranium fuel !
        /// ignored when printing the summary of articles
        /// </summary>
        [Ignore]
        [NonPrintable]
        public Dictionary<string, WordProcessed> Words { get => _words; set => _words = value; }

        public string Filename => _filename;
        public int TotalWordCount { get => _totalWordCount; }
        public int DistinctWordCount { get => _words.Count; }

        /// <summary>
        /// Add +1 to the occurences of the given word, if doesent exist add the word
        /// </summary>
        /// <param name="word">the given word</param>
        /// <returns>true if the word exists</returns>
        public bool IncrementWord(string word)
        {
            if(_words.ContainsKey(word))
            {
                _words[word].Occurence++;
                return true;
            }
            else
            {
                _words.Add(word, new WordProcessed() {Word = word, Occurence = 1 });
                return false;
            }
        }
        /// <summary>
        /// Add a new given word to the dictionnary
        /// </summary>
        /// <param name="word">the given word</param>
        /// <returns>true if the word is added</returns>
        public bool AddWord(string word)
        {
            if (_words.ContainsKey(word))
            {
                _words[word].Occurence++;
                return false;
            }
            else 
            {
                _words.Add(word, new WordProcessed() { Word = word, Occurence = 1 });
                return true;
            }
        }

        public override string ToString()
        {
            return $"{Filename}, words stored : {_words.Count}";
        }

        public static Article Dummy(int dummyIndex)
        {
            Article dummy=  new Article("articletest "+ dummyIndex.ToString() );


            dummy.Words.Add("test", new WordProcessed
            {
                Word = "test",
                Genre = "m",
                Nombre = "s",
                Occurence = 1,
                Lemme = "test",
                FrequenceLemme = 1,
                GrammarCategory = "noun",
                LexicalField = "test"
            });

            return dummy;
        }
    }
}
