using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BigNumbers
{
    /// <summary>
    /// Save and basic arithmetic for big numbers.
    /// </summary>
    public class BigNumber : IEquatable<BigNumber>, IComparable<BigNumber>
    {
        private static readonly BigNumber zero = new BigNumber(0);

        private static readonly BigNumber ten = new BigNumber(10);

        public bool Sign { get; private set; }

        private List<int> _integralPart;

        public string IntegralPart => string.Join(null, _integralPart);

        private List<int> _decimalPart;

        public string DecimalPart => string.Join(null, _decimalPart);

        public string Value
        {
            get
            {
                string val = "";

                if (!Sign) { val += '-'; }
                val += string.Join(null, _integralPart);
                if (_decimalPart.Count != 0) { val += '.' + string.Join(null, _decimalPart); }

                return val;
            }
            set
            {
                MatchCollection matches = Regex.Matches(value, @"[+-]?(\d*\.)?\d+");
                if (matches.Count != 1 || matches[0].ToString() != value)
                {
                    throw new FormatException("String is not a rational number.");
                }

                _integralPart = new List<int>();
                _decimalPart = new List<int>();
                Sign = value[0] != '-';
                value = value.TrimStart(new char[] { '-', '+' });

                int pointIndex = value.IndexOf('.');
                int len = value.Length;

                if (pointIndex > -1)
                {
                    for (int i = 0; i < pointIndex; i++)
                    {
                        _integralPart.Add(value[i] - '0');
                    }
                    for (int i = pointIndex + 1; i < len; i++)
                    {
                        _decimalPart.Add(value[i] - '0');
                    }
                }
                else
                {
                    for (int i = 0; i < len; i++)
                    {
                        _integralPart.Add(value[i] - '0');
                    }
                }

                if (_integralPart.Count == 0) { _integralPart.Add(0); }

                RemoveExtraZeroes();
                if (_integralPart[0] == 0 && _decimalPart.Count == 0) { Sign = true; }
            }
        }

        public BigNumber(string Value)
        {
            this.Value = Value;
        }

        public BigNumber(decimal Value)
        {
            _integralPart = new List<int>();
            _decimalPart = new List<int>();

            decimal abs = Math.Abs(Value);
            decimal i = Math.Truncate(abs);
            decimal d = abs - i;

            if (i == 0)
            {
                _integralPart.Add(0);
            }
            else
            {
                while (i >= 1)
                {
                    _integralPart.Insert(0, (int)(i % 10));
                    i /= 10;
                }
            }

            while (d > 0)
            {
                d *= 10;
                int digit = (int)d;
                _decimalPart.Add(digit);
                d -= digit;
            }
            Sign = Value >= 0;
        }

        private BigNumber()
        {
            Sign = true;
            _integralPart = new List<int>();
            _decimalPart = new List<int>();
        }

        public override string ToString()
        {
            return Value;
        }

        public decimal ToDecimal()
        {
            return decimal.Parse(Value);
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
            List<int> i0 = new List<int>(_integralPart);
            List<int> d0 = new List<int>(_decimalPart);
            List<int> i1 = new List<int>(other._integralPart);
            List<int> d1 = new List<int>(other._decimalPart);

            // Make lenght of numbers equal by adding non-significant 0s
            while (i1.Count < i0.Count) { i1.Insert(0, 0); }
            while (i0.Count < i1.Count) { i0.Insert(0, 0); }
            while (d1.Count < d0.Count) { d1.Add(0); }
            while (d0.Count < d1.Count) { d0.Add(0); }

            for (int i = 0; i < i0.Count; i++)
            {
                // No need to cast
                if (i0[i] > i1[i]) { result = 1; }
                else if (i0[i] < i1[i]) { result = -1; }
                if (result != 0) { break; }
            }
            if (result != 0) { return Sign ? result : -result; }

            for (int i = 0; i < d0.Count; i++)
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
            BigNumber result;

            result = n.Sign == n1.Sign ? AbsDif(n, n1) : AbsSum(n, n1);

            if ((!n.Sign && n1.Sign)
                || (!n.Sign && !n1.Sign && n.Abs() > n1.Abs())
                || (n.Sign && n1.Sign && n1.Abs() > n.Abs()))
            {
                result.Sign = false;
            }

            return result;
        }

        public static BigNumber operator ++(BigNumber n)
        {
            return n + new BigNumber(1);
        }

        public static BigNumber operator --(BigNumber n)
        {
            return n - new BigNumber(1);
        }

        public static BigNumber operator *(BigNumber n, BigNumber n1)
        {
            BigNumber result = zero.MemberwiseClone() as BigNumber;
            List<int> lnz = new List<int>(n._integralPart);
            lnz.AddRange(n._decimalPart);
            List<int> mnz = new List<int>(n1._integralPart);
            mnz.AddRange(n1._decimalPart);

            // Put number with less non-zero characters in lnz
            int nnz = 0;
            foreach (int digit in lnz)
            {
                if (digit != 0) { nnz++; }
            }
            int n1nz = 0;
            foreach (int digit in mnz)
            {
                if (digit != 0) { n1nz++; }
            }
            if (nnz > n1nz)
            {
                List<int> tmp = new List<int>(lnz);
                lnz = mnz;
                mnz = tmp;
            }

            int llen = lnz.Count;
            int glen = mnz.Count;
            int carry = 0;
            for (int i = llen - 1; i >= 0; i--)
            {
                if (lnz[i] == 0) { continue; }

                BigNumber mul = new BigNumber();
                for (int j = 0; j < llen - 1 - i; j++) { mul._integralPart.Add(0); }

                for (int j = glen - 1; j >= 0; j--)
                {
                    int m = (mnz[j] * lnz[i]) + carry;
                    if (m > 9)
                    {
                        carry = m / 10;
                        m %= 10;
                    }
                    else { carry = 0; }

                    mul._integralPart.Insert(0, m);
                }
                if (carry > 0)
                {
                    mul._integralPart.Insert(0, carry);
                    carry = 0;
                }

                result += mul;
            }

            int decimalPlaces = n._decimalPart.Count + n1._decimalPart.Count;
            while (decimalPlaces >= result._integralPart.Count)
            {
                // Add 0s before number to be able to insert decimal point
                result._integralPart.Insert(0, 0);
            }
            int integralPlacess = result._integralPart.Count - decimalPlaces;

            result._decimalPart = result._integralPart.GetRange(integralPlacess, decimalPlaces);
            result._integralPart = result._integralPart.GetRange(0, integralPlacess);
            result.RemoveExtraZeroes();
            return (n.Sign ^ n1.Sign) ? -result : result;
        }

        public static BigNumber operator /(BigNumber n, BigNumber n1)
        {
            return DivideAndRemainder(n, n1, out _);
        }

        public static BigNumber operator %(BigNumber n, BigNumber n1)
        {
            _ = DivideAndRemainder(n, n1, out BigNumber result);

            int ndc = n._decimalPart.Count;
            int n1dc = n1._decimalPart.Count;
            int decimalPlaces = ndc > n1dc ? ndc : n1dc;
            while (decimalPlaces >= result._integralPart.Count)
            {
                // Add 0s before number to be able to insert decimal point
                result._integralPart.Insert(0, 0);
            }
            int integralPlacess = result._integralPart.Count - decimalPlaces;

            result._decimalPart = result._integralPart.GetRange(integralPlacess, decimalPlaces);
            result._integralPart = result._integralPart.GetRange(0, integralPlacess);
            result.RemoveExtraZeroes();

            return result;
        }

        /// <summary>
        /// Returns the absolute value of this instance of <c>BigNumber</c>.
        /// </summary>
        /// <returns>
        /// A <c>BigNumber</c> representing the abolute value of this <c>Bignumber</c>.
        /// </returns>
        public BigNumber Abs()
        {
            return Sign ? MemberwiseClone() as BigNumber : -this;
        }

        /// <summary>
        /// Rounds a <c>BigNumber</c> to a specified number of decimal digits.
        /// </summary>
        /// <param name="decimals">The number of decimal digits in return value.</param>
        /// <returns>
        /// The <c>BigNumber</c> nearest to value that contains a number of decimal digits equal
        /// to <c>decimals</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><c>decimals</c> is less than 0.</exception>
        public BigNumber Round(int decimals = 0)
        {
            if (decimals < 0)
            {
                throw new ArgumentOutOfRangeException("Decimal points should be at least 0.");
            }
            BigNumber result = MemberwiseClone() as BigNumber;

            if (result._decimalPart.Count > decimals)
            {
                if (result._decimalPart[decimals] >= 5)
                {
                    StringBuilder str = new StringBuilder("0.");
                    for (int i = 1; i < decimals; i++)
                    {
                        _ = str.Append('0');
                    }
                    _ = str.Append('1');

                    result += result.Sign ? new BigNumber(str.ToString()) : -new BigNumber(str.ToString());
                }

                result._decimalPart = result._decimalPart.GetRange(0, decimals);
            }

            result.RemoveExtraZeroes();
            return result;
        }

        /// <summary>
        /// Calculates the sum of absolute values of two <c>BigNumber</c>s.
        /// </summary>
        /// <param name="n">A <c>BigNumber</c>.</param>
        /// <param name="n1">Another <c>BigNumber</c>.</param>
        /// <returns>Sum of absolute values of <c>n</c> and <c>n1</c>.</returns>
        private static BigNumber AbsSum(BigNumber n, BigNumber n1)
        {
            BigNumber result = new BigNumber();
            List<int> i0 = new List<int>(n._integralPart);
            List<int> d0 = new List<int>(n._decimalPart);
            List<int> i1 = new List<int>(n1._integralPart);
            List<int> d1 = new List<int>(n1._decimalPart);

            // Make lenght of numbers equal by adding non-significant 0s
            while (i1.Count < i0.Count) { i1.Insert(0, 0); }
            while (i0.Count < i1.Count) { i0.Insert(0, 0); }
            while (d1.Count < d0.Count) { d1.Add(0); }
            while (d0.Count < d1.Count) { d0.Add(0); }

            int carry = 0;
            int dlen = d0.Count;
            for (int i = dlen - 1; i >= 0; i--)
            {
                int sum = d0[i] + d1[i] + carry;
                if (sum > 9)
                {
                    sum -= 10;
                    carry = 1;
                }
                else { carry = 0; }
                result._decimalPart.Insert(0, sum);
            }

            int ilen = i0.Count;
            for (int i = ilen - 1; i >= 0; i--)
            {
                int sum = i0[i] + i1[i] + carry;
                if (sum > 9)
                {
                    sum -= 10;
                    carry = 1;
                }
                else { carry = 0; }
                result._integralPart.Insert(0, sum);
            }

            if (carry == 1) { result._integralPart.Insert(0, 1); }

            result.RemoveExtraZeroes();
            return result;
        }

        /// <summary>
        /// Calculates the differece of absolute values of two <c>BigNumber</c>s.
        /// </summary>
        /// <param name="n">A <c>BigNumber</c>.</param>
        /// <param name="n1">Another <c>BigNumber</c>.</param>
        /// <returns>Difference of absolute values of <c>n</c> and <c>n1</c>.</returns>
        private static BigNumber AbsDif(BigNumber n, BigNumber n1)
        {
            BigNumber result = new BigNumber();
            List<int> i0;
            List<int> d0;
            List<int> i1;
            List<int> d1;

            // Put greater number in x0
            if (n.Abs() > n1.Abs())
            {
                i0 = new List<int>(n._integralPart);
                d0 = new List<int>(n._decimalPart);
                i1 = new List<int>(n1._integralPart);
                d1 = new List<int>(n1._decimalPart);
            }
            else
            {
                i0 = new List<int>(n1._integralPart);
                d0 = new List<int>(n1._decimalPart);
                i1 = new List<int>(n._integralPart);
                d1 = new List<int>(n._decimalPart);
            }

            // Make lenght of numbers equal by adding non-significant 0s
            while (i1.Count < i0.Count) { i1.Insert(0, 0); }
            while (i0.Count < i1.Count) { i0.Insert(0, 0); }
            while (d1.Count < d0.Count) { d1.Add(0); }
            while (d0.Count < d1.Count) { d0.Add(0); }

            int carry = 0;
            for (int i = d0.Count - 1; i >= 0; i--)
            {
                int dif = d0[i] - d1[i] - carry;
                if (dif < 0)
                {
                    dif += 10;
                    carry = 1;
                }
                else { carry = 0; }
                result._decimalPart.Insert(0, dif);
            }

            for (int i = i0.Count - 1; i >= 0; i--)
            {
                int dif = i0[i] - i1[i] - carry;
                if (dif < 0)
                {
                    dif += 10;
                    carry = 1;
                }
                else { carry = 0; }
                result._integralPart.Insert(0, dif);
            }

            result.RemoveExtraZeroes();
            return result;
        }

        /// <summary>
        /// Calculates the quotient and remainder of division of two <c>BigNumber</c>s.
        /// </summary>
        /// <param name="n">A <c>BigNumber</c>.</param>
        /// <param name="n1">Another <c>BigNumber</c>.</param>
        /// <param name="remainder">A <c>BigNumber</c> to put remainder of <c>n/n1</c> in it.</param>
        /// <returns>Quotient of <c>n/n1</c>.</returns>
        /// <exception cref="DivideByZeroException"><c>n1</c> is 0.</exception>
        private static BigNumber DivideAndRemainder(BigNumber n, BigNumber n1, out BigNumber remainder)
        {
            if (n1 == zero) { throw new DivideByZeroException(); }

            BigNumber divisioned = (n.MemberwiseClone() as BigNumber).Abs();
            BigNumber divisor = (n1.MemberwiseClone() as BigNumber).Abs();
            BigNumber result = new BigNumber();

            // Get rid of decimals
            while (divisor._decimalPart.Count > 0 || divisioned._decimalPart.Count > 0)
            {
                divisioned *= ten;
                divisor *= ten;
            }

            BigNumber rem = zero.MemberwiseClone() as BigNumber;
            while (divisioned._integralPart.Count > 0 || divisioned > divisor)
            {
                BigNumber tempDiv = new BigNumber()
                {
                    _integralPart = new List<int>(rem._integralPart)
                };
                tempDiv._integralPart.Add(divisioned._integralPart[0]);

                BigNumber tmp = new BigNumber(9);
                BigNumber mul;

                int i = 9;
                do
                {
                    mul = divisor * tmp;
                    if (mul <= tempDiv) { break; }
                    i--;
                    tmp._integralPart[0] = i;
                } while (true);

                result._integralPart.Add(i);

                rem = tempDiv - mul;
                divisioned._integralPart = divisioned._integralPart.GetRange(1, divisioned._integralPart.Count - 1);
            }

            bool sign = !(n.Sign ^ n1.Sign);

            if (!sign) { rem = divisor - rem; }
            remainder = n1.Sign ? rem : -rem;

            result.RemoveExtraZeroes();
            return sign ? result : -result;
        }

        /// <summary>
        /// Removes extra zeroes (before integal and after decimal places).
        /// </summary>
        private void RemoveExtraZeroes()
        {
            while (_integralPart[0] == 0 && _integralPart.Count > 1)
            {
                _integralPart.RemoveAt(0);
            }
            while (_decimalPart.Count > 0 && _decimalPart[^1] == 0)
            {
                _decimalPart.RemoveAt(_decimalPart.Count - 1);
            }
        }
    }
}