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
        public bool TryGetRelationCount(DbWord word)
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
                        "(SELECT `wordSource`, count(`relationid`) as hyperonymCount FROM `relation` WHERE `relationGroup` = 'hyperonym') as r" +
                        "ON w.wordid = r.wordSource" +
                        "WHERE `word` = @Word";//da big request ever

                    cnn.Query<DbWord>(sql, word);
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

    }
}