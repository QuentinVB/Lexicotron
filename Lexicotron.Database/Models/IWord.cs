using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicotron.Database.Models
{
    public interface IWord: IEquatable<IWord>
    {
        string Word { get; set; }
    }
}
