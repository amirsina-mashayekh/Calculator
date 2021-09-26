using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
			if (string.IsNullOrWhiteSpace(inputBox.Text))
			{
				inputBox.Foreground = Brushes.Gray;
				inputBox.Text = "Input";
			}
		}
	}
}
