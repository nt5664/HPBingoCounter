using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HPBingoCounter.Coverters
{
    internal class ShowWhenFalse : MarkupExtension, IValueConverter
    {
        #region MarkupExtension overrides
        public static ShowWhenFalse Instance { get; }

        static ShowWhenFalse()
        {
            Instance = new ShowWhenFalse();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && !System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
