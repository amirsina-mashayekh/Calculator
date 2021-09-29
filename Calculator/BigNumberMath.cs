using System;

namespace Calculator
{
    /// <summary>
    /// Advanced arithmetic for BigNumbers.
    /// </summary>
    public class BigNumberMath
    {
        private static readonly BigNumber zero = new BigNumber(0);

        private static readonly BigNumber one = new BigNumber(1);

        private static readonly BigNumber ten = new BigNumber(10);

        private static readonly BigNumber twoPi = new BigNumber((decimal)Math.PI * 2);

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

            BigNumber result = new BigNumber(1);

            for (BigNumber i = new BigNumber(1); i <= n; i++)
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

            BigNumber result = new BigNumber(1);

            if (n1.Sign)
            {
                for (BigNumber i = new BigNumber(0); i < n1; i++)
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

        public static BigNumber Sinus(BigNumber n)
        {
            // Cosinus taylor calculation is faster so we calculate
            // cosinus of complement of n which is equal to sinus of it.
            return Cosinus(new BigNumber((decimal)Math.PI / 2) - n);
        }

        public static BigNumber Cosinus(BigNumber n)
        {
            BigNumber num = n - (n / twoPi * twoPi);
            BigNumber k = new BigNumber(1000);
            BigNumber cos = new BigNumber(1) * k;

            BigNumber mul = num * num;
            BigNumber p = mul * k;

            BigNumber f = new BigNumber(2);

            bool sign = false;

            for (int i = 2; i <= 8; i++)
            {
                cos += sign ? p / f : -p / f;
                sign = !sign;
                p *= mul;
                f *= new BigNumber(2 * i * ((2 * i) - 1));
            }

            if (cos != one) { cos = DivideWithDecimals(cos, k); }

            // Limit the answer to 2 decimals
            if (cos.DecimalPart.Length > 2)
            {
                if (cos.DecimalPart[2] > '4')
                {
                    cos += cos.Sign ? new BigNumber(0.01M) : new BigNumber(-0.01M);
                }
                cos = new BigNumber((cos.Sign ? "" : "-") + cos.IntegralPart + '.' + cos.DecimalPart.Substring(0, 2));
            }
            return cos;
        }

        public static BigNumber Tangent(BigNumber n)
        {
            BigNumber tan = DivideWithDecimals(Sinus(n), Cosinus(n));

            if (tan.DecimalPart.Length > 2)
            {
                if (tan.DecimalPart[2] > '4')
                {
                    tan += tan.Sign ? new BigNumber(0.01M) : new BigNumber(-0.01M);
                }
                tan = new BigNumber((tan.Sign ? "" : "-") + tan.IntegralPart + '.' + tan.DecimalPart.Substring(0, 2));
            }

            return tan;
        }

        public static BigNumber Cotangent(BigNumber n)
        {
            BigNumber cot = DivideWithDecimals(Cosinus(n), Sinus(n));

            if (cot.DecimalPart.Length > 2)
            {
                if (cot.DecimalPart[2] > '4')
                {
                    cot += cot.Sign ? new BigNumber(0.01M) : new BigNumber(-0.01M);
                }
                cot = new BigNumber((cot.Sign ? "" : "-") + cot.IntegralPart + '.' + cot.DecimalPart.Substring(0, 2));
            }

            return cot;
        }
    }
}
