using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Lexicotron.Core
{
    public class FileWriter
    {
        public static void PrintArticlesSummary(List<Article> articles)
        {
            using (var writer = new StreamWriter($".\\Output\\articleSummary-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Configuration.Delimiter = ";";
                csv.WriteRecords(articles);
            }
        }
    }
}
