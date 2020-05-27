using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using Lexicotron.Database.Models;

namespace Lexicotron.Database
{
    //YYYY-MM-DD HH:MM:SS.SSS is sqlite time format
    public partial class LocalWordDB
    {
        public bool TryAddLog(string synsetRequested, string jsonReturned)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (synsetRequested == null || jsonReturned == null) throw new ArgumentNullException();

            using (SQLiteConnection cnn = SimpleDbConnection())
            {           
                //https://sql.sh/cours/insert-into
                string sql = "INSERT INTO `babellog` (`requestDateTime`,`synsetRequested`,`jsonReturned` ) " +
                    "VALUES(date('now', 'localtime'), @synsetRequested,@jsonReturned);";

                cnn.Query(sql, new
                {
                    synsetRequested,
                    jsonReturned
                });   
            }
            return true;
        }
        public int GetTodayBabelRequestsCount()
        {
            return GetBabelRequestsCount(DateTime.Now);
        }
        public int GetBabelRequestsCount(DateTime dateSelected)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (dateSelected == null ) throw new ArgumentNullException();

            using (SQLiteConnection cnn = SimpleDbConnection())
            {             
                //https://sql.sh/cours/insert-into
                string sql = "SELECT count(`requestDateTime`) as Count " +
                    "FROM `babellog` " +
                    "WHERE date(`requestDateTime`) = date(@dateSelected) " +
                    "GROUP BY date(@dateSelected)";
                return cnn.QueryFirstOrDefault<int>(sql, new
                {
                    dateSelected= dateSelected.ToString("o")
                }) ;
            }
        }

    }
}
