using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HPBingoCounter.Coverters
{
    internal class ShowWhenTrue : MarkupExtension, IValueConverter
    {
        #region MarkupExtension overrides
        public static ShowWhenTrue Instance { get; }

        static ShowWhenTrue()
        {
            Instance = new ShowWhenTrue();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
