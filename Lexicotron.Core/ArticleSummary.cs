using System;
using System.Collections.Generic;
using System.Text;

namespace Lexicotron.Core
{
    /// <summary>
    /// Describe the summary of an article
    /// </summary>
    public class ArticleSummary
    {
        public string FileName { get; set; }
        public int TotalWordCount { get; set; }
        public int DistinctWordCount { get; set; }

    }
}
