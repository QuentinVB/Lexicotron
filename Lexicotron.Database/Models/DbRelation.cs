using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicotron.Database.Models
{
    public class DbRelation
    {
        public int Id { get; set; }
        public int? WordSourceId { get; set; }
        public string RelationGroup { get; set; }
        public string TargetSynsetId { get; set; }
        public int? WordTargetId { get; set; }
        public DateTime CreationDate { get; set; }

        public DbWord WordSource { get; set; }
        public DbWord WordTarget { get; set; }

        /*
         "weight": 0,
        "normalizedWeight": 0
         */
    }
}
