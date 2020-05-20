using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace Lexicotron.Core
{
    public class FileWriter
    {

        /// <summary>
        /// LEGACY : Write down the article summary to a csv file
        /// </summary>
        /// <param name="articles">the article list to summarize</param>
        /// <param name="stamp">the timestamp used to name the file</param>
        public static void PrintArticlesSummaryToCSV(List<Article> articles, string stamp)
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
        /// LEGACY :Write down article words statistics to a csv file
        /// </summary>
        /// <param name="articles">the article to write</param>
        /// <param name="stamp">the timestamp used to name the file</param>
        public static void PrintArticlesToCSV(List<Article> articles, string stamp)
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

        public static void PrintArticlesToExcel(ArticleGroup articleGroup, string stamp)
        {
            Console.WriteLine($" Writing {stamp}-{articleGroup.Name}.xlsx");
            List<Article> articles = articleGroup.Articles;

            Excel.Application xlApp = new Excel.Application();

            string path = Helpers.GetExecutingDirectoryPath() + $"\\Output\\{stamp}-{articleGroup.Name}.xlsx";


            if (xlApp == null)
            {
                throw new DllNotFoundException("Excel is not correctly installed");
            }

            //add each output to one sheet => yes http://csharp.net-informations.com/excel/worksheet.htm

            Excel.Workbook xlWorkBook;

            object misValue = Missing.Value;
            xlWorkBook = xlApp.Workbooks.Add(misValue);

            Excel.Sheets worksheets = xlWorkBook.Worksheets;
            Excel.Worksheet xlWorkSheet;


            for (int articleIndex = 0; articleIndex < articles.Count; articleIndex++)
            {
                Article article = articles[articleIndex];
            

                List<WordProcessed> words = new List<WordProcessed>(article.Words.Values);

                //select first sheet
                xlWorkSheet = (articleIndex == 0)?
                    (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1)
                    : (Excel.Worksheet)worksheets.Add( Type.Missing, worksheets[articleIndex], Type.Missing, Type.Missing);

                //Max char is 31 remove Article-journal- before every filename
                xlWorkSheet.Name = article.Filename.Truncate(30);

                //print headers
                PropertyInfo[] wordProperties = typeof(WordProcessed).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < wordProperties.Length; i++)
                {
                    xlWorkSheet.Cells[1, i+1] = wordProperties[i].Name.ToString();
                }

                //print rows
                for (int j = 0; j < words.Count; j++)
                {
                    for (int k = 0; k < wordProperties.Length; k++)
                    {
                        xlWorkSheet.Cells[j+2, k+1] =
                            words[j]
                            .GetType()
                            .GetProperty(wordProperties[k].Name)
                            .GetValue(words[j], null) ?? "";
                    }
                }
                Marshal.ReleaseComObject(xlWorkSheet);
            }

            //Print summary

            //select add before first sheet
            xlWorkSheet = (Excel.Worksheet)worksheets.Add(worksheets[1], Type.Missing, Type.Missing, Type.Missing);
            xlWorkSheet.Name = "Summary";


            //print headers
            List<PropertyInfo> printableArticleProperties= new List<PropertyInfo>();
            foreach (PropertyInfo p in typeof(Article).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {

                System.Attribute[] attrs = System.Attribute.GetCustomAttributes(p);

                //TODO : rework with select where or any
                bool isPrintable = true;
                foreach (System.Attribute attr in attrs)
                {
                    if (attr is NonPrintable)
                    {
                        isPrintable = false;
                    }
                }
                if (isPrintable)
                {
                    printableArticleProperties.Add(p);
                }
            }
            for (int i = 0; i < printableArticleProperties.Count; i++)
            {
                xlWorkSheet.Cells[1, i + 1] = printableArticleProperties[i].Name.ToString();
            }

            //print rows
            for (int j = 0; j < articles.Count; j++)
            {
                for (int k = 0; k < printableArticleProperties.Count; k++)
                {
                    xlWorkSheet.Cells[j + 2, k + 1] =
                        articles[j]
                        .GetType()
                        .GetProperty(printableArticleProperties[k].Name)
                        .GetValue(articles[j], null) ?? "";
                }
            }
            Marshal.ReleaseComObject(xlWorkSheet);


            xlWorkBook.SaveAs(path, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            Marshal.ReleaseComObject(xlWorkBook);

            xlApp.Quit();

            
            Marshal.ReleaseComObject(xlApp);
            Console.WriteLine($" Finished writing !");

        }
    }
}
