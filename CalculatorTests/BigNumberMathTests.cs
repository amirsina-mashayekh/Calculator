using Calculator;
using System;
using static Calculator.BigNumberMath;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass()]
    public class BigNumberMathTests
    {
        private readonly BigNumber zero = new BigNumber(0);

        private readonly BigNumber one = new BigNumber(1);

        [TestMethod(), Timeout(100)]
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

        [TestMethod(), Timeout(100)]
        public void ExponentTest()
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
                { "-3", "-3", "-0.037037037" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber pow = Exponent(n, n1);
                Assert.AreEqual(tests[i, 2], pow.Value);
            }
            _ = Assert.ThrowsException<ArithmeticException>(() => Exponent(zero, zero));
            _ = Assert.ThrowsException<ArithmeticException>(() => Exponent(one, new BigNumber(1.1M)));
        }

        [TestMethod(), Timeout(100)]
        public void DivideWithDecimalsTest()
        {
            string[,] tests =
            {
                { "0", "1", "5", "0" },
                { "1", "10", "10", "0.1" },
                { "2", "3", "10", "0.6666666666" },
                { "5", "2", "10", "2.5" },
                { "2299", "66", "20", "34.83333333333333333333" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber div = DivideWithDecimals(n, n1, int.Parse(tests[i, 2]));
                Assert.AreEqual(tests[i, 3], div.Value);
            }

            _ = Assert.ThrowsException<ArithmeticException>(() => DivideWithDecimals(zero, zero));
        }

        [TestMethod()]
        public void SinusTest()
        {
            Assert.AreEqual(zero, Sinus(zero));
            Assert.AreEqual(zero, Sinus(new BigNumber((decimal)Math.PI)));

            Assert.AreEqual(one, Sinus(new BigNumber((decimal)(Math.PI / 2))));
            Assert.AreEqual(-one, Sinus(new BigNumber((decimal)(3 * Math.PI / 2))));
            Assert.AreEqual(-one, Sinus(new BigNumber((decimal)(-Math.PI / 2))));

            Assert.AreEqual(new BigNumber(0.71M), Sinus(new BigNumber((decimal)(Math.PI / 4))));
            Assert.AreEqual(new BigNumber(0.71M), Sinus(new BigNumber((decimal)(3 * Math.PI / 4))));
            Assert.AreEqual(new BigNumber(-0.71M), Sinus(new BigNumber((decimal)(-Math.PI / 4))));
            Assert.AreEqual(new BigNumber(-0.71M), Sinus(new BigNumber((decimal)(-11 * Math.PI / 4))));

            Assert.AreEqual(new BigNumber(0.5M), Sinus(new BigNumber((decimal)(Math.PI / 6))));
            Assert.AreEqual(new BigNumber(0.5M), Sinus(new BigNumber((decimal)(5 * Math.PI / 6))));
            Assert.AreEqual(new BigNumber(-0.5M), Sinus(new BigNumber((decimal)(7 * Math.PI / 6))));
            Assert.AreEqual(new BigNumber(-0.5M), Sinus(new BigNumber((decimal)(11 * Math.PI / 6))));
            Assert.AreEqual(new BigNumber(-0.5M), Sinus(new BigNumber((decimal)(-Math.PI / 6))));
        }

        [TestMethod()]
        public void CosinusTest()
        {
            Assert.AreEqual(one, Cosinus(zero));
            Assert.AreEqual(-one, Cosinus(new BigNumber((decimal)Math.PI)));

            Assert.AreEqual(zero, Cosinus(new BigNumber((decimal)(Math.PI / 2))));
            Assert.AreEqual(-zero, Cosinus(new BigNumber((decimal)(3 * Math.PI / 2))));
            Assert.AreEqual(-zero, Cosinus(new BigNumber((decimal)(-Math.PI / 2))));

            Assert.AreEqual(new BigNumber(0.71M), Cosinus(new BigNumber((decimal)(Math.PI / 4))));
            Assert.AreEqual(new BigNumber(-0.71M), Cosinus(new BigNumber((decimal)(3 * Math.PI / 4))));
            Assert.AreEqual(new BigNumber(0.71M), Cosinus(new BigNumber((decimal)(-Math.PI / 4))));
            Assert.AreEqual(new BigNumber(-0.71M), Cosinus(new BigNumber((decimal)(-3 * Math.PI / 4))));

            Assert.AreEqual(new BigNumber(0.5M), Cosinus(new BigNumber((decimal)(Math.PI / 3))));
            Assert.AreEqual(new BigNumber(-0.5M), Cosinus(new BigNumber((decimal)(2 * Math.PI / 3))));
            Assert.AreEqual(new BigNumber(-0.5M), Cosinus(new BigNumber((decimal)(4 * Math.PI / 3))));
            Assert.AreEqual(new BigNumber(0.5M), Cosinus(new BigNumber((decimal)(13 * Math.PI / 3))));
            Assert.AreEqual(new BigNumber(0.5M), Cosinus(new BigNumber((decimal)(-13 * Math.PI / 3))));
        }
    }
}