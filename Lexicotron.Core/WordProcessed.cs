﻿using Lexicotron.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexicotron.Core
{
    /// <summary>
    /// Define word output
    /// </summary>
    public class WordProcessed : IWord
    {
        public WordProcessed()
        {
            LexicalFieldCollection = new HashSet<string>();
        }

        /// <summary>
        /// The given word
        /// </summary>
        public string Word { get; set; }
        /// <summary>
        /// Occurences of the word in the article, 1 based
        /// </summary>
        public int Occurence { get; set; }
        /// <summary>
        /// The genre of the word
        /// </summary>
        public string Genre { get; set; }
        /// <summary>
        /// if the word is a Plural (p) or singular (s)
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Grammar category of the word (Adjective, verb, noun...)
        /// </summary>
        public string GrammarCategory { get; set; }
        /// <summary>
        /// Lemme, the "infinitive" form of the word (ex : parler => parle)
        /// </summary>
        public string Lemme { get; set; }
        /// <summary>
        /// Frequenc of this lemme in the lexic corpus (cf studies)
        /// </summary>
        public double FrequenceLemme { get; set; }

        /// <summary>
        /// The lexical fields of the word, separed by semicolon
        /// </summary>
        public string LexicalField { get => String.Join(";", LexicalFieldCollection); }
        /// <summary>
        /// The collection of lexicalFields, should be distincts
        /// </summary>
        [NonPrintable]
        public HashSet<string> LexicalFieldCollection { get; set; }
        /// <summary>
        /// number of words wich are this words (above)
        /// </summary>
        public int HyperonymCount { get; set; }
        /// <summary>
        /// number of words wich are a kind of this word (below)
        /// </summary>
        public int HyponymCount { get; set; } 
        /// <summary>
        /// number of other relation with this word
        /// </summary>
        public int OtherCount { get; set; }

        public bool Equals(IWord other)
        {
            if (other is null)
                return false;

            return other is WordProcessed && this.Word == other.Word;
        }

        public override bool Equals(object obj) => Equals(obj as IWord);
        public override int GetHashCode() => (Word).GetHashCode();
    }
}
