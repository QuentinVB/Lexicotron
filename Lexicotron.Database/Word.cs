using System;
using System.Collections.Generic;
using System.Text;

namespace Lexicotron.Database
{
    /// <summary>
    /// Describe a word from the french lexic database
    /// </summary>
    public class Word
    {
        //public int Id { get; set; }
        public string ortho { get; set; }
        public string lemme { get; set; }
        public string cgram { get; set; }
        public string genre { get; set; }
        public string nombre { get; set; }
        public double freqlemfilms { get; set; }
        public double freqlemlivres { get; set; }
        public double freqfilms { get; set; }
        public double freqlivres { get; set; }
        public bool islem { get; set; }
        public int nblettres { get; set; }
        public int deflem { get; set; }//connaissez vous le mot
        public int defobs { get; set; }
    }
}
