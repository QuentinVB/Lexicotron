using System;
using System.Collections.Generic;

namespace Lexicotron.Core
{
    public class Article
    {
        Dictionary<string,int> _words;

        public Article()
            :this(new Dictionary<string, int>())
        {}

        public Article(Dictionary<string, int> words)
        {
            _words = words;
        }

        public Dictionary<string, int> Words { get => _words; set => _words = value; }

        public bool IncrementWord(string word)
        {
            if(_words.ContainsKey(word))
            {
                _words[word]++;
            }
            else
            {
                _words.Add(word, 1);
                return false;
            }
            return true;
        }

        public bool AddWord(string word)
        {
            if(!_words.TryAdd(word, 1))
            {
                _words[word]++; return false;
            }


            return true;
        }
    }
}
