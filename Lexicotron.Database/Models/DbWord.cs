using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicotron.Database.Models
{
#pragma warning disable CS0659 // Le type se substitue à Object.Equals(object o) mais pas à Object.GetHashCode()
    public class DbWord : IWord
#pragma warning restore CS0659 // Le type se substitue à Object.Equals(object o) mais pas à Object.GetHashCode()
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public string SynsetId { get; set; }
        /*
        Hypernym 
        Hyponym 
         */
        public int HyperonymCount { get; set; }
        public int HyponymCount { get; set; }
        public DateTime CreationDate { get; set; }


        public bool Equals(IWord other)
        {
            if (other is null)
                return false;

            return other is IWord && this.Word == other.Word;
        }
        public override bool Equals(object obj) => Equals(obj as IWord);
        public override int GetHashCode() => (Word).GetHashCode();


    }
}
