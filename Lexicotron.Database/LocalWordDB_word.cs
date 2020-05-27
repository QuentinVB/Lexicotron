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
    public partial class LocalWordDB
    {
        //TODO : split into 2 methods instead of selector calling the sub method
        /// <summary>
        /// Try to get a single word from the database using a selector
        /// </summary>
        /// <param name="selector">a word or a synset</param>
        /// <param name="dbWord">a dbword passed to/by the caller by reference</param>
        /// <param name="fromSynset">selector mode, if true the selector must be a synset</param>
        /// <returns>True if the word was found, else false</returns>
        public bool TryGetWord(string selector, out DbWord dbWord, bool fromSynset = false)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (string.IsNullOrWhiteSpace(selector)) throw new ArgumentNullException(nameof(selector));

            dbWord = new DbWord();

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                cnn.Open();
                string sql = "SELECT * FROM `word` WHERE" + (fromSynset ? "`synsetId` = @selector" : "`word` = @selector") + ";";

                var wordData = cnn.Query<DbWord>(sql, new { Selector = selector });

                if (wordData.Count() > 0)
                {
                    dbWord = wordData.First();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// try to get words from the database using an enumerable of <see cref="IWord"/>
        /// </summary>
        /// <param name="words">the words to find</param>
        /// <param name="outdbWords">a dbword enumerable passed to/by the caller by reference</param>
        /// <returns></returns>
        public bool TryGetWords(IEnumerable<IWord> words, out IEnumerable<DbWord> outdbWords)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (words.Count() == 0) throw new InvalidDataException(nameof(words));

            IEnumerable<DbWord> FindWords(IEnumerable<IWord> wordsToFind)
            {
                using (SQLiteConnection cnn = SimpleDbConnection())
                {
                    cnn.Open();
                    string sql = "SELECT * FROM `word` WHERE `word` IN @wordlist;";

                    return cnn.Query<DbWord>(sql, new { wordlist = wordsToFind.Select(x => x.Word).ToArray() });
                }
            }

            //max words should be 999 according to SQLITE_MAX_VARIABLE_NUMBER https://www.sqlite.org/limits.html
            //this little trick split the request into n sub requests if necessary
            int maxArgs = 900;
            if (words.Count() > maxArgs)
            {
                IEnumerable<List<IWord>> lots = SplitList(words.ToList(), maxArgs);
                List<DbWord> concatenedLots = new List<DbWord>();
                foreach (List<IWord> lot in lots)
                {
                    concatenedLots.AddRange(FindWords(lot));
                }

                outdbWords = concatenedLots;
                return true;
            }
            else
            {
                outdbWords = FindWords(words);
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


        public bool TryAddWord(string word, string synsetId)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (string.IsNullOrWhiteSpace(word) && string.IsNullOrWhiteSpace(synsetId)) throw new ArgumentNullException(nameof(word) + nameof(synsetId));

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="words">the enumeration of words to insert</param>
        /// <param name="safe">check if the words already exists in the database before insertion</param>
        /// <returns></returns>
        public int TryAddWords(IEnumerable<IWord> words, bool safe = true)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (words.Count() == 0) throw new InvalidOperationException(nameof(words) + " is empty");

            //TODO : Move the operation to the database
            //TODO : Add precheck to avoid double
            if (safe)
            {
                if (TryGetWords(words, out IEnumerable<DbWord> wordsFound))
                {
                    words = words.Except(wordsFound).ToList();
                    if (words.Count() == 0) return 0;
                }
            }
            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        //TODO : Cast IWord from DbWord to get synset

                        string sql = "";
                        if (words.FirstOrDefault() is DbWord)
                        {
                            sql = "INSERT INTO `word` (`word`, `synsetId`, `creationDate` ) VALUES( @Word,@SynsetId,date('now'));";

                        }
                        else
                        {
                            sql = "INSERT INTO `word` (`word`, `synsetId`, `creationDate` ) VALUES( @Word,null,date('now'));";

                        }


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
        public int GetWordWithoutSynsetCount()
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                //https://sql.sh/cours/insert-into
                string sql = "SELECT count(`wordid`) as Count FROM `word` WHERE (`word` IS NOT NULL AND `synsetId` IS NULL) ";

                return cnn.QueryFirst<int>(sql);
            }
        }
        public int GetWordsNotCompletedCount(int goal)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                //https://sql.sh/cours/insert-into
                string sql = "SELECT count(`wordid`) as Count " +
                    "FROM (SELECT `wordid`,`word`,`synsetId` FROM `word` LIMIT @goal) " +
                    "WHERE (`word` IS NOT NULL AND `synsetId` IS NULL ) ";

                return cnn.QueryFirst<int>(sql,new {goal});
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbwordToUpdateOrAdd">the words to update or add</param>
        /// <returns>a tuple with count of word inserted, then updated</returns>
        public (int, int) UpdateOrAddWordsWithSynset(HashSet<DbWord> dbwordToUpdateOrAdd)
        {
            HashSet<DbWord> wordAlreadyInDb;
            HashSet<DbWord> wordToAddInDb = new HashSet<DbWord>();
            HashSet<DbWord> dbwordToUpdate = new HashSet<DbWord>();

            //get words that are already into the database
            if (TryGetWords(dbwordToUpdateOrAdd, out IEnumerable<DbWord> dbwordalreadyindb))
            {
                wordAlreadyInDb = dbwordalreadyindb.ToHashSet();

                //TODO : Optimization
                //split dbwordto update : the words already in must be updated, the other must be added
                foreach (DbWord word in dbwordToUpdateOrAdd)
                {
                    //if word is already in db :  add to update
                    if (wordAlreadyInDb.Contains(word))
                    {
                        dbwordToUpdate.Add(word);
                    }
                    //else add to add

                    else
                    {
                        wordToAddInDb.Add(word);
                    }
                }
            }
            //update
            int wordupdated = (dbwordToUpdate.Count > 0) ? TryUpdateDbWords(dbwordToUpdate) : 0;
            int wordsinserted = (wordToAddInDb.Count > 0) ? TryAddWords(wordToAddInDb, false) : 0;
            return (wordsinserted, wordupdated);
        }

        public int TryUpdateDbWords(IEnumerable<DbWord> dbwordToUpdate)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (dbwordToUpdate == null) throw new ArgumentNullException();

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        string sql = "UPDATE `word` SET" +
                            "`synsetId` = @SynsetId," +
                            "`senseId` = @SenseId " +
                            "WHERE `word` = @Word";
                        int affectedRows = cnn.Execute(sql, dbwordToUpdate, transaction: transaction);

                        transaction.Commit();

                        if (affectedRows != dbwordToUpdate.Count()) throw new InvalidDataException();

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
    }
}
