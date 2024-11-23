using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using HPBingoCounter.Types;

namespace HPBingoCounter.Coverters
{
    internal class EnumToBorderThicknessConverter : MarkupExtension, IValueConverter
    {
        #region MarkupExtension overrides
        static EnumToBorderThicknessConverter()
        {
            Instance = new EnumToBorderThicknessConverter();
        }

        public static EnumToBorderThicknessConverter Instance { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not PlayerColors color)
                throw new ArgumentException("value must be a PlayerColors value");

            if (!Enum.TryParse(parameter?.ToString(), out PlayerColors expectedColor))
                throw new ArgumentException("parameter must be a PlayerColors value");

            return color.Equals(expectedColor) ? 3 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
