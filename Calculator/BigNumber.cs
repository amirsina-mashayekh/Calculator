using System;
using System.Text.RegularExpressions;

namespace Calculator
{
    /// <summary>
    /// Save and arithmetics for big numbers.
    /// </summary>
    public class BigNumber : IEquatable<BigNumber>, IComparable<BigNumber>
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

        /// <summary>
        /// Compares this instance to a specified BigNumber and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A BigNumber to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <c>n</c>. Returns
        /// -1 if this instance is less than <c>n</c>, 0 if this instance is equal to <c>n</c> and
        /// 1 if this instance is greater than <c>n</c>.
        /// </returns>
        public int CompareTo(BigNumber other)
        {
            if (Sign != other.Sign)
            {
                return Sign ? 1 : -1;
            }

            int result = 0;
            string i0 = _integer;
            string d0 = _decimal;
            string i1 = other._integer;
            string d1 = other._decimal;

            // Make lenght of numbers equal by adding non-significant 0s
            while (i1.Length < i0.Length) { i1 = '0' + i1; }
            while (i0.Length < i1.Length) { i0 = '0' + i0; }
            while (d1.Length < d0.Length) { d1 += '0'; }
            while (d0.Length < d1.Length) { d0 += '0'; }

            for (int i = 0; i < i0.Length; i++)
            {
                // No need to cast
                if (i0[i] > i1[i]) { result = 1; }
                else if (i0[i] < i1[i]) { result = -1; }
                if (result != 0) { break; }
            }
            if (result != 0) { return result; }

            for (int i = 0; i < d0.Length; i++)
            {
                // No need to cast
                if (d0[i] > d1[i]) { result = 1; }
                else if (d0[i] < d1[i]) { result = -1; }
                if (result != 0) { break; }
            }

            return Sign ? result : -result;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BigNumber);
        }

        public bool Equals(BigNumber other)
        {
            return !(other is null) && (ReferenceEquals(this, other) || CompareTo(other) == 0);
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

        public static bool operator <(BigNumber n, BigNumber n1)
        {
            return n.CompareTo(n1) == -1;
        }

        public static bool operator >(BigNumber n, BigNumber n1)
        {
            return n.CompareTo(n1) == 1;
        }

        public static bool operator <=(BigNumber n, BigNumber n1)
        {
            return n.CompareTo(n1) < 1;
        }

        public static bool operator >=(BigNumber n, BigNumber n1)
        {
            return n.CompareTo(n1) > -1;
        }

        public static BigNumber operator +(BigNumber n, BigNumber n1)
        {
            if (n.Sign != n1.Sign)
            {
                return n.Sign ? n - (-n1) : n1 - (-n);
            }

            string i0 = n._integer;
            string d0 = n._decimal;
            string i1 = n1._integer;
            string d1 = n1._decimal;

            // Make lenght of numbers equal by adding non-significant 0s
            while (i1.Length < i0.Length) { i1 = '0' + i1; }
            while (i0.Length < i1.Length) { i0 = '0' + i0; }
            while (d1.Length < d0.Length) { d1 += '0'; }
            while (d0.Length < d1.Length) { d0 += '0'; }

            string rd = "";
            int carry = 0;
            for (int i = d0.Length - 1; i >= 0; i--)
            {
                int sum = d0[i] - '0' + d1[i] - '0' + carry;
                if (sum > 9)
                {
                    sum -= 10;
                    carry = 1;
                }
                else { carry = 0; }
                rd = sum.ToString() + rd;
            }

            string ri = "";
            for (int i = i0.Length - 1; i >= 0; i--)
            {
                int sum = i0[i] - '0' + i1[i] - '0' + carry;
                if (sum > 9)
                {
                    sum -= 10;
                    carry = 1;
                }
                else { carry = 0; }
                ri = sum.ToString() + ri;
            }

            if (carry == 1) { ri = '1' + ri; }

            return new BigNumber((n.Sign ? "" : "-") + ri + '.' + rd);
        }

        // TODO: Complete it
        public static BigNumber operator -(BigNumber n, BigNumber n1)
        {
            if (!n1.Sign)
            {
                return n + (-n1);
            }

            string i0 = n._integer;
            string d0 = n._decimal;
            string i1 = n1._integer;
            string d1 = n1._decimal;

            // Make lenght of numbers equal by adding non-significant 0s
            while (i1.Length < i0.Length) { i1 = '0' + i1; }
            while (i0.Length < i1.Length) { i0 = '0' + i0; }
            while (d1.Length < d0.Length) { d1 += '0'; }
            while (d0.Length < d1.Length) { d0 += '0'; }

            string rd = "";
            int carry = 0;
            for (int i = d0.Length - 1; i >= 0; i--)
            {
                int dif = d0[i] - '0' - (d1[i] - '0') - carry;
                if (dif < 0)
                {
                    dif += 10;
                    carry = 1;
                }
                else { carry = 0; }
                rd = dif.ToString() + rd;
            }

            string ri = "";
            for (int i = i0.Length - 1; i >= 0; i--)
            {
                int dif = i0[i] - '0' - (i1[i] - '0') - carry;
                if (dif < 0)
                {
                    dif += 10;
                    carry = 1;
                }
                else { carry = 0; }
                ri = dif.ToString() + ri;
            }

            return new BigNumber((n.Sign ? "" : "-") + ri + '.' + rd);
        }
    }
}