using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicotron.Database.Models
{
    public class BabelLog
    {
        public string RequestedSynset { get; set; }
        public string JsonReturned { get; set; }
    }
}
