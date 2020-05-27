using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Lexicotron.Database;
using Lexicotron.Database.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lexicotron.Tests.TestsDB
{
    [TestClass]
    public class TestDBWord
    {
        DbWord defaultDbWord;
        DbWord defaultDbWord2;
        DbRelation defaultRelation;
        public TestDBWord()
        {
            //_primeService = new PrimeService();
            defaultDbWord = new DbWord { Word = "test", SynsetId = "b:fzf4687", CreationDate = DateTime.Today };
            defaultDbWord2 = new DbWord { Word = "test2", SynsetId = "b:vdqvqdv45", CreationDate = DateTime.Today };
            defaultRelation = new DbRelation { WordSourceId = 1, WordTargetId = 2, RelationGroup = "Hyponym", TargetSynsetId = defaultDbWord2.SynsetId, CreationDate = DateTime.Today };

        }
       
        [TestMethod]
        public void TestAddWordFromWord()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();
            //act-assert
            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();
            //restore
            sut.DeleteDatabase();
        }
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void TestAddWordFromWordMultiple(int count)
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();
            List<DbWord> wordlist = new List<DbWord>();

            DbWord word;
            for (int i = 0; i < count; i++)
            {
                word = new DbWord
                {
                    Word = Guid.NewGuid().ToString(),
                    SynsetId = Guid.NewGuid().ToString(),
                    CreationDate = DateTime.Today
                };
                
                wordlist.Add(word);
            }

            //act-assert
            for (int i = 0; i < count; i++)
            {
                sut.TryAddWord(wordlist[i].Word, wordlist[i].SynsetId).Should().BeTrue();
            }

            int countCheck = 0;
            foreach (DbWord word2 in wordlist)
            {
                sut.TryGetWord(word2.Word, out DbWord wordOut).Should().BeTrue();
                countCheck++;
            }

            countCheck.Should().Be(wordlist.Count);

            //restore
            sut.DeleteDatabase();
        }
        [TestMethod]
        public void TestSelectWordFromWord()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();
            //act-assert
            sut.TryGetWord(defaultDbWord.Word, out DbWord data).Should().BeFalse();

            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();

            sut.TryGetWord(defaultDbWord.Word, out DbWord data2).Should().BeTrue();

            data2.Should().Be(defaultDbWord);

            
            //restore
            sut.DeleteDatabase();
        }
        [TestMethod]
        public void TestSelectWordFromSynset()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();
            //act-assert
            sut.TryGetWord(defaultDbWord.Word, out DbWord data).Should().BeFalse();

            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();

            sut.TryGetWord(defaultDbWord.SynsetId, out DbWord data2,true).Should().BeTrue();

            data2.Should().Be(defaultDbWord);


            //restore
            sut.DeleteDatabase();
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void TestMultipleWordInsertion(int count)
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();
            List<DbWord> wordlist = new List<DbWord>();

            for (int i = 0; i < count; i++)
            {
                wordlist.Add(new DbWord { 
                    Word = Guid.NewGuid().ToString(), 
                    SynsetId = Guid.NewGuid().ToString(),
                    CreationDate = DateTime.Today 
                });
            }

            //act-assert
            sut.TryAddWords(wordlist).Should().Be(count);

            int countCheck=0;
            foreach (DbWord word in wordlist)
            {
                sut.TryGetWord(word.Word, out DbWord wordOut).Should().BeTrue();
                countCheck++;
            }

            countCheck.Should().Be(wordlist.Count);

            //restore
            sut.DeleteDatabase();
        }
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void TestWordCount(int count)
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();
            List<DbWord> wordlist = new List<DbWord>();

            for (int i = 0; i < count; i++)
            {
                wordlist.Add(new DbWord
                {
                    Word = Guid.NewGuid().ToString(),
                    SynsetId = Guid.NewGuid().ToString(),
                    CreationDate = DateTime.Today
                });
            }
            //act-assert
            sut.TryAddWords(wordlist).Should().Be(count);

            sut.GetWordCount().Should().Be(wordlist.Count);

            //restore
            sut.DeleteDatabase();
        }
        [TestMethod]
        public void TestMultipleSimilarWordInsertion()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();
            List<DbWord> wordlist = new List<DbWord>();
            List<DbWord> wordlist2 = new List<DbWord>();

            wordlist.Add(defaultDbWord);
            wordlist.Add(defaultDbWord2);


            //act-assert
            sut.TryAddWords(wordlist).Should().Be(wordlist.Count);


            wordlist2.Add(defaultDbWord);
            sut.TryAddWords(wordlist2).Should().Be(0, "the wordlist 2 should'nt be inserted");

            //restore
            sut.DeleteDatabase();
        }

        [TestMethod]
        public void TestSelectManyWords()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();

            sut.CreateDatabase();

            List<DbWord> wordlist = new List<DbWord>
            {
                defaultDbWord,
                defaultDbWord2
            };

            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();
            sut.TryAddWord(defaultDbWord2.Word, defaultDbWord2.SynsetId).Should().BeTrue();
            //act-assert


            sut.TryGetWords(wordlist, out IEnumerable<DbWord> data2);

            DbWord[] wordSelected = data2.ToArray();

            for (int i = 0; i < wordlist.Count; i++)
            {
                wordSelected[i].Should().Be(wordlist[i]);
            }

            //restore
            sut.DeleteDatabase();
        }
        [DataTestMethod]
        [DataRow(1000)]
        [DataRow(2000)]
        [DataRow(100)]
        public void TestSelectManyManyMoreWords(int argsize)
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();

            sut.CreateDatabase();

            List<DbWord> wordlist = new List<DbWord>();
            for (int i = 0; i < argsize; i++)
            {
                wordlist.Add(new DbWord { Word = Guid.NewGuid().ToString(), SynsetId = Guid.NewGuid().ToString(), CreationDate = DateTime.Today });
            }
            wordlist.Add(defaultDbWord);
            wordlist.Add(defaultDbWord2);

            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();
            sut.TryAddWord(defaultDbWord2.Word, defaultDbWord2.SynsetId).Should().BeTrue();


            //act
            sut.TryGetWords(wordlist, out IEnumerable<DbWord> data2).Should().BeTrue();
            //assert
            var wordOut = data2.ToHashSet();
            wordOut.Contains(defaultDbWord).Should().BeTrue();
            wordOut.Contains(defaultDbWord2).Should().BeTrue();
            //restore
            sut.DeleteDatabase();
        }
        [TestMethod]
        public void TestSelectWordsWithoutSynset()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();

            sut.CreateDatabase();

            List<DbWord> wordlist = new List<DbWord>
            {
                defaultDbWord,
                defaultDbWord2,
                new DbWord { Word = "test3", SynsetId = null, CreationDate = DateTime.Today }
            };

            foreach (DbWord word in wordlist)
            {
                sut.TryAddWord(word.Word, word.SynsetId).Should().BeTrue();
            }
            //act-assert


            sut.TryGetWordsWithoutSynset(1000, out IEnumerable<DbWord> outdbWords).Should().Be(1);

            outdbWords.First().Should().Be(wordlist.Last());

            //restore
            sut.DeleteDatabase();
        }
        [TestMethod]
        public void TestUpdateManyWords()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();

            sut.CreateDatabase();

            List<DbWord> wordlist = new List<DbWord>
            {
                defaultDbWord,
                defaultDbWord2
            };

            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();
            sut.TryAddWord(defaultDbWord2.Word, defaultDbWord2.SynsetId).Should().BeTrue();

            var updatedSynset = "b:04704254";

            List<DbWord> wordlist2 = new List<DbWord>
            {
                new DbWord { Word = "test", SynsetId =updatedSynset, CreationDate = DateTime.Today }
            };

            //act
            sut.TryUpdateDbWords(wordlist2).Should().Be(1);

            //assert

            sut.TryGetWords(wordlist, out IEnumerable<DbWord> outDbWords).Should().BeTrue();

            outDbWords.Count().Should().Be(wordlist.Count);

            outDbWords.ToList().Where(x => x.Word == "test").First().SynsetId.Should().Be(updatedSynset);
            

            //restore
            sut.DeleteDatabase();
        }
        [DataTestMethod]
        [DataRow(10)]
        [DataRow(100)]
        public void TestWordCompletedCount(int count)
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();
            List<DbWord> wordlist = new List<DbWord>();

            int nullSynsetCount = 0;
            string synset=null;

            for (int i = 0; i < count; i++)
            {
                if(i % 2 == 0)//only even
                {
                    synset = Guid.NewGuid().ToString();
                    nullSynsetCount++;
                }

                wordlist.Add(new DbWord
                {
                    Word = Guid.NewGuid().ToString(),
                    SynsetId = synset, 
                    CreationDate = DateTime.Today
                });

                synset = null;
            }

            sut.TryAddWords(wordlist, true).Should().Be(count);

            //act-assert
            sut.GetWordsNotCompletedCount(count).Should().Be(nullSynsetCount);

            //restore
            sut.DeleteDatabase();
        }
    }
}

/*
 [DataTestMethod]
[DataRow(-1)]
[DataRow(0)]
[DataRow(1)]
*/
