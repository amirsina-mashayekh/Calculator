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
                BigNumber res = new BigNumber(tests[i, 1]);
                BigNumber fact = Factorial(n);
                Debug.Print("res: " + res + "    fact:" + fact);
                Assert.IsTrue(fact == res);
            }

            _ = Assert.ThrowsException<ArithmeticException>(() => Factorial(new BigNumber("-1")));
            _ = Assert.ThrowsException<ArithmeticException>(() => Factorial(new BigNumber("1.1")));
            _ = Assert.ThrowsException<ArithmeticException>(() => Factorial(new BigNumber("-1.1")));
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
                { "-3", "-3", "-0.0370370370" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber res = new BigNumber(tests[i, 2]);
                BigNumber pow = Exponent(n, n1);
                Debug.Print("res: " + res + "    pow:" + pow);
                Assert.IsTrue(pow == res);
            }

            BigNumber zero = new BigNumber("0");
            _ = Assert.ThrowsException<ArithmeticException>(() => Exponent(zero, zero));
            _ = Assert.ThrowsException<ArithmeticException>(() => Exponent(new BigNumber("1"), new BigNumber("1.1")));
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
                BigNumber res = new BigNumber(tests[i, 3]);
                BigNumber div = DivideWithDecimals(n, n1, int.Parse(tests[i, 2]));
                Debug.Print("res: " + res + "    div:" + div);
                Assert.IsTrue(div == res);
            }

            BigNumber zero = new BigNumber("0");
            _ = Assert.ThrowsException<ArithmeticException>(() => DivideWithDecimals(zero, zero));
        }
    }
}