using ChainMapLib;

namespace ChainMapTests
{
    [TestClass]
    public class ChainMapTests
    {
        [TestMethod]
        public void TestAddAndRetrieveElements()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap.Add("key1", 1);
            chainMap.Add("key2", 2);

            Assert.AreEqual(1, chainMap["key1"]);
            Assert.AreEqual(2, chainMap["key2"]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddDuplicateKeyThrowsException()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap.Add("key1", 1);
            chainMap.Add("key1", 2);
        }

        [TestMethod]
        public void TestTryAdd()
        {
            var chainMap = new ChainMap<string, int>();
            bool added = chainMap.TryAdd("key1", 1);
            bool notAdded = chainMap.TryAdd("key1", 2);

            Assert.IsTrue(added);
            Assert.IsFalse(notAdded);
            Assert.AreEqual(1, chainMap["key1"]);
        }

        [TestMethod]
        public void TestContainsKeyAndValue()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap.Add("key1", 1);

            Assert.IsTrue(chainMap.ContainsKey("key1"));
            Assert.IsFalse(chainMap.ContainsKey("key2"));
            Assert.IsTrue(chainMap.ContainsValue(1));
            Assert.IsFalse(chainMap.ContainsValue(2));
        }

        [TestMethod]
        public void TestRemoveKey()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap.Add("key1", 1);
            chainMap.Add("key2", 2);

            bool removed = chainMap.Remove("key1");

            Assert.IsTrue(removed);
            Assert.IsFalse(chainMap.ContainsKey("key1"));
            Assert.IsTrue(chainMap.ContainsKey("key2"));
        }

        [TestMethod]
        public void TestClear()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap.Add("key1", 1);
            chainMap.Add("key2", 2);

            chainMap.Clear();

            Assert.AreEqual(0, chainMap.Count);
            Assert.IsFalse(chainMap.ContainsKey("key1"));
            Assert.IsFalse(chainMap.ContainsKey("key2"));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestIndexerKeyNotFoundException()
        {
            var chainMap = new ChainMap<string, int>();
            var value = chainMap["nonexistentKey"];
        }

        [TestMethod]
        public void TestKeysAndValues()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap.Add("key1", 1);
            chainMap.Add("key2", 2);

            var keys = chainMap.Keys;
            var values = chainMap.Values;

            CollectionAssert.AreEquivalent(new[] { "key1", "key2" }, keys.ToList());
            CollectionAssert.AreEquivalent(new[] { 1, 2 }, values.ToList());
        }

        [TestMethod]
        public void TestConstructorWithAdditionalDictionaries()
        {
            var dict1 = new Dictionary<string, int> { { "key1", 1 } };
            var dict2 = new Dictionary<string, int> { { "key2", 2 } };

            var chainMap = new ChainMap<string, int>(dict1, dict2);

            Assert.AreEqual(1, chainMap["key1"]);
            Assert.AreEqual(2, chainMap["key2"]);
        }

        [TestMethod]
        public void TestAddAndRemoveDictionary()
        {
            var dict1 = new Dictionary<string, int> { { "key1", 1 } };
            var chainMap = new ChainMap<string, int>(dict1);

            Assert.AreEqual(1, chainMap["key1"]);

            var dict2 = new Dictionary<string, int> { { "key2", 2 } };
            chainMap.AddDictionary(dict2, 1);

            Assert.AreEqual(2, chainMap["key2"]);

            chainMap.RemoveDictionary(1);

            Assert.ThrowsException<KeyNotFoundException>(() => chainMap["key2"]);
        }

        [TestMethod]
        public void TestMerge()
        {
            var dict1 = new Dictionary<string, int> { { "key1", 1 } };
            var dict2 = new Dictionary<string, int> { { "key2", 2 } };

            var chainMap = new ChainMap<string, int>(dict1, dict2);
            chainMap.Add("key3", 3);

            var merged = chainMap.Merge();

            Assert.AreEqual(1, merged["key1"]);
            Assert.AreEqual(2, merged["key2"]);
            Assert.AreEqual(3, merged["key3"]);
        }

        [TestMethod]
        public void TestCountDictionaries()
        {
            var dict1 = new Dictionary<string, int> { { "key1", 1 } };
            var dict2 = new Dictionary<string, int> { { "key2", 2 } };

            var chainMap = new ChainMap<string, int>(dict1, dict2);

            Assert.AreEqual(3, chainMap.CountDictionaries); // 2 sub dictionaries + 1 main dictionary
        }

        [TestMethod]
        public void TestClearMainDictionary()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap.Add("key1", 1);
            chainMap.Add("key2", 2);

            chainMap.ClearMainDictionary();

            Assert.AreEqual(0, chainMap.Count);
        }

        [TestMethod]
        public void TestTryGetValue()
        {
            var dict1 = new Dictionary<string, int> { { "key1", 1 } };
            var chainMap = new ChainMap<string, int>(dict1);
            chainMap.Add("key2", 2);

            bool result = chainMap.TryGetValue("key1", out int value1);
            Assert.IsTrue(result);
            Assert.AreEqual(1, value1);

            result = chainMap.TryGetValue("key3", out int value2);
            Assert.IsFalse(result);
            Assert.AreEqual(0, value2);
        }
    }
}
