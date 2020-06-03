﻿using System;
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
                WordSourceId = dbwordsource.WordId,
                WordSource = dbwordtarget,
                RelationGroup = relationGroup,
                TargetSynsetId = synsetTargetId,
                WordTargetId = dbwordtarget.WordId,
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="words">the enumeration of relations to insert</param>
        /// <returns>Nomber of rows inserted</returns>
        public int TryAddRelations(IEnumerable<DbRelation> relations)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (relations.Count() == 0) throw new InvalidOperationException(nameof(relations) + " is empty");

           
            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        //TODO : Cast IWord from DbWord to get synset

                        string sql = "INSERT INTO `relation` (`wordSourceid`,`relationGroup`,`targetSynsetid`, `wordTargetid`,`creationDate` ) " +
                            "VALUES(@WordSourceId, @RelationGroup,@TargetSynsetId,@WordTargetId,date('now', 'localtime'));";

                        int affectedRows = cnn.Execute(sql, relations, transaction: transaction);

                        transaction.Commit();

                        if (affectedRows != relations.Count()) throw new InvalidDataException();

                        return affectedRows;
                    }
                    catch 
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public DbWord TryGetRelationCount(DbWord word)
        {
            if (!File.Exists(DbFile)) throw new FileNotFoundException("no database");
            if (word.Word == null) throw new ArgumentNullException();

            using (SQLiteConnection cnn = SimpleDbConnection())
            {
                try
                {
                    //https://sql.sh/cours/insert-into
                    string sql = "SELECT w.wordid, w.word, w.synsetId, w.creationDate, r.hyperonymCount FROM `word` AS w " +
                        "INNER JOIN" +
                        "(SELECT `wordSourceid`, count(`relationid`) as hyperonymCount FROM `relation` WHERE `relationGroup` = 'hypernym') AS r " +
                        "ON w.wordid = r.wordSourceid " +
                        "WHERE `word` = @Word";//da big request ever

                    DbWord returnedWord = cnn.QueryFirstOrDefault<DbWord>(sql, word);

                    if (returnedWord ==null || returnedWord.Word != word.Word) return word;
                    return returnedWord;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2601 || ex.Number == 2627)
                    {
                        throw;

                    }
                    throw;
                }
            }
        }

        

    }
}
