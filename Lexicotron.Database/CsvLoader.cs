using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Lexicotron.Database
{
    public class CsvLoader
    {
        public static List<Word> LoadCSV()
        {
            List<Word> words;
            CsvHelper.Configuration.CsvConfiguration config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture);
            config.Delimiter = ";";

            using (var reader = new StreamReader("Lexique383.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<Word>();
                words = new List<Word>(records);
            }
            return words;
        }
    }
}
