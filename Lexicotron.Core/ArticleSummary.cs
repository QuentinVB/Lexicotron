using System;
using System.Collections.Generic;
using System.Text;

namespace Lexicotron.Core
{
    public class ArticleSummary
    {
        public string FileName { get; set; }
        public int TotalWordCount { get; set; }
        public int DistinctWordCount { get; set; }

    }
}
