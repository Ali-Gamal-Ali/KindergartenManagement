using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WalaaKidsApp.Business
{
    internal class EgyptianPhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string[] validPrefixes = { "010", "011", "012", "015" };
            var phoneNumber = value as string;
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return true;
            if (phoneNumber.Length != 11 || !phoneNumber.All(char.IsDigit))
                return false;
            if (!validPrefixes.Any(phoneNumber.StartsWith))
                return false;
            return true;
        }
    }
}
