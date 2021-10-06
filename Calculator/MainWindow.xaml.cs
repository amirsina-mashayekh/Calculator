using BigNumbers;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using static Evaluation.Evaluator;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Brush placeholderColor = Brushes.Gray;

        private readonly Brush successColor = Brushes.LightGreen;

        private readonly string piString = '(' + Math.PI.ToString() + ')';

        private readonly GridLength inputHeight;

        private BigNumber memory;

        private readonly int disableUITimeout = 250;

        public MainWindow()
        {
            InitializeComponent();
            inputHeight = inputRow.Height;
        }

        private void ShowResult(string result, bool success)
        {
            resultBox.IsEnabled = true;

            resultBox.Foreground = success ? successColor : Brushes.White;

            resultBox.Text = result;
        }

        private void ResetResultBox()
        {
            resultBox.Foreground = placeholderColor;
            resultBox.IsEnabled = false;
            resultBox.Text = "Result";
        }

        private void ResetInputBox()
        {
            inputBox.Foreground = placeholderColor;
            inputBox.Text = "Input";
        }

        private void InsertInput(string text, int caretIndexOffset = 0)
        {
            if (inputBox.Foreground == placeholderColor)
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
            if (inputBox.Foreground == placeholderColor)
            {
                inputBox.Clear();
                inputBox.Foreground = Brushes.White;
            }
        }

        private void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputBox.Text)) { ResetInputBox(); }
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
            inputRow.Height = new GridLength(0);
            inputBox.IsEnabled = false;
            buttonsGrid.Visibility = Visibility.Collapsed;
            integralGrid.Visibility = Visibility.Visible;
        }

        private void BackToKeypad_Click(object sender, RoutedEventArgs e)
        {
            inputBox.IsEnabled = true;
            inputRow.Height = inputHeight;
            integralGrid.Visibility = Visibility.Collapsed;
            buttonsGrid.Visibility = Visibility.Visible;
            polynomial.Children.Clear();
            upperBound.Clear();
            lowerBound.Clear();
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

        private async void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (inputBox.Foreground == placeholderColor) { return; }
            string expression = inputBox.Text
                .ToLowerInvariant()
                .Replace("^", "pow")
                .Replace("%", "mod")
                .Replace("\u03C0", piString)
                .Replace("\u00D7", "*")
                .Replace("\u2212", "-")
                .Replace("\u00F7", "/");

            bool inputFocused = inputBox.IsFocused;

            try
            {
                Task<BigNumber> calc = CalculateAsync(expression);

                if (await Task.WhenAny(calc, Task.Delay(disableUITimeout)) != calc)
                {
                    mainGrid.IsEnabled = false;
                    resultBox.Foreground = placeholderColor;
                    resultBox.Text = "Calculating...";
                }

                ShowResult((await calc).Value, true);
            }
            catch (Exception ex)
            {
                ShowResult("Error: " + ex.Message, false);
            }

            mainGrid.IsEnabled = true;
            if (inputFocused) { _ = inputBox.Focus(); }
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            int caretIndex = inputBox.CaretIndex;

            if (caretIndex <= 1)
            {
                ResetInputBox();
                return;
            }

            inputBox.Text = inputBox.Text.Remove(caretIndex - 1, 1);
            inputBox.CaretIndex = caretIndex - 1;
        }

        private void MemoryStore_Click(object sender, RoutedEventArgs e)
        {
            if (resultBox.Foreground == successColor)
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

        private void Floor_Click(object sender, RoutedEventArgs e)
        {
            InsertInput("floor()", -1);
        }

        private void Ceil_Click(object sender, RoutedEventArgs e)
        {
            InsertInput("ceil()", -1);
        }

        private void AddExponent_Click(object sender, RoutedEventArgs e)
        {
            int exp = polynomial.Children.Count;

            if (exp > 1)
            {
                exp = (exp + 1) / 2;
            }

            Grid grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Center };
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            TextBox input = new TextBox()
            {
                Name = "exp" + exp.ToString(),
                Style = FindResource("IntegralInputsStyle") as Style,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 75,
                FontSize = 18
            };

            TextBlock exponent = new TextBlock()
            {
                Margin = new Thickness(5, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 18
            };
            Run run = new Run(exp.ToString());
            run.Typography.Variants = FontVariants.Superscript;
            exponent.Inlines.Add("x");
            exponent.Inlines.Add(run);

            grid.Children.Add(input);
            grid.Children.Add(exponent);
            Grid.SetColumn(exponent, 1);

            if (exp > 0)
            {
                polynomial.Children.Add(new TextBlock()
                {
                    TextAlignment = TextAlignment.Center,
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 18,
                    Margin = new Thickness(0, -3, 0, 0),
                    Text = "+"
                });
            }

            polynomial.Children.Add(grid);
        }

        private void RemoveExponent_Click(object sender, RoutedEventArgs e)
        {
            int cnt = polynomial.Children.Count;

            if (cnt == 0) { return; }

            polynomial.Children.RemoveAt(--cnt);

            if (cnt > 0) { polynomial.Children.RemoveAt(--cnt); }
        }

        private async void CalculateIntegral_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task<BigNumber> calc = CalculateIntegral();

                if (await Task.WhenAny(calc, Task.Delay(disableUITimeout)) != calc)
                {
                    mainGrid.IsEnabled = false;
                    resultBox.Foreground = placeholderColor;
                    resultBox.Text = "Calculating...";
                }

                ShowResult((await calc).Value, true);
            }
            catch (Exception ex)
            {
                ShowResult(ex.Message, false);
            }

            mainGrid.IsEnabled = true;
        }

        private async Task<BigNumber> CalculateIntegral()
        {
            int cnt = polynomial.Children.Count;

            if (cnt == 0) { throw new ArgumentException(); }

            BigNumber ub;
            BigNumber lb;

            try
            {
                ub = await CalculateAsync(upperBound.Text);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in upper bound: " + ex.Message);
            }

            try
            {
                lb = await CalculateAsync(lowerBound.Text);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in lower bound: " + ex.Message);
            }

            string ube = "";
            string lbe = "";

            for (int i = 0; i < cnt; i += 2)
            {
                int exp = (i / 2) + 1;

                TextBox input =
                    (polynomial.Children[i] as Grid)
                    .Children[0] as TextBox;

                BigNumber coefficient;

                try
                {
                    coefficient = await CalculateAsync('(' + input.Text + ")/" + exp.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in coefficient of exponent " + exp.ToString() + ": " + ex.Message);
                }

                ube += "+(" + coefficient.Value + ")*" + ub + "pow" + exp;
                lbe += "+(" + coefficient.Value + ")*" + lb + "pow" + exp;
            }

            try
            {
                return await CalculateAsync(ube) - await CalculateAsync(lbe);
            }
            catch (Exception)
            {
                // Should not get here at all, but just in case...
                throw new Exception("Error: Something went wrong.");
            }
        }
    }
}