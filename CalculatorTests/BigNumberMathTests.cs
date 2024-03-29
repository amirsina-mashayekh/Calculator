﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static BigNumbers.BigNumberMath;

namespace BigNumbers.Tests
{
    [TestClass()]
    public class BigNumberMathTests
    {
        private readonly BigNumber zero = new BigNumber(0);

        private readonly decimal pi = (decimal)Math.PI;

        [TestMethod()]
        public void FactorialTest()
        {
            string[,] tests =
            {
                { "0", "1" },
                { "1", "1" },
                { "2", "2" },
                { "4", "24" },
                { "4", "24" },
                { "20", "2432902008176640000" },
                { "29", "8841761993739701954543616000000" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber fact = Factorial(n);
                Assert.AreEqual(tests[i, 1], fact.Value);
            }

            _ = Assert.ThrowsException<ArithmeticException>(() => Factorial(new BigNumber(-1)));
            _ = Assert.ThrowsException<ArithmeticException>(() => Factorial(new BigNumber(1.1M)));
            _ = Assert.ThrowsException<ArithmeticException>(() => Factorial(new BigNumber(-1.1M)));
        }

        [TestMethod()]
        public void PowerTest()
        {
            string[,] tests =
            {
                { "0", "1", "0" },
                { "1", "100", "1" },
                { "2", "2", "4" },
                { "9", "9", "387420489" },
                { "-2", "2", "4" },
                { "-2", "0", "1" },
                { "3.3", "3", "35.937" },
                { "2", "-2", "0.25" },
                { "-3", "-3", "-0.03703703703703703703703703703704" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber pow = Power(n, n1);
                Assert.AreEqual(tests[i, 2], pow.Value);
            }
            _ = Assert.ThrowsException<ArithmeticException>(() => Power(zero, zero));
            _ = Assert.ThrowsException<ArithmeticException>(() => Power(new BigNumber(1), new BigNumber(1.1M)));
        }

        [TestMethod()]
        public void DivideWithDecimalsTest()
        {
            string[,] tests =
            {
                { "0", "1", "5", "0" },
                { "1", "1", "32", "1" },
                { "1", "10", "10", "0.1" },
                { "2", "3", "10", "0.6666666667" },
                { "5", "2", "10", "2.5" },
                { "2299", "66", "32", "34.83333333333333333333333333333333" },
                { "200", "299", "32", "0.6688963210702341137123745819398" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber div = DivideWithDecimals(n, n1, int.Parse(tests[i, 2]));
                Assert.AreEqual(tests[i, 3], div.Value);
            }

            _ = Assert.ThrowsException<DivideByZeroException>(() => DivideWithDecimals(zero, zero));
        }

        [TestMethod()]
        public void SinusTest()
        {
            Assert.AreEqual("0", Sinus(zero).Value);
            Assert.AreEqual("0", Sinus(new BigNumber(pi)).Value);

            Assert.AreEqual("1", Sinus(new BigNumber(pi / 2)).Value);
            Assert.AreEqual("-1", Sinus(new BigNumber(3 * pi / 2)).Value);
            Assert.AreEqual("-1", Sinus(new BigNumber(-pi / 2)).Value);

            Assert.AreEqual("0.86602540378", Sinus(new BigNumber(pi / 3)).Value);
            Assert.AreEqual("-0.70710678119", Sinus(new BigNumber(-pi / 4)).Value);
            Assert.AreEqual("0.5", Sinus(new BigNumber(pi / 6)).Value);
        }

        [TestMethod()]
        public void CosinusTest()
        {
            Assert.AreEqual("1", Cosinus(zero).Value);
            Assert.AreEqual("-1", Cosinus(new BigNumber(pi)).Value);

            Assert.AreEqual("0", Cosinus(new BigNumber(pi / 2)).Value);
            Assert.AreEqual("0", Cosinus(new BigNumber(11 * pi / 2)).Value);
            Assert.AreEqual("0", Cosinus(new BigNumber(-pi / 2)).Value);

            Assert.AreEqual("0.5", Cosinus(new BigNumber(pi / 3)).Value);
            Assert.AreEqual("0.70710678119", Cosinus(new BigNumber(pi / 4)).Value);
            Assert.AreEqual("0.86602540378", Cosinus(new BigNumber(pi / 6)).Value);
        }

        [TestMethod()]
        public void TangentTest()
        {
            Assert.AreEqual("0", Tangent(zero).Value);
            Assert.AreEqual("0", Tangent(new BigNumber(pi)).Value);
            Assert.AreEqual("0", Tangent(new BigNumber(-pi)).Value);

            Assert.AreEqual("1.73205080756", Tangent(new BigNumber(pi / 3)).Value);
            Assert.AreEqual("1", Tangent(new BigNumber(pi / 4)).Value);
            Assert.AreEqual("0.57735026919", Tangent(new BigNumber(pi / 6)).Value);

            _ = Assert.ThrowsException<DivideByZeroException>(() => Tangent(new BigNumber(pi / 2)));
            _ = Assert.ThrowsException<DivideByZeroException>(() => Tangent(new BigNumber(3 * pi / 2)));
            _ = Assert.ThrowsException<DivideByZeroException>(() => Tangent(new BigNumber(-pi / 2)));
        }

        [TestMethod()]
        public void CotangentTest()
        {
            Assert.AreEqual("0", Cotangent(new BigNumber(pi / 2)).Value);
            Assert.AreEqual("-0.57735026919", Cotangent(new BigNumber(-pi / 3)).Value);
            Assert.AreEqual("-1", Cotangent(new BigNumber(-pi / 4)).Value);
            Assert.AreEqual("1.73205080756", Cotangent(new BigNumber(pi / 6)).Value);

            _ = Assert.ThrowsException<DivideByZeroException>(() => Cotangent(zero));
            _ = Assert.ThrowsException<DivideByZeroException>(() => Cotangent(new BigNumber(pi)));
            _ = Assert.ThrowsException<DivideByZeroException>(() => Cotangent(new BigNumber(-2 * pi)));
        }

        [TestMethod()]
        public void FloorTest()
        {
            Assert.AreEqual("1", Floor(new BigNumber(1M)).Value);
            Assert.AreEqual("1", Floor(new BigNumber(1.1M)).Value);
            Assert.AreEqual("1", Floor(new BigNumber(1.9M)).Value);
            Assert.AreEqual("-1", Floor(new BigNumber(-1M)).Value);
            Assert.AreEqual("-2", Floor(new BigNumber(-1.1M)).Value);
            Assert.AreEqual("-2", Floor(new BigNumber(-1.9M)).Value);
        }

        [TestMethod()]
        public void CeilTest()
        {
            Assert.AreEqual("1", Ceil(new BigNumber(1M)).Value);
            Assert.AreEqual("2", Ceil(new BigNumber(1.1M)).Value);
            Assert.AreEqual("2", Ceil(new BigNumber(1.9M)).Value);
            Assert.AreEqual("-1", Ceil(new BigNumber(-1M)).Value);
            Assert.AreEqual("-1", Ceil(new BigNumber(-1.1M)).Value);
            Assert.AreEqual("-1", Ceil(new BigNumber(-1.9M)).Value);
        }
    }
}