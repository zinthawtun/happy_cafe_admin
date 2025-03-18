using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities
{
    public static class UniqueIdGenerator
    {
        private static readonly Random random = new Random();
        private static readonly Regex regex = new Regex("[A-Z0-9]{8}");

        public static string GenerateUniqueId()
        {
            string id;
            do
            {
                id = "UI" + new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (!regex.IsMatch(id));
            return id;
        }
    }
}
