﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ResetResultBox()
        {
            resultBox.Foreground = Brushes.Gray;
            resultBox.Text = "Result";
        }

        private void ResetInputBox()
        {
            inputBox.Foreground = Brushes.Gray;
            inputBox.Text = "Input";
        }

        private void InsertInput(string text, int caretIndexOffset = 0)
        {
            if (inputBox.Foreground == Brushes.Gray)
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
            if (inputBox.Foreground == Brushes.Gray)
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

        private void Power_Click(object sender, RoutedEventArgs e)
        {
            InsertInput("^");
        }

        private void Factorial_Click(object sender, RoutedEventArgs e)
        {
            InsertInput("!");
        }

        private void Absolute_Click(object sender, RoutedEventArgs e)
        {
            InsertInput("||", -1);
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            resultBox.Foreground = Brushes.White;
            resultBox.Text = "Nothing yet...";
        }
    }
}