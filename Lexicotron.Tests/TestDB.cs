using System;
using System.IO;
using FluentAssertions;
using Lexicotron.Database;
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
