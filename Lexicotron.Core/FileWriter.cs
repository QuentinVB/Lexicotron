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
        public static void PrintArticlesSummary(List<Article> articles, string stamp)
        {
            //new CultureInfo("fr-FR")

            using (var writer = new StreamWriter($".\\Output\\articleSummary-{stamp}.csv",true, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, new CultureInfo("fr-FR")))
            {
                csv.Configuration.Delimiter = ";";
                //csv.Configuration.Encoding = Encoding.UTF32;

                csv.WriteRecords(articles);
            }
        }

        public static void PrintArticles(List<Article> articles, string stamp)
        {
            foreach (Article article in articles)
            {
                //CultureInfo.InvariantCulture
                using (var writer = new StreamWriter($".\\Output\\article-{article.Filename}-{stamp}.csv",true, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, new CultureInfo("fr-FR")))
                {
                    csv.Configuration.Delimiter = ";";
                    //csv.Configuration.Encoding = Encoding.UTF32;
                    csv.WriteRecords(article.Words.Values);
                }
            }
            
        }
    }
}
