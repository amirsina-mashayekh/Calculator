using System;
using System.Globalization;
using System.Collections.Generic;
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

        private static readonly BigNumber decimalMax = new BigNumber(decimal.MaxValue);

        private static readonly BigNumber decimalMin = new BigNumber(decimal.MinValue);

        public bool Sign { get; private set; }

        public List<int> IntegralPart { get; private set; }

        public List<int> DecimalPart { get; private set; }

        public string Value
        {
            get
            {
                string val = "";

                if (!Sign) { val += '-'; }
                val += string.Join(null, IntegralPart);
                if (DecimalPart.Count != 0) { val += '.' + string.Join(null, DecimalPart); }

                return val;
            }
            set
            {
                MatchCollection matches = Regex.Matches(value, @"[+-]?(\d*\.)?\d+");
                if (matches.Count != 1)
                {
                    throw new FormatException("String is not a rational number.");
                }

                IntegralPart = new List<int>();
                DecimalPart = new List<int>();
                Sign = value[0] != '-';

                int pointIndex = value.IndexOf('.');
                int len = value.Length;

                if (pointIndex > -1)
                {
                    for (int i = Sign ? 0 : 1; i < pointIndex; i++)
                    {
                        IntegralPart.Add(value[i] - '0');
                    }
                    for (int i = pointIndex + 1; i < len; i++)
                    {
                        DecimalPart.Add(value[i] - '0');
                    }
                }
                else
                {
                    for (int i = Sign ? 0 : 1; i < len; i++)
                    {
                        IntegralPart.Add(value[i] - '0');
                    }
                }

                if (IntegralPart.Count == 0) { IntegralPart.Add(0); }

                RemoveExtraZeroes();
                if (IntegralPart[0] == 0 && DecimalPart.Count == 0) { Sign = true; }
            }
        }

        public BigNumber(string Value)
        {
            this.Value = Value;
        }

        public BigNumber(decimal Value)
        {
            IntegralPart = new List<int>();
            DecimalPart = new List<int>();

            decimal abs = Math.Abs(Value);
            decimal i = Math.Truncate(abs);
            decimal d = abs - i;

            if (i == 0)
            {
                IntegralPart.Add(0);
            }
            else
            {
                while (i >= 1)
                {
                    IntegralPart.Insert(0, (int)(i % 10));
                    i /= 10;
                }
            }

            while (d > 0)
            {
                d *= 10;
                int digit = (int)d;
                DecimalPart.Add(digit);
                d -= digit;
            }
            Sign = Value >= 0;
        }

        private BigNumber()
        {
            Sign = true;
            IntegralPart = new List<int>();
            DecimalPart = new List<int>();
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
            List<int> i0 = new List<int>(IntegralPart);
            List<int> d0 = new List<int>(DecimalPart);
            List<int> i1 = new List<int>(other.IntegralPart);
            List<int> d1 = new List<int>(other.DecimalPart);

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
            List<int> lnz = new List<int>(n.IntegralPart);
            lnz.AddRange(n.DecimalPart);
            List<int> mnz = new List<int>(n1.IntegralPart);
            mnz.AddRange(n1.DecimalPart);

            // Put number with less non-zero characters in lnz
            int nnz = 0;
            foreach (int digit in lnz)
            {
                if (digit != 0) nnz++;
            }
            int n1nz = 0;
            foreach (int digit in mnz)
            {
                if (digit != 0) n1nz++;
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
                for (int j = 0; j < llen - 1 - i; j++) { mul.IntegralPart.Add(0); }

                for (int j = glen - 1; j >= 0; j--)
                {
                    int m = (mnz[j] * lnz[i]) + carry;
                    if (m > 9)
                    {
                        carry = m / 10;
                        m %= 10;
                    }
                    else { carry = 0; }

                    mul.IntegralPart.Insert(0, m);
                }
                if (carry > 0)
                {
                    mul.IntegralPart.Insert(0, carry);
                    carry = 0;
                }

                result += mul;
            }

            int decimalPlaces = n.DecimalPart.Count + n1.DecimalPart.Count;
            while (decimalPlaces >= result.IntegralPart.Count)
            {
                // Add 0s before number to be able to insert decimal point
                result.IntegralPart.Insert(0, 0);
            }
            int integralPlacess = result.IntegralPart.Count - decimalPlaces;

            result.DecimalPart = result.IntegralPart.GetRange(integralPlacess, decimalPlaces);
            result.IntegralPart = result.IntegralPart.GetRange(0, integralPlacess);
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

            int ndc = n.DecimalPart.Count;
            int n1dc = n1.DecimalPart.Count;
            int decimalPlaces = ndc > n1dc ? ndc : n1dc;
            while (decimalPlaces >= result.IntegralPart.Count)
            {
                // Add 0s before number to be able to insert decimal point
                result.IntegralPart.Insert(0, 0);
            }
            int integralPlacess = result.IntegralPart.Count - decimalPlaces;

            result.DecimalPart = result.IntegralPart.GetRange(integralPlacess, decimalPlaces);
            result.IntegralPart = result.IntegralPart.GetRange(0, integralPlacess);
            result.RemoveExtraZeroes();

            return result;
        }

        /// <summary>
        /// Returns the absolute value of this instance of BigNumber.
        /// </summary>
        /// <returns>A bigNumber.</returns>
        public BigNumber Abs()
        {
            return Sign ? MemberwiseClone() as BigNumber : -this;
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
            List<int> i0 = new List<int>(n.IntegralPart);
            List<int> d0 = new List<int>(n.DecimalPart);
            List<int> i1 = new List<int>(n1.IntegralPart);
            List<int> d1 = new List<int>(n1.DecimalPart);

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
                result.DecimalPart.Insert(0, sum);
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
                result.IntegralPart.Insert(0, sum);
            }

            if (carry == 1) { result.IntegralPart.Insert(0, 1); }

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
                i0 = new List<int>(n.IntegralPart);
                d0 = new List<int>(n.DecimalPart);
                i1 = new List<int>(n1.IntegralPart);
                d1 = new List<int>(n1.DecimalPart);
            }
            else
            {
                i0 = new List<int>(n1.IntegralPart);
                d0 = new List<int>(n1.DecimalPart);
                i1 = new List<int>(n.IntegralPart);
                d1 = new List<int>(n.DecimalPart);
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
                result.DecimalPart.Insert(0, dif);
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
                result.IntegralPart.Insert(0, dif);
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
        private static BigNumber DivideAndRemainder(BigNumber n, BigNumber n1, out BigNumber remainder)
        {
            if (n1 == zero) { throw new DivideByZeroException(); }

            BigNumber divisioned = (n.MemberwiseClone() as BigNumber).Abs();
            BigNumber divisor = (n1.MemberwiseClone() as BigNumber).Abs();
            BigNumber result = new BigNumber();

            // Get rid of decimals
            while (divisor.DecimalPart.Count > 0 || divisioned.DecimalPart.Count > 0)
            {
                divisioned *= ten;
                divisor *= ten;
            }

            BigNumber rem = zero.MemberwiseClone() as BigNumber;
            while (divisioned.IntegralPart.Count > 0 || divisioned > divisor)
            {
                BigNumber tempDiv = new BigNumber()
                {
                    IntegralPart = new List<int>(rem.IntegralPart)
                };
                tempDiv.IntegralPart.Add(divisioned.IntegralPart[0]);

                BigNumber tmp = new BigNumber(9);
                BigNumber mul;

                int i = 9;
                do
                {
                    mul = divisor * tmp;
                    if (mul <= tempDiv) { break; }
                    i--;
                    tmp.IntegralPart[0] = i;
                } while (true);

                result.IntegralPart.Add(i);

                rem = tempDiv - mul;
                divisioned.IntegralPart = divisioned.IntegralPart.GetRange(1, divisioned.IntegralPart.Count - 1);
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
            while (IntegralPart[0] == 0 && IntegralPart.Count > 1)
            {
                IntegralPart.RemoveAt(0);
            }
            while (DecimalPart.Count > 0 && DecimalPart[^1] == 0)
            {
                DecimalPart.RemoveAt(DecimalPart.Count - 1);
            }
        }
    }
}