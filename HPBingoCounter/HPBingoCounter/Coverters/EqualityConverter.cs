using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace HPBingoCounter.Coverters
{
    internal class EqualityConverter : MarkupExtension, IMultiValueConverter
    {
        #region MarkupExtension overrides
        public static readonly EqualityConverter Instance;

        static EqualityConverter()
        {
            Instance = new EqualityConverter();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
        #endregion

        public object Convert(object[] values, Type targetTypes, object parameter, CultureInfo culture)
        {
            if (values is null || values.Length < 2)
                throw new NotSupportedException("There must be at least 2 values to compare");

            object? value = values[0];
            for (int i = 1; i < values.Length; ++i)
            {
                object? nextComparison = values[i];
                if (value?.GetType() != nextComparison?.GetType() || !(value?.Equals(nextComparison) ?? true))
                    return false;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
