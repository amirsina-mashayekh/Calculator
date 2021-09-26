using System;
using System.Text.RegularExpressions;

namespace Calculator
{
    public class BigNumber
    {
        public string Value
        {
            get { return Value; }
            set
            {
                if (!Regex.IsMatch(value, @"-?\d+\.*\d*") && value != "")
                {
                    throw new FormatException("String is not a real number.");
                }
            }
        }

        public BigNumber(string Value)
        {
            this.Value = Value;
        }
    }
}
