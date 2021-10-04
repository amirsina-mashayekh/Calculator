using BigNumbers;
using static Calculator.Evaluator;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Brush disabledColor = Brushes.Gray;

        private BigNumber memory;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ResetResultBox()
        {
            resultBox.Foreground = disabledColor;
            resultBox.Text = "Result";
        }

        private void ResetInputBox()
        {
            inputBox.Foreground = disabledColor;
            inputBox.Text = "Input";
        }

        private void InsertInput(string text, int caretIndexOffset = 0)
        {
            if (inputBox.Foreground == disabledColor)
            {
                inputBox.Text = "";
                inputBox.Foreground = Brushes.White;
            }

            int caretIndex = inputBox.CaretIndex;
            inputBox.Text = inputBox.Text.Insert(inputBox.CaretIndex, text);
            inputBox.CaretIndex = caretIndex + text.Length + caretIndexOffset;
        }

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (inputBox.Foreground == disabledColor)
            {
                inputBox.Clear();
                inputBox.Foreground = Brushes.White;
            }
        }

        private void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputBox.Text)) ResetInputBox();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ResetResultBox();
            ResetInputBox();
        }

        private void FunctionButtons_Click(object sender, RoutedEventArgs e)
        {
            string text = (e.Source as Button).Content.ToString() + "()";
            InsertInput(text, -1);
        }

        private void SelfInsert_Click(object sender, RoutedEventArgs e)
        {
            string text = (e.Source as Button).Content.ToString();
            InsertInput(text);
        }

        private void Integral_Click(object sender, RoutedEventArgs e)
        {
            InsertInput("\u222B(,,)", -3);
        }

        private void Power_Click(object sender, RoutedEventArgs e)
        {
            InsertInput("^");
        }

        private void Factorial_Click(object sender, RoutedEventArgs e)
        {
            InsertInput("fact()", -1);
        }

        private void Absolute_Click(object sender, RoutedEventArgs e)
        {
            InsertInput("abs()", -1);
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (inputBox.Foreground == disabledColor) { return; }
            string expression = inputBox.Text
                .ToLowerInvariant()
                .Replace("^", "pow")
                .Replace("%", "mod")
                .Replace("\u03C0", Math.PI.ToString())
                .Replace("\u00D7", "*")
                .Replace("\u2212", "-")
                .Replace("\u00F7", "/")
                .Replace("\u222B", "integral");
            try
            {
                resultBox.Text = EvaluateRPN(InfixToRPN(Tokenize(expression))).Value;
                resultBox.Foreground = Brushes.LightGreen;
            }
            catch (ArgumentException ex)
            {
                resultBox.Foreground = Brushes.White;
                resultBox.Text = "Error: " + ex.Message;
            }
            catch (Exception)
            {
                resultBox.Foreground = Brushes.White;
                resultBox.Text = "Error: Invalid expression.";
            }
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            int caretIndex = inputBox.CaretIndex;

            if (caretIndex == 0)
            {
                ResetInputBox();
                return;
            }

            inputBox.Text = inputBox.Text.Remove(caretIndex - 1, 1);
            inputBox.CaretIndex = caretIndex - 1;
        }

        private void MemoryStore_Click(object sender, RoutedEventArgs e)
        {
            if (resultBox.Foreground != disabledColor)
            {
                try
                {
                    memory = new BigNumber(resultBox.Text);
                    memoryRecall.IsEnabled = true;
                    memoryClear.IsEnabled = true;
                }
                catch (ArithmeticException) { }
            }
        }

        private void MemoryRecall_Click(object sender, RoutedEventArgs e)
        {
            if (memory != null)
            {
                InsertInput(memory.Value);
            }
        }

        private void MemoryClear_Click(object sender, RoutedEventArgs e)
        {
            memory = null;
            memoryRecall.IsEnabled = false;
            memoryClear.IsEnabled = false;
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (resultBox.Foreground != disabledColor)
            {
                Clipboard.SetText(resultBox.Text);
            }
        }

        private void ButtonsGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            _ = inputBox.Focus();
        }

        private void InputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                equals.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, equals));
            }
        }
    }
}