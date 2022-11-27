using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BoardController
{
    internal static class Helper
    {
        public static string Serialize(this Dictionary<string, string> input)
        {
            return JsonSerializer.Serialize(input);
        }
    }

    /// <summary>
    ///   <br />
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>Merges the left.</summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="me">Me.</param>
        /// <param name="others">The others.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }

    }
}