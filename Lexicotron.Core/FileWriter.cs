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
        /// <summary>
        /// Write down the article summary to a csv file
        /// </summary>
        /// <param name="articles">the article list to summarize</param>
        /// <param name="stamp">the timestamp used to name the file</param>
        public static void PrintArticlesSummary(List<Article> articles, string stamp)
        {
            //new CultureInfo("fr-FR")

            using (var writer = new StreamWriter($".\\Output\\{stamp}-articleSummary.csv",true, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, new CultureInfo("fr-FR")))
            {
                csv.Configuration.Delimiter = ";";
                //csv.Configuration.Encoding = Encoding.UTF32;

                csv.WriteRecords(articles);
            }
        }
        /// <summary>
        /// Write down article words statistics to a csv file
        /// </summary>
        /// <param name="articles">the article to write</param>
        /// <param name="stamp">the timestamp used to name the file</param>
        public static void PrintArticles(List<Article> articles, string stamp)
        {
            foreach (Article article in articles)
            {
                //CultureInfo.InvariantCulture
                using (var writer = new StreamWriter($".\\Output\\{stamp}-article-{article.Filename}.csv",true, Encoding.UTF8))
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
