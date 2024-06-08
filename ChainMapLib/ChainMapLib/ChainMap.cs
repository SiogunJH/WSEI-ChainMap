using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ChainMapLib
{
    public class ChainMap<TKey, TValue> : IEnumerable where TKey : IEquatable<TKey>
    {
        #region Variables & Properties

        Dictionary<TKey, TValue> mainDict = new();
        List<Dictionary<TKey, TValue>> subDicts = new();

        #endregion

        #region Constructors

        public ChainMap()
        {

        }

        public ChainMap(params Dictionary<TKey, TValue>[] additionalDicts) : this()
        {
            subDicts = additionalDicts.ToList();
        }

        #endregion

        #region IDictionary Partial

        public TValue this[TKey key]
        {
            get
            {
                if (!Keys.Contains(key)) throw new KeyNotFoundException();

                if (mainDict.ContainsKey(key)) return mainDict[key];

                foreach (var dict in subDicts)
                    if (dict.ContainsKey(key))
                        return dict[key];

                throw new InvalidOperationException("This code path should not be reached.");
            }
            set
            {
                if (!Keys.Contains(key)) throw new KeyNotFoundException();

                if (mainDict.ContainsKey(key)) mainDict[key] = value;
                else mainDict.Add(key, value);
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                HashSet<TKey> result = new HashSet<TKey>(mainDict.Keys);

                foreach (var dict in subDicts)
                    foreach (var key in dict.Keys)
                        if (!result.Contains(key))
                            result.Add(key);

                return result;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                HashSet<TValue> result = new();

                foreach (var key in Keys)
                    result.Add(this[key]);

                return result;
            }
        }

        public int Count => Keys.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            // prevent adding existing keys
            if (mainDict.ContainsKey(key)) throw new ArgumentException($"Attempt to add an existing key ({key}) was aborted.");

            // add new rule to main dict
            mainDict.Add(key, value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            // prevent adding existing keys
            if (mainDict.ContainsKey(key)) return false;

            // add new rule to main dict
            mainDict.Add(key, value);
            return true;
        }

        public void Clear() => mainDict.Clear();

        // Values that are inaccessible at the moment will not be taken into the account
        public bool ContainsValue(TValue item) => Values.Contains(item);

        public bool ContainsKey(TKey key) => Keys.Contains(key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in Keys)
                yield return new KeyValuePair<TKey, TValue>(key, this[key]);
        }

        public bool Remove(TKey key) => mainDict.Remove(key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (mainDict.TryGetValue(key, out value))
                return true;

            foreach (var dict in subDicts)
                if (dict.TryGetValue(key, out value))
                    return true;

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region SubDicts Operations

        public int CountDictionaries { get => subDicts.Count + 1; }

        // Has conflicts with documentation
        public void AddDictionary(IDictionary<TKey, TValue> dictionary, int index)
        {
            /*/ 
            |*| Documentation suggests that if [index] is out of bounds, the [dict] should be saved at incorrect ends of the [subDicts] list.
            |*| 
            |*| Example: An [index] of [0] would result in [dict] inserion on position [0], which has the highest priority,
            |*| yet the index of [-1] or less would result in [dict] insertion on last position, which has the lowest priority.
            |*| Same logic applies to an [index] greater than the length of a [subDicts] list.
            |*| 
            |*| I find it a bit unintuitive, so in this implementation [index] is clamped to not be out of bounds, so values of [-1] and less will be treated as [0]
            |*| and values larger than [subDicts] list count, will be treated as [subDicts] list count.
            /*/

            index = Math.Clamp(index, 0, subDicts.Count);
            subDicts.Insert(index, (Dictionary<TKey, TValue>)dictionary);
        }

        public void RemoveDictionary(int index)
        {
            if (index < 0 || index >= subDicts.Count) return;

            subDicts.RemoveAt(index);
        }

        public void ClearDictionaries() => subDicts.Clear();

        // Has conflicts with documentation
        public void ClearMainDictionary()
        {
            /*/
            |*| This function is not present in the documentation, and is expected to be a part of ClearDictionaries
            |*|
            |*| It seems unintuitive to have two functions to access dictionaries (GetDict and GetMainDict)
            |*| but only one to clear them.
            /*/

            mainDict.Clear();
        }

        public IReadOnlyList<Dictionary<TKey, TValue>> GetDictionaries() => subDicts.AsReadOnly();

        public IReadOnlyDictionary<TKey, TValue> GetDictionary(int index)
        {
            if (index < 0 || index >= subDicts.Count) throw new IndexOutOfRangeException();

            return new ReadOnlyDictionary<TKey, TValue>(subDicts[index]);
        }

        public IReadOnlyDictionary<TKey, TValue> GetMainDictionary() => new ReadOnlyDictionary<TKey, TValue>(mainDict);

        public ChainMap<TKey, TValue> Merge()
        {
            Dictionary<TKey, TValue> mergedDict = new();

            foreach (var key in Keys)
                mergedDict.Add(key, this[key]);

            return new ChainMap<TKey, TValue>(mergedDict);
        }

        #endregion
    }
}