﻿using System;
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
    public class TestDBRelation
    {
        DbWord defaultDbWord;
        DbWord defaultDbWord2;
        DbRelation defaultRelation;
        public TestDBRelation()
        {
            //_primeService = new PrimeService();
            defaultDbWord = new DbWord { Word = "test", SynsetId = "b:fzf4687", CreationDate = DateTime.Today };
            defaultDbWord2 = new DbWord { Word = "test2", SynsetId = "b:vdqvqdv45", CreationDate = DateTime.Today };
            defaultRelation = new DbRelation { WordSourceId = 1, WordTargetId = 2, RelationGroup = "hypernym", TargetSynsetId = defaultDbWord2.SynsetId, CreationDate = DateTime.Today };
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
        public void TestRelationsOfWord()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();

            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();
            sut.TryAddWord(defaultDbWord2.Word, defaultDbWord2.SynsetId).Should().BeTrue();
            sut.TryAddRelation(defaultRelation).Should().BeTrue();

            //act-assert
            var wordreturned = sut.TryGetRelationsSum(defaultDbWord);
            wordreturned.Word.Should().Be(defaultDbWord.Word);
            wordreturned.HyperonymCount.Should().Be(1);
            wordreturned.HyponymCount.Should().Be(0);
            wordreturned.OtherCount.Should().Be(0);

            //no hyponym !!!

            //restore
            sut.DeleteDatabase();
        }

        [TestMethod]
        public void TestNoRelationsOfWord()
        {
            //arrange
            LocalWordDB sut = new LocalWordDB();
            sut.CreateDatabase();

            sut.TryAddWord(defaultDbWord.Word, defaultDbWord.SynsetId).Should().BeTrue();

            //act-assert
            var wordreturned = sut.TryGetRelationsSum(defaultDbWord);
            wordreturned.Word.Should().Be(defaultDbWord.Word);
            wordreturned.HyperonymCount.Should().Be(0);
            wordreturned.HyponymCount.Should().Be(0);
            wordreturned.OtherCount.Should().Be(0);

            //no hyponym !!!

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
