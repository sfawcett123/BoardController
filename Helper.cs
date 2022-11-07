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
}
