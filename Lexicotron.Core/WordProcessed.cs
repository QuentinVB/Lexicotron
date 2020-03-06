using System;
using System.Collections.Generic;
using System.Text;

namespace Lexicotron.Core
{
    /// <summary>
    /// Define word output
    /// </summary>
    public class WordProcessed
    {
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

        /*
         Champs lexical
             */
    }
}
