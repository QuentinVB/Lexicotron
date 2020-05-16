using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace Lexicotron.Database
{
    public class DbWord
    {
        public int Id{get;set;}
        public string Word { get; set; }
        public string SynsetId { get; set; }
        /*
        Hypernym 
        Hyponym 
         */
        public int HyperonymCount { get; set; }
        public int HyponymCount { get; set; }
        public DateTime CreationDate { get; set; }

        public override bool Equals(object obj)
        {
            return obj is DbWord word &&
                   Id == word.Id &&
                   Word == word.Word &&
                   SynsetId == word.SynsetId &&
                   HyperonymCount == word.HyperonymCount &&
                   HyponymCount == word.HyponymCount &&
                   CreationDate == word.CreationDate;
        }
    }

    public class DbRelation
    {
        public int Id { get; set; }
        public int? WordSourceId { get; set; }
        public string RelationGroup { get; set; }
        public string TargetSynsetId { get; set; }
        public int? WordTargetId { get; set; }
        public DateTime CreationDate { get; set; }

        public DbWord WordSource { get; set; }
        public DbWord WordTarget { get; set; }

        /*
         "weight": 0,
        "normalizedWeight": 0
         */
    }
    public class LocalWordDB
    {
        HashSet<string> _synsetIdToSearch;
        public LocalWordDB()
        {
            _synsetIdToSearch = new HashSet<string>();
        }
        public static string DbFile
        {
            get { return Environment.CurrentDirectory + "\\wordDb.sqlite"; }
        }

        public HashSet<string> SynsetIdToSearch { get => _synsetIdToSearch; }

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
                    "`wordSourceid` INT NOT NULL, " +
                    "`relationGroup` TEXT NOT NULL, " +
                    "`targetSynsetid` TEXT NOT NULL, " +
                    "`wordTargetid` INT NULL, " +
                    "`creationDate` TEXT NULL, " +
                    "FOREIGN KEY(wordSourceid) REFERENCES word(wordid) ON DELETE SET NULL ON UPDATE CASCADE," +
                    "FOREIGN KEY(wordTargetid) REFERENCES word(wordid) ON DELETE SET NULL ON UPDATE CASCADE)");
            }
        }


        public void DeleteDatabase()
        {
            File.Delete(DbFile);
        }


        public bool TryGetWord(string word, out DbWord dbWord, bool fromSynset = false)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (string.IsNullOrWhiteSpace(word)) throw new ArgumentNullException(nameof(word));
            //if (dbWord == null) throw new ArgumentNullException(nameof(dbWord));
            dbWord = new DbWord();

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
         * TODO : a trygetwordS with batch requests
         */
        public bool TryAddWord(string word,string synsetId, bool fromSynset = false)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (string.IsNullOrWhiteSpace(word)) throw new ArgumentNullException(nameof(word));
            if (string.IsNullOrWhiteSpace(synsetId)) throw new ArgumentNullException(nameof(synsetId));

            //transform word : to lower ?

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                try
                {
                    //https://sql.sh/cours/insert-into
                    string sql = "INSERT INTO `word` (`word`,`synsetId`, `creationDate` ) VALUES( @Word,@SynsetId,date('now'));";

                    cnn.Query<DbWord>(sql, new { Word = word, SynsetId = synsetId });
                    return true;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627) 
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        /*
         * TODO : a tryAddwordS with transactions requests
         */
        public bool TryAddRelation(string wordSource, string synsetTargetId,string relationGroup)
        {
            if (!TryGetWord(wordSource, out DbWord dbwordsource)) throw new InvalidOperationException();
            if(!TryGetWord(synsetTargetId, out DbWord dbwordtarget,true))
            {
                _synsetIdToSearch.Add(synsetTargetId);
            }

            DbRelation relation = new DbRelation()
            {
                WordSourceId = dbwordsource.Id,
                WordSource = dbwordtarget,
                RelationGroup = relationGroup,
                TargetSynsetId = synsetTargetId,
                WordTargetId = dbwordtarget.Id,
                WordTarget = dbwordtarget
            };
            return TryAddRelation(relation);
        }
        public bool TryAddRelation(DbRelation relation)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (relation.WordSourceId == null ||relation.RelationGroup==null || relation.TargetSynsetId == null) throw new ArgumentNullException();


            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                try
                {
                    //https://sql.sh/cours/insert-into
                    string sql = "INSERT INTO `relation` (`wordSourceid`,`relationGroup`,`targetSynsetid`, `wordTargetid`,`creationDate` ) VALUES(@WordSourceId, @RelationGroup,@TargetSynsetId,@WordTargetId,date('now'));";

                    cnn.Query<DbWord>(sql, new {
                        relation.WordSourceId,
                        relation.RelationGroup,
                        relation.TargetSynsetId,
                        relation.WordTargetId
                    });
                    return true;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        return false;
                    }
                    return false;
                }
            }
        }
    }
}
