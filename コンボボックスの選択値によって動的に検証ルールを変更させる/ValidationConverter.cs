using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace コンボボックスの選択値によって動的に検証ルールを変更させる {
    public class ValidationConverter : System.Windows.Data.IValueConverter {
        private readonly string _pattern;

        public ValidationConverter(string pattern) {
            _pattern = pattern;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var strValue = value as string;
            if (!string.IsNullOrEmpty(strValue)) {
                if (!System.Text.RegularExpressions.Regex.IsMatch(strValue, _pattern)) {
                    return DependencyProperty.UnsetValue;
                }
            }
            return value;
        }
    }
}
