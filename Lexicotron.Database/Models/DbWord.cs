using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicotron.Database.Models
{
    public class DbWord : IWord
    {
        public int WordId { get; set; }
        public string Word { get; set; }
        public string SynsetId { get; set; }
        public string SenseId { get; set; }
        public bool RelationsRequested { get; set; }
        /*
        Hypernym 
        Hyponym 
         */
        public int HyperonymCount { get; set; }//number of words wich are this words (above)
        public int HyponymCount { get; set; } //number of words wich are a kind of this word (below)
        public int OtherCount { get; set; }

        public DateTime CreationDate { get; set; }

        public bool Equals(IWord other)
        {
            if (other is null)
                return false;

            return other is IWord && this.Word == other.Word;
        }
        public override bool Equals(object obj) => Equals(obj as IWord);
        public override int GetHashCode() => Word.GetHashCode();


    }
}
