using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Dapper;

namespace Lexicotron.Database
{
    public class DbWord
    {
        public int Id{get;set;}
        public string Word { get; set; }
        public string SynsetId { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class DbRelation
    {
        public int Id { get; set; }
        public int WordSourceId { get; set; }
        public string RelationGroup { get; set; }
        public string TargetSynsetId { get; set; }
        public int WordTargetId { get; set; }
        public DateTime CreationDate { get; set; }

        public DbWord WordSource { get; set; }
        public DbWord WordTarget { get; set; }

    }
    public class LocalWordDB
    {

        public LocalWordDB()
        {

        }
        public static string DbFile
        {
            get { return Environment.CurrentDirectory + "\\network.sqlite"; }
        }

        public static SQLiteConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=" + DbFile);
        }

        //NOTE : SQLITE RowID is NOT 0 based !

        public void CreateDatabase()
        {
            using (var con = SimpleDbConnection())
            {
                con.Open();

                //words
                con.Execute("PRAGMA foreign_keys = ON");
                con.Execute("DROP TABLE IF EXISTS `word`");
                con.Execute("CREATE TABLE IF NOT EXISTS `word` (" +
                     "`wordid` INTEGER PRIMARY KEY NOT NULL, " +
                     "`word` TEXT UNIQUE NOT NULL, " +
                     "`synsetId` TEXT NULL, " +
                     "`creationDate` TEXT NULL)");
                //relations
                con.Execute("DROP TABLE IF EXISTS `relation`");
                con.Execute("CREATE TABLE IF NOT EXISTS `relation` (" +
                    "`relationid` INTEGER PRIMARY KEY NOT NULL, " +
                    "`wordSourceid` INT UNIQUE NOT NULL, " +
                    "`relationGroup` TEXT NOT NULL, " +
                    "`targetSynsetid` TEXT NOT NULL, " +
                    "`wordTargetid` INT UNIQUE NULL, " +
                    "`creationDate` TEXT NULL, " +
                    "FOREIGN KEY(wordSourceid) REFERENCES word(wordid) ON DELETE SET NULL ON UPDATE CASCADE," +
                    "FOREIGN KEY(wordTargetid) REFERENCES word(wordid) ON DELETE SET NULL ON UPDATE CASCADE");
            }
        }


        public bool TryGetWord(string word, ref DbWord dbWord, bool fromSynset = false)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (string.IsNullOrWhiteSpace(word)) throw new ArgumentNullException();
            if (dbWord == null) throw new ArgumentNullException();

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                cnn.Open();
                string sql = "SELECT * FROM `word` WHERE" + (fromSynset?"`synsetId` = @selector": "`word` = @selector")+";";

                var wordData = cnn.Query<DbWord>(sql, new { Selector = word });

                if(wordData.Count() > 0)
                {
                    dbWord = wordData.First();
                    return true;
                } 
            }

            return false;



        }
        /*
        public bool InsertWord(Layer layer, int depth)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

        }*/
    }
}
