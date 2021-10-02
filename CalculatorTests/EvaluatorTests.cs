using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static Calculator.Evaluator;
using BigNumbers;
using static BigNumbers.BigNumberMath;

namespace Calculator.Tests
{
    [TestClass()]
    public class EvaluatorTests
    {
        [TestMethod()]
        public void EvaluateRPNTest()
        {
            // 5.3 * (2 + |sin(-pi/2) + cos(pi/4)|) - (tan(0) + cot(pi/3)) + 4! ^ 3 % 7
            // == 17.5759
            BigNumber _5_3 = new BigNumber(5.3M);
            BigNumber _2 = new BigNumber(2M);
            BigNumber pi = new BigNumber((decimal)Math.PI);
            BigNumber _3 = new BigNumber(3M);
            BigNumber _4 = new BigNumber(4M);
            BigNumber _0 = new BigNumber(0M);
            BigNumber _7 = new BigNumber(7M);

            object[] test =
            {
                _5_3, _2, -pi, _2, "/", "sin", pi, _4, "/", "cos", "+", "|", "+", "*",
                _0, "tan", pi, _3, "/", "cot", "+", "-", _4, "!", _3, "^", _7, "%", "+"
            };

            BigNumber result =
                (_5_3 * (_2 + (Sinus(DivideWithDecimals(-pi, _2)) + Cosinus(DivideWithDecimals(pi, _4))).Abs()))
                - (Tangent(_0) + Cotangent(DivideWithDecimals(pi, _3))) + (Exponent(Factorial(_4), _3) % _7);

            Assert.AreEqual(result, EvaluateRPN(test));

            test[^1] = 2;
            _ = Assert.ThrowsException<ArgumentException>(() => EvaluateRPN(test));
            test[^1] = "Hello!";
            _ = Assert.ThrowsException<ArgumentException>(() => EvaluateRPN(test));
            test[^1] = _7;
            _ = Assert.ThrowsException<ArgumentException>(() => EvaluateRPN(test));
        }
    }
}