using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using HPBingoCounter.Types;

namespace HPBingoCounter.Coverters
{
    internal class TextColorByCompletionConverter : MarkupExtension, IMultiValueConverter
    {
        #region MarkupExtension overrides
        static TextColorByCompletionConverter()
        {
            Instance = new TextColorByCompletionConverter();
        }

        public static TextColorByCompletionConverter Instance { get; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
        #endregion

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(values);
            if (values[0] is not bool status)
                throw new ArgumentException("value[0] must be a bool");

            if (values[1] is not PlayerColors playerColor)
                throw new ArgumentException("value[1] must be a PlayerColors value");

            return status && playerColor.Equals(PlayerColors.Navy) ? Brushes.White : Brushes.Black;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
