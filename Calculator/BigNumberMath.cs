using System;

namespace Calculator
{
    /// <summary>
    /// Advanced arithmetic for BigNumbers.
    /// </summary>
    public class BigNumberMath
    {
        private static readonly BigNumber zero = new BigNumber("0");

        private static readonly BigNumber one = new BigNumber("1");

        private static readonly BigNumber ten = new BigNumber("10");

        public static BigNumber DivideWithDecimals(BigNumber n, BigNumber n1, int decimalPoints = 10)
        {
            if (decimalPoints < 0)
            {
                throw new ArgumentOutOfRangeException("Decimal points should be at least 0.");
            }

            BigNumber divisioned = new BigNumber(n.Value);
            BigNumber divisor = new BigNumber(n1.Value);

            for (uint i = 0; i < decimalPoints; i++)
            {
                divisioned *= ten;
            }

            BigNumber result = divisioned / divisor;
            string rstr = result.Abs().Value;

            while (decimalPoints >= rstr.Length)
            {
                // Add 0s before number to be able to insert decimal point
                rstr = '0' + rstr;
            }

            return new BigNumber((result.Sign ? "" : "-") + rstr.Insert(rstr.Length - decimalPoints, "."));
        }

        public static BigNumber Factorial(BigNumber n)
        {
            if (!n.Sign || n.DecimalPart != "")
            {
                throw new ArithmeticException("Factorial is only supported for zero and positive integers.");
            }

            BigNumber result = new BigNumber("1");

            for (BigNumber i = new BigNumber("1"); i <= n; i++)
            {
                result *= i;
            }

            return result;
        }

        public static BigNumber Exponent(BigNumber n, BigNumber n1)
        {
            if (n1.DecimalPart != "")
            {
                throw new ArithmeticException("Decimal power is not supported yet.");
            }

            if (n1 == zero && n == n1)
            {
                throw new ArithmeticException("Zero raised to zero is undefined.");
            }

            BigNumber result = new BigNumber("1");

            if (n1.Sign)
            {
                for (BigNumber i = new BigNumber("0"); i < n1; i++)
                {
                    result *= n;
                }
            }
            else
            {
                result = DivideWithDecimals(one, Exponent(n, -n1));
            }

            return result;
        }
    }
}
