using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Noc.Tests
{
    public class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Modified { get; set; }
    }

    [TestClass]
    public class UnitTest_Comparer
    {
        List<TestObject> list1 = new List<TestObject>
            {
                new TestObject {Id=1, Modified=new DateTime(2012,1,1,10,0,0), Name="SomeName" }
            };

        List<TestObject> list2 = new List<TestObject>
            {
                new TestObject {Id=1, Modified=new DateTime(2012,1,1,10,0,0), Name="SomeName"
            },
            {
                new TestObject {Id=2, Modified=new DateTime(2012,1,1,10,0,0), Name="SomeName" }
            }
        };
        List<TestObject> list3 = new List<TestObject>
            {
                new TestObject {Id=1, Modified=new DateTime(2015,1,1,10,0,0), Name="SomeName" }
            };

            
        [TestMethod]
        public void Test_Unaltered()
        {
            var c = Comparer.ListComparer<TestObject>(list1, list1, "Id", new List<string>().ToArray());

            Assert.IsTrue(c.Unaltered.Count == 1);
        }

        [TestMethod]
        public void Test_Add_one_object()
        {
            var c = Comparer.ListComparer<TestObject>(list1, list2, "Id", new List<string>().ToArray());

            Assert.IsTrue(c.Added.Count == 1);
        }

        [TestMethod]
        public void Test_Remove_one_object()
        {
            var c = Comparer.ListComparer<TestObject>(list2, list1, "Id", new List<string>().ToArray());

            Assert.IsTrue(c.Deleted.Count == 1);
        }
        
        [TestMethod]
        public void Test_Ignore_property()
        {
            var c = Comparer.ListComparer<TestObject>(list2, list1, "Id", ["Modified"]);

            Assert.IsTrue(c.Unaltered.Count == 1);
        }
    }
}
