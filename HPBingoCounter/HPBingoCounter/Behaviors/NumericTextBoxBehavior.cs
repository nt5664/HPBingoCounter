using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace HPBingoCounter.Behaviors
{
    internal partial class NumericTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty MaxInputLengthProperty = 
            DependencyProperty.Register(nameof(MaxInputLength), typeof(int), typeof(NumericTextBoxBehavior), new PropertyMetadata(1, OnMaxInputLengthChanged));

        public static readonly DependencyProperty SelectAllTextOnFocusProperty =
            DependencyProperty.Register(nameof(SelectAllTextOnFocus), typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(false, OnSelectAllTextOnFocusChanged));

        private static void OnMaxInputLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericTextBoxBehavior behavior = (NumericTextBoxBehavior)d;
            if (behavior.AssociatedObject is null)
                return;

            behavior.AssociatedObject.MaxLength = (int)e.NewValue;
        }

        private static void OnSelectAllTextOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericTextBoxBehavior behavior = (NumericTextBoxBehavior)d;
            if (behavior.AssociatedObject is null)
                return;

            behavior.SetFocusHandler();
        }

        public int MaxInputLength 
        {
            get => (int)GetValue(MaxInputLengthProperty);
            set => SetValue(MaxInputLengthProperty, value);
        }

        public bool SelectAllTextOnFocus
        {
            get => (bool)GetValue(SelectAllTextOnFocusProperty);
            set => SetValue(SelectAllTextOnFocusProperty, value);
        }

        [GeneratedRegex("[^0-9]+")]
        private static partial Regex NumericRegex();

        private static bool IsNumeric(string input)
        {
            return !NumericRegex().IsMatch(input);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MaxLength = MaxInputLength;
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            DataObject.AddPastingHandler(AssociatedObject, OnPasting);
            SetFocusHandler();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.GotKeyboardFocus -= OnGotKeyboardFocus;
            DataObject.RemovePastingHandler(AssociatedObject, OnPasting);
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;

            base.OnDetaching();
        }

        private void SetFocusHandler()
        {
            if (SelectAllTextOnFocus)
                AssociatedObject.GotKeyboardFocus += OnGotKeyboardFocus;
            else
                AssociatedObject.GotKeyboardFocus -= OnGotKeyboardFocus;
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return; 
            }

            string text = (string)e.DataObject.GetData(typeof(string));
            if (!IsNumeric(text))
                e.CancelCommand();
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (AssociatedObject is null || AssociatedObject.Dispatcher is null)
                return;

            AssociatedObject.Dispatcher.BeginInvoke(AssociatedObject.SelectAll);
        }
    }
}
