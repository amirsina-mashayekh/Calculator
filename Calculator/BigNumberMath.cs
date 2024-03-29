﻿using System;
using System.Text;

namespace BigNumbers
{
    /// <summary>
    /// Advanced arithmetic for BigNumbers.
    /// </summary>
    public static class BigNumberMath
    {
        private static readonly BigNumber zero = new BigNumber(0);

        private static readonly BigNumber one = new BigNumber(1);

        private static readonly BigNumber ten = new BigNumber(10);

        private static readonly BigNumber twoPi = new BigNumber((decimal)Math.PI * 2);

        public static BigNumber Floor(BigNumber n)
        {
            BigNumber result = new BigNumber(n.Value);
            if (n.DecimalPart.Length == 0) { return result; }

            result = new BigNumber(result.IntegralPart);

            return n.Sign ? result : -result - one;
        }

        public static BigNumber Ceil(BigNumber n)
        {
            BigNumber result = new BigNumber(n.Value);
            if (n.DecimalPart.Length == 0) { return result; }

            result = new BigNumber(result.IntegralPart);

            return n.Sign ? result + one : -result;
        }

        public static BigNumber DivideWithDecimals(BigNumber n, BigNumber n1, int decimals = 32)
        {
            if (decimals < 0)
            {
                throw new ArgumentOutOfRangeException("Decimal points should be at least 0.");
            }

            BigNumber divisioned = new BigNumber(n.Value);
            BigNumber divisor = new BigNumber(n1.Value);

            for (int i = 0; i <= decimals; i++)
            {
                divisioned *= ten;
            }

            BigNumber result = divisioned / divisor;
            StringBuilder rstr = new StringBuilder(result.Abs().Value);

            while (decimals >= rstr.Length)
            {
                // Add 0s before number to be able to insert decimal point
                _ = rstr.Insert(0, '0');
            }
            _ = rstr.Insert(rstr.Length - decimals - 1, ".");
            if (!result.Sign) { _ = rstr.Insert(0, '-'); }

            return new BigNumber(rstr.ToString()).Round(decimals);
        }

        public static BigNumber Factorial(BigNumber n)
        {
            if (!n.Sign || n.DecimalPart.Length != 0)
            {
                throw new ArithmeticException("Factorial is only supported for zero and positive integers.");
            }

            BigNumber result = new BigNumber(1);

            for (BigNumber i = new BigNumber(n.Value); i > one; i--)
            {
                result = i * result;
            }

            return result;
        }

        public static BigNumber Power(BigNumber n, BigNumber n1)
        {
            if (n1.DecimalPart.Length != 0)
            {
                throw new ArithmeticException("Decimal exponent is not supported yet.");
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
                result = DivideWithDecimals(one, Power(n, -n1));
            }

            return result;
        }

        public static BigNumber Sinus(BigNumber n)
        {
            // Cosinus taylor calculation is faster so we calculate
            // cosinus of complement of n which is equal to sinus of n.
            return Cosinus(new BigNumber((decimal)Math.PI / 2) - n);
        }

        public static BigNumber Cosinus(BigNumber n)
        {
            double num = (double)(n % twoPi).ToDecimal();
            double cos = 1;

            double mul = num * num;
            double p = mul;

            double f = 2;

            bool sign = false;

            for (int i = 2; i <= 20; i++)
            {
                cos += sign ? p / f : -p / f;
                sign = !sign;
                p *= mul;
                f *= 2 * i * ((2 * i) - 1);
            }

            cos = Math.Round(cos, 11);

            return new BigNumber((decimal)cos);
        }

        public static BigNumber Tangent(BigNumber n)
        {
            BigNumber tan = DivideWithDecimals(Sinus(n), Cosinus(n), 11);

            return tan;
        }

        public static BigNumber Cotangent(BigNumber n)
        {
            BigNumber cot = DivideWithDecimals(Cosinus(n), Sinus(n), 11);

            return cot;
        }
    }
}