using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WalaaKidsApp.Business.Converters
{
    public class AddStudentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = true;
            foreach(var obj in values)
            {
                if(obj == null) continue;
                if(obj is string textValue)
                {
                    if (string.IsNullOrWhiteSpace(textValue))
                        result = false;
                }
                if(obj is bool HasErrors)
                {
                    if(HasErrors)
                        result = false;
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
