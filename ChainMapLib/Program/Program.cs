using ChainMapLib;

class Program
{
    static void Main()
    {
        var dict1 = new Dictionary<string, string>
        {
            { "a", "1" },
            { "b", "2" },
            { "c", "3" }
        };

        var dict2 = new Dictionary<string, string>
        {
            { "b", "22" },
            { "c", "33" },
            { "d", "44" }
        };

        var dict3 = new Dictionary<string, string>
        {
            { "c", "333" },
            { "d", "444" },
            { "e", "555" }
        };

        var chainMap = new ChainMap<string, string>(dict1, dict2, dict3);

        Console.WriteLine(chainMap["a"]); // 1
        Console.WriteLine(chainMap["b"]); // 2
        Console.WriteLine(chainMap["c"]); // 3
        Console.WriteLine(chainMap["d"]); // 44
        Console.WriteLine(chainMap["e"]); // 555

        // add to main dictionary
        chainMap["a"] = "11";
        chainMap["b"] = "22";
        chainMap["c"] = "33";
        chainMap["d"] = "44";
        chainMap["e"] = "55";

        Console.WriteLine(chainMap["a"]); // 11
        Console.WriteLine(chainMap["b"]); // 22
        Console.WriteLine(chainMap["c"]); // 33
        Console.WriteLine(chainMap["d"]); // 44
        Console.WriteLine(chainMap["e"]); // 55

        // remove from main dictionary
        chainMap.Remove("a");
        Console.WriteLine(chainMap["a"]); // 1

        chainMap.Add("f", "66");
        Console.WriteLine(chainMap["f"]); // 66

        chainMap.Remove("f");
        Console.WriteLine(chainMap.ContainsKey("f")); // False

        chainMap.AddDictionary(new Dictionary<string, string> { { "g", "77" } }, 0);
        Console.WriteLine(chainMap["g"]); // 77

        chainMap.RemoveDictionary(0);
        Console.WriteLine(chainMap.ContainsKey("g")); // False

        chainMap.ClearDictionaries();
        Console.WriteLine(chainMap.Count); // 4

        chainMap.ClearMainDictionary();
        Console.WriteLine(chainMap.Count); // 0
    }
}