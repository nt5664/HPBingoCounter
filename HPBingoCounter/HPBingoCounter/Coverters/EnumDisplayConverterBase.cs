using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace HPBingoCounter.Coverters
{
    internal abstract class EnumDisplayConverterBase<T> : MarkupExtension, IValueConverter where T : Enum
    {
        protected static Dictionary<T, string>? Pairs { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Pairs is null)
                throw new InvalidOperationException("Pairs are not set up");

            if (value is null)
                return "NULL";

            if (value is not T mode)
                throw new ArgumentException($"The parameter must be {typeof(T).Name} value");

            return Pairs[mode];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Pairs is null)
                throw new InvalidOperationException("Pairs are not set up");

            try
            {
                return Pairs.Single(x => x.Value.Equals(value?.ToString())).Key;
            }
            catch
            {
                throw new ArgumentException($"The parameter must be the string equivalent of a {typeof(T).Name} value");
            }
        }
    }
}
