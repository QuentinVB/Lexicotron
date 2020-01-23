﻿using System;
using System.Collections.Generic;

namespace Lexicotron.Core
{
    public class Article
    {
        readonly string _filename;
        Dictionary<string,int> _words;

        public Article()
            :this(new Guid().ToString())
        { }

        public Article(string filename)
            :this(filename,new Dictionary<string, int>())
        {}

        public Article(string filename, Dictionary<string, int> words)
        {
            _filename = filename;
            _words = words;
        }

        public Dictionary<string, int> Words { get => _words; set => _words = value; }

        public string Filename => _filename;

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

        public override string ToString()
        {
            return $"{Filename}, words stored : {_words.Count}";
        }
    }
}
