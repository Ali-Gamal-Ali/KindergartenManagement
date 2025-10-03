using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalaaKidsApp.Business
{
    internal class EgyptianNationalIDAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var id = value as string;
            if (string.IsNullOrWhiteSpace(id))
                return true;
            if (id.Length != 14 || !id.All(char.IsDigit))
                return false;

            if (id[0] != '2' && id[0] != '3') return false;

            string year = id.Substring(1, 2);
            string month = id.Substring(3, 2);
            string day = id.Substring(5, 2);
            string fullYear = (id[0] == '2' ? "19" : "20") + year;

            if (!DateTime.TryParse($"{fullYear}-{month}-{day}", out _))
                return false;

            return true;
        }
    }
}
