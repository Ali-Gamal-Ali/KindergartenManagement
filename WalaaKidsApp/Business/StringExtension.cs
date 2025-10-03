using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace WalaaKidsApp.Business
{
    public static class StringExtension
    {
        public static bool IsValidNationalId(this string input)
        {
            if (input.Length != 14 || !input.All(char.IsDigit))
                return false;

            if (input[0] != '2' && input[0] != '3') return false;

            string year = input.Substring(1, 2);
            string month = input.Substring(3, 2);
            string day = input.Substring(5, 2);
            string fullYear = (input[0] == '2' ? "19" : "20") + year;

            if (!DateTime.TryParse($"{fullYear}-{month}-{day}", out _))
                return false;

            return true;
        }
        public static bool IsValidName(this string input)
        {
            return Regex.IsMatch(input, @"^[a-z A-Z\u0600-\u06FF]+$");
        }
    }
}
