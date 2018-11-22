namespace DW.ELA.Utility.Extensions
{
    using System.Collections.Generic;

    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return default(TValue);
        }
    }
}
