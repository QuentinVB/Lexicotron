using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;

namespace Lexicotron.Core
{
    public class Article
    {
        readonly string _filename;
        int _totalWordCount=0;
        Dictionary<string,WordProcessed> _words;

        public Article()
            :this(new Guid().ToString())
        { }

        public Article(string filename)
            :this(filename,new Dictionary<string, WordProcessed>())
        {}

        public Article(string filename, Dictionary<string, WordProcessed> words)
        {
            _filename = filename;
            _words = words;
        }
        [Ignore]
        public Dictionary<string, WordProcessed> Words { get => _words; set => _words = value; }

        public string Filename => _filename;
        public int TotalWordCount { get => _totalWordCount; }
        public int DistinctWordCount { get => _words.Count; }

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

        public bool AddWord(string word)
        {
            if (!_words.TryAdd(word,  new WordProcessed() { Word = word, Occurence = 1 }))
            {
                _words[word].Occurence++;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{Filename}, words stored : {_words.Count}";
        }
    }
}
