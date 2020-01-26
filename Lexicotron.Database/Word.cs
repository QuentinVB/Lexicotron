using System;
using System.Collections.Generic;
using System.Text;

namespace Lexicotron.Database
{
    public class Word
    {
        public int Id { get; set; }
        public string ortho { get; set; }
        public string phon { get; set; }
        public string lemme { get; set; }
        public string cgram { get; set; }
        public string genre { get; set; }
        public string nombre { get; set; }
        public double freqlemfilms { get; set; }
        public double freqlemlivres { get; set; }
        public double freqfilms { get; set; }
        public double freqlivres { get; set; }
        public string infover { get; set; }
        public int nbhomogr { get; set; }
        public int nbhomoph { get; set; }
        public bool islem { get; set; }
        public int nblettres { get; set; }
        public int nbphons { get; set; }
        public string cvcv { get; set; }
        public string p_cvcv { get; set; }
        public int voisorth { get; set; }
        public int voisphon { get; set; }
        public int puorth { get; set; }
        public int puphon { get; set; }
        public string syll { get; set; }
        public int nbsyll { get; set; }
        public string cv_cv {get;set;}
        public string orthrenv { get; set; }
        public string phonrenv { get; set; }
        public string orthosyll { get; set; }
        public string cgramortho { get; set; }
        public int deflem { get; set; }
        public int defobs { get; set; }
        public double old20 { get; set; }
        public double pld20 { get; set; }
        public string morphoder { get; set; }
        public int nbmorph { get; set; }
    }
}
