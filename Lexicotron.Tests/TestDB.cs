using System;
using System.IO;
using FluentAssertions;
using Lexicotron.Database;
using Lexicotron.Database.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lexicotron.Tests
{
    [TestClass]
    public class TestDB
    {
        DbWord defaultDbWord;
        DbWord defaultDbWord2;
        DbRelation defaultRelation;
        public TestDB()
        {
            //_primeService = new PrimeService();
            defaultDbWord = new DbWord { Word = "test", SynsetId = "b:fzf4687", CreationDate = DateTime.Today };
            defaultDbWord2 = new DbWord { Word = "test2", SynsetId = "b:vdqvqdv45", CreationDate = DateTime.Today };
            defaultRelation = new DbRelation { WordSourceId = 1, WordTargetId = 2, RelationGroup = "Hyponym", TargetSynsetId = defaultDbWord2.SynsetId, CreationDate = DateTime.Today };

            }
        [TestMethod]
        public void TestDatabaseCreation()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            //act
            sut.CreateDatabase();
            //assert
            File.Exists(LocalWordDB.DbFile).Should().BeTrue();
            //restore
            sut.DeleteDatabase();
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
        [TestMethod]
        public void TestAddRelationFromRelation()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();

            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();
            sut.TryAddWord(defaultDbWord2.Word, defaultDbWord2.SynsetId).Should().BeTrue();
            //act-assert
            sut.TryAddRelation(defaultRelation).Should().BeTrue();
            //restore
            sut.DeleteDatabase();
        }
        [TestMethod]
        public void TestAddRelationFromWordInfo()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();

            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();
            sut.TryAddWord(defaultDbWord2.Word, defaultDbWord2.SynsetId).Should().BeTrue();
            //act-assert
            sut.TryAddRelation(defaultDbWord2.Word, defaultDbWord2.SynsetId, "Hyperonym").Should().BeTrue();
            //restore
            sut.DeleteDatabase();
        }
        [TestMethod]
        public void TestAddRelationToEmpty()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();

            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();
            sut.TryAddWord(defaultDbWord2.Word, defaultDbWord2.SynsetId).Should().BeTrue();
            //act-assert
            sut.TryAddRelation(defaultDbWord2.Word, "b:578764644", "Hyperonym").Should().BeTrue();
            sut.SynsetIdToSearch.Count.Should().Be(1);
            //restore
            sut.DeleteDatabase();
        }
        [TestMethod]
        public void TestAddLog()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();

            sut.TryAddLog("b:576847","{'hdjkh':'result'}").Should().BeTrue();
            //act-assert
            //restore
            sut.DeleteDatabase();
        }
        [TestMethod]
        public void TestTodayCountLog()
        {
            //arrange
            var date = DateTime.Now;
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();

            sut.TryAddLog("b:576847", "{'hdjkh':'result'}").Should().BeTrue();
            //act-assert
            sut.GetBabelRequestsCount(date).Should().Be(1);
            //restore
            sut.DeleteDatabase();
        }

        [DataTestMethod]
        [DataRow(2019,01,01)]
        [DataRow(1902,08, 24)]
        public void TestNotTodayCountLog(int year, int month, int day)
        {
            //arrange
            var date = new DateTime(year, month, day);
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();

            sut.TryAddLog("b:576847", "{'hdjkh':'result'}").Should().BeTrue();
            //act-assert
            sut.GetBabelRequestsCount(date).Should().Be(0);
            //restore
            sut.DeleteDatabase();
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(10)]
        public void TestTodayMultipleCountLog(int count)
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();
            for (int i = 0; i < count; i++)
            {
                sut.TryAddLog(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()).Should().BeTrue();
            }

            //act-assert
            sut.GetTodayBabelRequestsCount().Should().Be(count);
            
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
