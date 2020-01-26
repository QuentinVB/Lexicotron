using CsvHelper;
using System;
using System.Globalization;
using System.IO;

namespace Lexicotron.Database
{
    public class CsvLoader
    {
        public void LoadCsV()
        {
            using (var reader = new StreamReader("Lexique383.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Word>();
                /*
                 
             */
            }
        }
    }
}
