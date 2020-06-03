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
    //TODO : change db DAL to singleton ?

    /// <summary>
    /// Data Access Object to the local word db storing results from babel
    /// </summary>
    public partial class LocalWordDB
    {
        HashSet<string> _synsetIdToSearch;

        /// <summary>
        /// Initialize a new instance of the local word db data access object
        /// </summary>
        public LocalWordDB()
        {
            _synsetIdToSearch = new HashSet<string>();
            if (!File.Exists(DbFile)) CreateDatabase();

        }
        /// <summary>
        /// return the db file path
        /// </summary>
        public static string DbFile
        {
            get { return Environment.CurrentDirectory + "\\wordDb.sqlite"; }
        }

        public HashSet<string> SynsetIdToSearch { get => _synsetIdToSearch; }
        /// <summary>
        /// Create a SQLiteConnection object to the database
        /// </summary>
        /// <returns>a SQLiteConnection to the database</returns>
        public static SQLiteConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=" + DbFile);
        }

        //NOTE : 
        /// <summary>
        /// Recreate the database from scratch
        /// </summary>
        /// <remarks>
        /// SQLITE RowID is NOT 0 based ! <para />
        /// YYYY-MM-DD HH:MM:SS.SSS is sqlite time format
        /// </remarks>
        public void CreateDatabase()
        {
            using (var con = SimpleDbConnection())
            {
                con.Open();
                //

                //words
                con.Execute("PRAGMA foreign_keys = ON");
                con.Execute("DROP TABLE IF EXISTS `word`");
                con.Execute("CREATE TABLE IF NOT EXISTS `word` (" +
                     "`wordid` INTEGER PRIMARY KEY NOT NULL, " +
                     "`word` TEXT UNIQUE NULL, " +
                     "`senseId` TEXT UNIQUE NULL, " +
                     "`synsetId` TEXT NULL, " +
                     "`relationsRequested` INT NULL, " +
                     "`creationDate` TEXT NULL)");
                con.Execute("CREATE INDEX idx_word ON `word`(`word`) ");
                
                //relations

                con.Execute("DROP TABLE IF EXISTS `relation`");

                con.Execute("CREATE TABLE IF NOT EXISTS `relation` (" +
                    "`relationid` INTEGER NOT NULL, " +
                    "`wordSourceid` INT NOT NULL, " +
                    "`relationGroup` TEXT NOT NULL, " +
                    "`targetSynsetid` TEXT NOT NULL, " +
                    "`wordTargetid` INT NULL, " +
                    "`creationDate` TEXT NULL, " +
                    "PRIMARY KEY(`relationid`)," +
                    "FOREIGN KEY(`wordSourceid`) REFERENCES word(`wordid`) ON DELETE CASCADE ON UPDATE CASCADE," +
                    "FOREIGN KEY(`wordTargetid`) REFERENCES word(`wordid`) ON DELETE SET NULL ON UPDATE CASCADE)");
                con.Execute("CREATE INDEX idx_wordSourceid ON `relation`(`wordSourceid`) ");

                //log
                con.Execute("DROP TABLE IF EXISTS `babellog`");

                con.Execute("CREATE TABLE IF NOT EXISTS `babellog` (" +
                    "`id` INTEGER PRIMARY KEY NOT NULL, " +
                    "`requestDateTime` TEXT NOT NULL, " +
                    "`synsetRequested` TEXT NOT NULL, " +
                    "`jsonReturned` TEXT NOT NULL )");
            }
        }

        /// <summary>
        /// Delete the database file, ie. the database itself
        /// </summary>
        public void DeleteDatabase()
        {
            File.Delete(DbFile);
        }

        /// <summary>
        /// static method to split a list into n list of <paramref name="nSize"/> element
        /// </summary>
        /// <typeparam name="T">A ref or value type</typeparam>
        /// <param name="collection">The list to split</param>
        /// <param name="nSize">Elements per group</param>
        /// <returns>An enumerable of list containing nSize elements</returns>
        public static IEnumerable<List<T>> SplitList<T>(List<T> collection, int nSize = 30)
        {
            for (int i = 0; i < collection.Count; i += nSize)
            {
                yield return collection.GetRange(i, Math.Min(nSize, collection.Count - i));
            }
        }


    }
}
