using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace HPBingoCounter.Coverters
{
    internal class InvertBoolConverter : MarkupExtension, IValueConverter
    {
        public static InvertBoolConverter Instance { get; }

        static InvertBoolConverter()
        {
            Instance = new InvertBoolConverter();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Invert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Invert(value);
        }

        private static bool Invert(object value)
        {
            if (value is not bool)
                throw new ArgumentException("Value must be a bool");

            return !(bool)value;
        }
    }
}
