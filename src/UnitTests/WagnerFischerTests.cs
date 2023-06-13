using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EditDistance;

namespace UnitTests
{
    [TestClass]
    public class WagnerFischerTests
    {

        [TestMethod]
        [DataRow("", "")] //Empty
        [DataRow("Something", "")] //One Empty
        [DataRow("", "Other")] //Other Empty
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmpty(string str1, string str2)
        {
            WagnerFischer wf = new WagnerFischer(str1, str2);
        }


        [TestMethod]
        [DataRow("Example1", "Example1", 0)] //Identical
        [DataRow("Example1", "Example", 1)]  //Deletion
        [DataRow("Example", "MyExample", 2)]  //Insertion at start
        [DataRow("ABCD", "aBCde", 3)] //Multiple + insertion at end
        [DataRow("A very long token with many changes", "A pretty long token with changes", 9)] //long with multiple
        public void TestLevenshteinDistance(string str1, string str2, int expectedDistance)
        {
            WagnerFischer wf = new WagnerFischer(str1, str2);
            wf.PopulateMatrix();
            Assert.AreEqual(expectedDistance, wf.EditDistance);
        }
    }
}