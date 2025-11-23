using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HPBingoCounter.Coverters
{
    internal class EnumToVisibilityConverter : MarkupExtension, IValueConverter
    {
        #region MarkupExtension overrides
        static EnumToVisibilityConverter()
        {
            Instance = new EnumToVisibilityConverter();
        }

        public static EnumToVisibilityConverter Instance { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value is not Enum || parameter is null)
                throw new ArgumentException("Both 'value' and 'parameter' cannot be null and 'value' must be an enum value");

            object? enumValue;
            if (!Enum.TryParse(value.GetType(), parameter.ToString(), out enumValue))
                return Visibility.Collapsed;

            return value.Equals(enumValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
