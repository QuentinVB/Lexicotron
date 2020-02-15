using System;
using System.Collections.Generic;
using System.Text;

namespace Lexicotron.Core
{
    public class WordProcessed
    {
        private string word;
        private int occurence;
        private string genre;
        private string nombre;
        private string grammarCategory;
        private string lemme;
        private double frequenceLemme;

        public string Word { get => word; set => word = value; }
        public int Occurence { get => occurence; set => occurence = value; }
        public string Genre { get => genre; set => genre = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string GrammarCategory { get => grammarCategory; set => grammarCategory = value; }
        public string Lemme { get => lemme; set => lemme = value; }
        public double FrequenceLemme { get => frequenceLemme; set => frequenceLemme = value; } 
    }
}
