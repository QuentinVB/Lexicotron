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


    }
}
