using Lexicotron.Database;
using Lexicotron.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lexicotron.BabelAPI
{
    internal class DALAPIAdapter
    {
        readonly LocalWordDB _dal;

        public DALAPIAdapter(LocalWordDB database)
        {
            _dal = database;
        }

        private LocalWordDB DAL => _dal;

        public int InsertWords(IEnumerable<DbWord> wordsToAdd)
        {
            //TODO : cast here !
            return DAL.TryAddWords(wordsToAdd);
        }

        public IEnumerable<DbWord> GetWordsWithoutSynset(int amount)
        {
            //TODO : cast here !
            DAL.TryGetWordsWithoutSynset(amount, out IEnumerable <DbWord> wordsFound);
            return wordsFound;
        }

        internal int RequestsAvailable()
        {
            //TODO : put max request into const or config file
            return 1000-DAL.GetTodayBabelRequestsCount();
        }

        internal void LogBabelRequest(string synsetId, string json)
        {
            DAL.TryAddLog(synsetId, json);
        }
    }
}
