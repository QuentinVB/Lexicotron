using Lexicotron.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexicotron.Database.Models;


namespace Lexicotron.Core
{

    internal class DALAdapter
    {
        readonly LocalWordDB _dal;

        public DALAdapter(LocalWordDB database)
        {
            _dal = database;
        }

        private LocalWordDB DAL => _dal;

        public int InsertWords(IEnumerable<WordProcessed> wordsToAdd)
        {
            //TODO : cast here !
            return DAL.TryAddWords(wordsToAdd);
        }

        public int GetWordCount()
        {
            return DAL.GetWordCount();           
        }

        //TODO : make a better proxy... the memory is awful here
        internal WordProcessed GetRelationCount(WordProcessed word)
        {
            DbWord returnedDbWord = DAL.TryGetRelationsSum(new DbWord() { Word = word.Word });
            word.HyperonymCount = returnedDbWord.HyperonymCount;
            word.HyponymCount = returnedDbWord.HyponymCount;
            word.OtherCount = returnedDbWord.OtherCount;
            return word;
        }
    }
}
