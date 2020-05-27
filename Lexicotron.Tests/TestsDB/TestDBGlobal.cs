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
    public class TestDBGlobal
    {
        public TestDBGlobal()
        {
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
            File.Exists(LocalWordDB.DbFile).Should().BeFalse();

        }
    }
}

/*
 [DataTestMethod]
[DataRow(-1)]
[DataRow(0)]
[DataRow(1)]
*/
