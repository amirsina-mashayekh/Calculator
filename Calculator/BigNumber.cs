using System;
using System.Text.RegularExpressions;

namespace Calculator
{
    /// <summary>
    /// Save and basic arithmetic for big numbers.
    /// </summary>
    public class BigNumber : IEquatable<BigNumber>, IComparable<BigNumber>
    {
        private static readonly BigNumber zero = new BigNumber("0");

        private static readonly BigNumber ten = new BigNumber("10");

        public bool Sign { get; private set; }

        public string IntegerPart { get; private set; }

        public string DecimalPart { get; private set; }

        public string Value
        {
            get
            {
                string val = "";

                if (!Sign) { val += '-'; }
                val += IntegerPart;
                if (DecimalPart != "") { val += '.' + DecimalPart; }

                return val;
            }
            set
            {
                MatchCollection matches = Regex.Matches(value, @"[+-]?(\d*\.)?\d+");
                if (matches.Count != 1)
                {
                    throw new FormatException("String is not a rational number.");
                }

                Sign = value[0] != '-';

                int pointIndex = value.IndexOf('.');
                if (pointIndex > -1)
                {
                    IntegerPart = value[(Sign ? 0 : 1)..pointIndex];
                    DecimalPart = value[(pointIndex + 1)..];
                }
                else
                {
                    IntegerPart = value[(Sign ? 0 : 1)..];
                    DecimalPart = "";
                }

                IntegerPart = IntegerPart.TrimStart('0');
                if (IntegerPart == "") { IntegerPart = "0"; }

                DecimalPart = DecimalPart.TrimEnd('0');

                if (IntegerPart == "0" && DecimalPart == "") { Sign = true; }
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
        /// Compares this instance to a specified <c>BigNumber</c> and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <c>BigNumber</c> to compare.</param>
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
            string i0 = IntegerPart;
            string d0 = DecimalPart;
            string i1 = other.IntegerPart;
            string d1 = other.DecimalPart;

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
            if (n.Value == "0") { return n; }

            BigNumber result = n.MemberwiseClone() as BigNumber;
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
            BigNumber rv;
            bool rs = true;

            rv = n.Sign != n1.Sign ? AbsDif(n, n1) : AbsSum(n, n1);

            if ((!n.Sign && -n > n1) || (!n1.Sign && -n1 > n))
            {
                rs = false;
            }

            return rs ? rv : -rv;
        }

        public static BigNumber operator -(BigNumber n, BigNumber n1)
        {
            BigNumber rv;
            bool rs = true;

            rv = n.Sign == n1.Sign ? AbsDif(n, n1) : AbsSum(n, n1);

            if ((!n.Sign && n.Abs() > n1.Abs()) || (n1.Sign && n1.Abs() > n.Abs()))
            {
                rs = false;
            }

            return rs ? rv : -rv;
        }

        public static BigNumber operator ++(BigNumber n)
        {
            return n + new BigNumber("1");
        }

        public static BigNumber operator --(BigNumber n)
        {
            return n - new BigNumber("1");
        }

        public static BigNumber operator *(BigNumber n, BigNumber n1)
        {
            BigNumber result = zero.MemberwiseClone() as BigNumber;
            BigNumber nAbs = n.Abs();
            BigNumber n1Abs = n1.Abs();
            string lev;
            string gtv;

            // Put number with smaller absolute value in lev
            if (nAbs > n1Abs)
            {
                lev = n1Abs.Value.Replace(".", "");
                gtv = nAbs.Value.Replace(".", "");
            }
            else
            {
                lev = nAbs.Value.Replace(".", "");
                gtv = n1Abs.Value.Replace(".", "");
            }

            int llen = lev.Length;
            int glen = gtv.Length;
            int carry = 0;
            for (int i = llen - 1; i >= 0; i--)
            {
                if (lev[i] == '0') { continue; }

                string mul = "";
                for (int j = 0; j < llen - 1 - i; j++) { mul += '0'; }

                for (int j = glen - 1; j >= 0; j--)
                {
                    int m = ((gtv[j] - '0') * (lev[i] - '0')) + carry;
                    if (m > 9)
                    {
                        carry = m / 10;
                        m %= 10;
                    }
                    else { carry = 0; }

                    mul = m.ToString() + mul;
                }
                if (carry > 0)
                {
                    mul = carry.ToString() + mul;
                    carry = 0;
                }

                result += new BigNumber(mul);
            }

            int decimalPoints = nAbs.DecimalPart.Length + n1Abs.DecimalPart.Length;
            while (decimalPoints >= result.IntegerPart.Length)
            {
                // Add 0s before number to be able to insert decimal point
                result.IntegerPart = '0' + result.IntegerPart;
            }
            result = new BigNumber(result.IntegerPart.Insert(result.IntegerPart.Length - decimalPoints, "."));
            return (n.Sign ^ n1.Sign) ? -result : result;
        }

        public static BigNumber operator /(BigNumber n, BigNumber n1)
        {
            return DivideAndRemainder(n, n1, out _);
        }

        public static BigNumber operator %(BigNumber n, BigNumber n1)
        {
            if (n.DecimalPart != "" || n1.DecimalPart != "")
            {
                throw new ArithmeticException("Modulus division is only available for integers.");
            }
            _ = DivideAndRemainder(n, n1, out BigNumber result);
            return result;
        }

        /// <summary>
        /// Returns the absolute value of this instance of BigNumber.
        /// </summary>
        /// <returns>A bigNumber.</returns>
        public BigNumber Abs()
        {
            return Sign ? this : -this;
        }

        /// <summary>
        /// Calculates the sum of absolute values of two <c>BigNumber</c>s.
        /// </summary>
        /// <param name="n">A <c>BigNumber</c>.</param>
        /// <param name="n1">Another <c>BigNumber</c>.</param>
        /// <returns>Sum of absolute values of <c>n</c> and <c>n1</c>.</returns>
        private static BigNumber AbsSum(BigNumber n, BigNumber n1)
        {
            string i0 = n.IntegerPart;
            string d0 = n.DecimalPart;
            string i1 = n1.IntegerPart;
            string d1 = n1.DecimalPart;

            // Make lenght of numbers equal by adding non-significant 0s
            while (i1.Length < i0.Length) { i1 = '0' + i1; }
            while (i0.Length < i1.Length) { i0 = '0' + i0; }
            while (d1.Length < d0.Length) { d1 += '0'; }
            while (d0.Length < d1.Length) { d0 += '0'; }

            string rd = "";
            int carry = 0;
            int dlen = d0.Length;
            for (int i = dlen - 1; i >= 0; i--)
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
            int ilen = i0.Length;
            for (int i = ilen - 1; i >= 0; i--)
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

            return new BigNumber(ri + '.' + rd);
        }

        /// <summary>
        /// Calculates the differece of absolute values of two <c>BigNumber</c>s.
        /// </summary>
        /// <param name="n">A <c>BigNumber</c>.</param>
        /// <param name="n1">Another <c>BigNumber</c>.</param>
        /// <returns>Difference of absolute values of <c>n</c> and <c>n1</c>.</returns>
        private static BigNumber AbsDif(BigNumber n, BigNumber n1)
        {
            string i0;
            string d0;
            string i1;
            string d1;

            // Put greater number in x0
            if (n.Abs() > n1.Abs())
            {
                i0 = n.IntegerPart;
                d0 = n.DecimalPart;
                i1 = n1.IntegerPart;
                d1 = n1.DecimalPart;
            }
            else
            {
                i0 = n1.IntegerPart;
                d0 = n1.DecimalPart;
                i1 = n.IntegerPart;
                d1 = n.DecimalPart;
            }

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

            return new BigNumber(ri + '.' + rd);
        }

        /// <summary>
        /// Calculates the quotient and remainder of division of two <c>BigNumber</c>s.
        /// </summary>
        /// <param name="n">A <c>BigNumber</c>.</param>
        /// <param name="n1">Another <c>BigNumber</c>.</param>
        /// <param name="remainder">A <c>BigNumber</c> to put remainder of <c>n/n1</c> in it.</param>
        /// <returns>Quotient of <c>n/n1</c>.</returns>
        private static BigNumber DivideAndRemainder(BigNumber n, BigNumber n1, out BigNumber remainder)
        {
            if (n1 == zero) { throw new ArithmeticException("Can't divide by zero."); }

            BigNumber divisioned = (n.MemberwiseClone() as BigNumber).Abs();
            BigNumber divisor = (n1.MemberwiseClone() as BigNumber).Abs();

            // Get rid of decimals
            while (divisor.DecimalPart.Length > 0 || divisioned.DecimalPart.Length > 0)
            {
                divisioned *= ten;
                divisor *= ten;
            }

            string ri = "";
            int divLen = divisor.IntegerPart.Length;
            BigNumber rem = zero.MemberwiseClone() as BigNumber;
            while (divisioned.IntegerPart.Length > 0 || divisioned > divisor)
            {
                BigNumber tempDiv = new BigNumber(rem.IntegerPart + divisioned.IntegerPart[0]);

                BigNumber tmp = new BigNumber("9");
                BigNumber mul;

                int i = 9;
                do
                {
                    mul = divisor * tmp;
                    if (mul <= tempDiv) { break; }
                    i--;
                    tmp.IntegerPart = i.ToString();
                } while (true);

                ri += i.ToString();

                rem = tempDiv - mul;
                divisioned.IntegerPart = divisioned.IntegerPart[1..];
            }

            bool sign = n.Sign ^ n1.Sign;
            if (n.DecimalPart == "" || n1.DecimalPart == "")
            {
                if (sign) { rem = divisor - rem; }
                remainder = n1.Sign ? rem : -rem;
            }
            else { remainder = null; }

            return new BigNumber((sign ? "-" : "") + ri);
        }
    }
}