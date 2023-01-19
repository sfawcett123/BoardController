// ***********************************************************************
// Assembly         : BoardManager
// Author           : steve
// Created          : 12-30-2022
//
// Last Modified By : steve
// Last Modified On : 12-30-2022
// ***********************************************************************
// <copyright file="Helper.cs" company="BoardManager">
//     Steven Fawcett
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Text.Json;

namespace BoardManager
{
    /// <summary>
    /// Provides extensions to the Dictionary&lt;String,String&gt; type
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Serializes the input.
        /// </summary>
        /// <param name="input">Dictionary</param>
        /// <returns>System.String.</returns>
        public static string Serialize(this Dictionary<string, string> input)
        {
            return JsonSerializer.Serialize(input);
        }

        /// <summary>
        /// Merges from the left.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <typeparam name="K">Key</typeparam>
        /// <typeparam name="V">Value</typeparam>
        /// <param name="me">The master Dictionary</param>
        /// <param name="others">List of Dictionaries to Merge</param>
        /// <returns>T.</returns>
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others) where T : IDictionary<K, V>, new()
        {
            T newMap = new();
            foreach (IDictionary<K, V> src in new List<IDictionary<K, V>> { me }.Concat(others))
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