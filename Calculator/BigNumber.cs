using System;
using System.Text.RegularExpressions;

namespace Calculator
{
    /// <summary>
    /// Save and arithmetics for big numbers.
    /// </summary>
    public class BigNumber : IEquatable<BigNumber>
    {
        public bool Sign { get; private set; }

        private string _integer;

        private string _decimal;

        public string Value
        {
            get
            {
                string val = "";

                if (!Sign) { val += '-'; }
                val += _integer;
                if (_decimal != "") { val += '.' + _decimal; }

                return val;
            }
            set
            {
                if (!Regex.IsMatch(value, @"-?\d+\.*\d*"))
                {
                    throw new FormatException("String is not a rational number.");
                }

                Sign = value[0] != '-';

                int pointIndex = value.IndexOf('.');
                if (pointIndex > -1)
                {
                    _integer = value[(Sign ? 0 : 1)..pointIndex];
                    _decimal = value[(pointIndex + 1)..];
                }
                else
                {
                    _integer = value[(Sign ? 0 : 1)..];
                    _decimal = "";
                }

                _integer = _integer.TrimStart('0');
                if (_integer == "") { _integer = "0"; }

                _decimal = _decimal.TrimEnd('0');

                if (_integer == "0" && _decimal == "") { Sign = true; }
            }
        }

        public BigNumber(string Value)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BigNumber);
        }

        public bool Equals(BigNumber other)
        {
            return !(other is null) && (ReferenceEquals(this, other) || Value == other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static BigNumber operator +(BigNumber n)
        {
            return n;
        }

        public static BigNumber operator -(BigNumber n)
        {
            BigNumber result = (BigNumber)n.MemberwiseClone();
            result.Sign = !result.Sign;
            return result;
        }

        public static bool operator ==(BigNumber n, BigNumber n1)
        {
            return ReferenceEquals(n, n1) || (!(n is null) && !(n1 is null) && n.Equals(n1));
        }

        public static bool operator !=(BigNumber n, BigNumber n1)
        {
            return !(n == n1);
        }
    }
}