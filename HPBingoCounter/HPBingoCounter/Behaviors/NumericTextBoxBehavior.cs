using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HPBingoCounter.Behaviors
{
    internal partial class NumericTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty MaxInputLengthProperty = 
            DependencyProperty.Register(nameof(MaxInputLength), typeof(int), typeof(NumericTextBoxBehavior), new PropertyMetadata(1, OnMaxInputLengthChanged));

        private static void OnMaxInputLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericTextBoxBehavior behavior = (NumericTextBoxBehavior)d;
            if (behavior.AssociatedObject is null)
                return;

            behavior.AssociatedObject.MaxLength = (int)e.NewValue;
        }

        public int MaxInputLength 
        {
            get => (int)GetValue(MaxInputLengthProperty);
            set => SetValue(MaxInputLengthProperty, value);
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
        }

        protected override void OnDetaching()
        {
            DataObject.RemovePastingHandler(AssociatedObject, OnPasting);
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;

            base.OnDetaching();
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
    }
}
