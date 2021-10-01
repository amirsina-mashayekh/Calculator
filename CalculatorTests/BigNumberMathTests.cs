using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static BigNumbers.BigNumberMath;

namespace BigNumbers.Tests
{
    [TestClass()]
    public class BigNumberMathTests
    {
        private readonly BigNumber zero = new BigNumber(0);

        private readonly decimal pi = (decimal)Math.PI;

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
            _ = Assert.ThrowsException<ArithmeticException>(() => Exponent(new BigNumber(1), new BigNumber(1.1M)));
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
            Assert.AreEqual("0", Sinus(zero).Value);
            Assert.AreEqual("0", Sinus(new BigNumber(pi)).Value);

            Assert.AreEqual("1", Sinus(new BigNumber(pi / 2)).Value);
            Assert.AreEqual("-1", Sinus(new BigNumber(3 * pi / 2)).Value);
            Assert.AreEqual("-1", Sinus(new BigNumber(-pi / 2)).Value);

            Assert.AreEqual("0.866", Sinus(new BigNumber(pi / 3)).Value);
            Assert.AreEqual("0.707", Sinus(new BigNumber(pi / 4)).Value);
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
            Assert.AreEqual("0.707", Cosinus(new BigNumber(pi / 4)).Value);
            Assert.AreEqual("0.866", Cosinus(new BigNumber(pi / 6)).Value);
        }

        [TestMethod()]
        public void TangentTest()
        {
            Assert.AreEqual("0", Tangent(zero).Value);
            Assert.AreEqual("0", Tangent(new BigNumber(pi)).Value);
            Assert.AreEqual("0", Tangent(new BigNumber(-pi)).Value);

            Assert.AreEqual("1.732", Tangent(new BigNumber(pi / 3)).Value);
            Assert.AreEqual("1", Tangent(new BigNumber(pi / 4)).Value);
            Assert.AreEqual("0.577", Tangent(new BigNumber(pi / 6)).Value);

            _ = Assert.ThrowsException<ArithmeticException>(() => Tangent(new BigNumber(pi / 2)));
            _ = Assert.ThrowsException<ArithmeticException>(() => Tangent(new BigNumber(3 * pi / 2)));
            _ = Assert.ThrowsException<ArithmeticException>(() => Tangent(new BigNumber(-pi / 2)));
        }

        [TestMethod()]
        public void CotangentTest()
        {
            Assert.AreEqual("0", Cotangent(new BigNumber(pi / 2)).Value);
            Assert.AreEqual("-0.577", Cotangent(new BigNumber(-pi / 3)).Value);
            Assert.AreEqual("-1", Cotangent(new BigNumber(-pi / 4)).Value);
            Assert.AreEqual("1.732", Cotangent(new BigNumber(pi / 6)).Value);

            _ = Assert.ThrowsException<ArithmeticException>(() => Cotangent(zero));
            _ = Assert.ThrowsException<ArithmeticException>(() => Cotangent(new BigNumber(pi)));
            _ = Assert.ThrowsException<ArithmeticException>(() => Cotangent(new BigNumber(-2 * pi)));
        }
    }
}