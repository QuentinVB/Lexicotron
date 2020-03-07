using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Lexicotron.Database
{
    public class CsvLoader
    {
        /// <summary>
        /// Load a CSV, in this case, the french lexic
        /// </summary>
        /// <returns></returns>
        public static List<Word> LoadCSV()
        {
            List<Word> words;
            //CsvHelper.Configuration.CsvConfiguration config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture);
            //config.Delimiter = ";";

            //TODO add path helper
            //new CultureInfo("fr-FR")
            //CultureInfo.InvariantCulture
            using (var reader = new StreamReader(@"Data\Lexique383.csv"))
            using (var csv = new CsvReader(reader, new CultureInfo("fr-FR")))
            {
                csv.Configuration.Delimiter = ";";

                var records = csv.GetRecords<Word>();
                //words= new Dictionary<string,Word>() fast access with dictionnary
                words = new List<Word>(records);
            }
            return words;
        }
    }
}
