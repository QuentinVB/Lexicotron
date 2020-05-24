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
    public class LocalWordDB
    {
        HashSet<string> _synsetIdToSearch;

        //TODO : change db DAL to singleton
        public LocalWordDB()
        {
            _synsetIdToSearch = new HashSet<string>();
            if (!File.Exists(DbFile)) CreateDatabase();

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
                     "`word` TEXT UNIQUE NULL, " +
                     "`senseId` TEXT UNIQUE NULL, " +
                     "`synsetId` TEXT UNIQUE NULL, " +
                     "`creationDate` TEXT NULL)");
                con.Execute("CREATE INDEX idx_word ON `word`(`word`) ");

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

                //relations
                con.Execute("DROP TABLE IF EXISTS `babellog`");

                con.Execute("CREATE TABLE IF NOT EXISTS `babellog` (" +
                    "`id` INTEGER PRIMARY KEY NOT NULL, " +
                    "`requestDateTime` TEXT NOT NULL, " +
                    "`synsetRequested` TEXT NOT NULL, " +
                    "`jsonReturned` TEXT NOT NULL )");
            }
        }


        public void DeleteDatabase()
        {
            File.Delete(DbFile);
        }


        public bool TryGetWord(string selector, out DbWord dbWord, bool fromSynset = false)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (string.IsNullOrWhiteSpace(selector)) throw new ArgumentNullException(nameof(selector));
            
            dbWord = new DbWord();

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                cnn.Open();
                string sql = "SELECT * FROM `word` WHERE" + (fromSynset?"`synsetId` = @selector": "`word` = @selector")+";";

                var wordData = cnn.Query<DbWord>(sql, new { Selector = selector });

                if(wordData.Count() > 0)
                {
                    dbWord = wordData.First();
                    return true;
                } 
            }
            return false;
        }

        public bool TryGetWords(IEnumerable<IWord> words, out IEnumerable<DbWord> outdbWords)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (words.Count()==0) throw new InvalidDataException(nameof(words));

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                cnn.Open();
                string sql = "SELECT * FROM `word` WHERE `word` IN @wordlist;";

                outdbWords = cnn.Query<DbWord>(sql, new { wordlist = words.Select(x => x.Word).ToArray() });
            

                return true;
            }
        }

        public int TryGetWordsWithoutSynset(int amount, out IEnumerable<DbWord> outdbWords)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                cnn.Open();
                string sql = "SELECT * FROM `word` WHERE (`word` IS NOT NULL AND `synsetId` IS NULL) LIMIT @amount;";

                outdbWords = cnn.Query<DbWord>(sql, new { amount });

                return outdbWords.Count();
            }
        }
        public bool TryAddWord(string word,string synsetId)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (string.IsNullOrWhiteSpace(word) && string.IsNullOrWhiteSpace(synsetId)) throw new ArgumentNullException(nameof(word)+ nameof(synsetId));

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
        public int TryAddWords(IEnumerable<IWord> words)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (words.Count()==0) throw new InvalidOperationException(nameof(words)+" is empty");

            //TODO : Move the operation to the database
            if(TryGetWords(words, out IEnumerable<DbWord> wordsFound))
            {
                words = words.Except(wordsFound).ToList();
                if (words.Count() == 0) return 0;
            }

            //TODO : Cast IWord from DbWord to get synset

            //TODO : transform word : to lower ?

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        string sql = "INSERT INTO `word` (`word`, `synsetId`, `creationDate` ) VALUES( @Word,null,date('now'));";
                        int affectedRows = cnn.Execute(sql, words, transaction: transaction);

                        transaction.Commit();

                        if (affectedRows != words.Count()) throw new InvalidDataException();

                        return affectedRows;
                    }
                    catch (Exception ex)
                    {
                        // roll the transaction back
                        if (ex is SqlException sqlex)
                        { 
                            if (sqlex.Number == 2601 || sqlex.Number == 2627)
                            {
                                return 0;
                            }
                        }
                        transaction.Rollback();

                        throw;
                    }
                }
            }
        }
        public bool TryAddRelation(string wordSource, string synsetTargetId,string relationGroup)
        {
            if (!TryGetWord(wordSource, out DbWord dbwordsource)) throw new InvalidOperationException();
            if(!TryGetWord(synsetTargetId, out DbWord dbwordtarget,true))
            {
                TryAddWord(null,synsetTargetId);
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
                    string sql = "INSERT INTO `relation` (`wordSourceid`,`relationGroup`,`targetSynsetid`, `wordTargetid`,`creationDate` ) VALUES(@WordSourceId, @RelationGroup,@TargetSynsetId,@WordTargetId,date('now', 'localtime'));";

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
                    throw;
                }
            }
        }

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


        public int GetWordCount()
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                //https://sql.sh/cours/insert-into
                string sql = "SELECT count(`wordid`) as Count FROM `word` ";

                return cnn.QueryFirst<int>(sql);
            }
        }

        public void UpdateOrAddWordsWithSynset(List<DbWord> dbwordToUpdate)
        {
            //get words that are already into the database
            if(TryGetWords(dbwordToUpdate,out IEnumerable<DbWord> dbwordalreadyindb))
            {
                var wordalreadyindb = dbwordalreadyindb.ToHashSet();

                //match synset

                //wordalreadyindb.
            }
            //split dbwordto update : the words already in must be updated, the other must be added
            throw new NotImplementedException();
        }
    }
}
