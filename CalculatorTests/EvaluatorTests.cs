using Calculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static Calculator.Evaluator;
using BigNumbers;
using static BigNumbers.BigNumberMath;
using System.Linq;

namespace Calculator.Tests
{
    [TestClass()]
    public class EvaluatorTests
    {
        // 5.3 * (2 + |sin(-pi/2) + cos(pi/4)|) - (tan(0) + cot(pi/3)) + 4! ^ 3 % 7
        // == 17.5759
        private static readonly BigNumber _5_3 = new BigNumber(5.3M);
        private static readonly BigNumber _2 = new BigNumber(2M);
        private static readonly BigNumber pi = new BigNumber((decimal)Math.PI);
        private static readonly BigNumber _3 = new BigNumber(3M);
        private static readonly BigNumber _4 = new BigNumber(4M);
        private static readonly BigNumber _0 = new BigNumber(0M);
        private static readonly BigNumber _7 = new BigNumber(7M);

        private static readonly object[] infix =
        {
            _5_3, "*", "(", _2, "+", "abs", "(", "sin", "(", "neg", pi, "/", _2, ")", "+", "cos", "(", pi, "/", _4, ")", ")", ")",
            "-", "(", "tan", "(", _0, ")", "+", "cot", "(", pi, "/", _3, ")", ")", "+", _4, "fact", "pow", _3, "mod", _7
        };

        private static readonly object[] rpn =
        {
            _5_3, _2, pi, "neg", _2, "/", "sin", pi, _4, "/", "cos", "+", "abs", "+", "*",
            _0, "tan", pi, _3, "/", "cot", "+", "-", _4, "fact", _3, "pow", _7, "mod", "+"
        };

        static EvaluatorTests()
        {
            int infixLen = infix.Length;
            int rpnLen = rpn.Length;

            for (int i = 0; i < infixLen; i++)
            {
                if (infix[i] is string)
                {
                    infix[i] = operators[infix[i] as string];
                }
            }
            for (int i = 0; i < rpnLen; i++)
            {
                if (rpn[i] is string)
                {
                    rpn[i] = operators[rpn[i] as string];
                }
            }
        }

        private static readonly BigNumber result =
            (_5_3 * (_2 + (Sinus(DivideWithDecimals(-pi, _2)) + Cosinus(DivideWithDecimals(pi, _4))).Abs()))
            - (Tangent(_0) + Cotangent(DivideWithDecimals(pi, _3))) + (Exponent(Factorial(_4), _3) % _7);

        [TestMethod()]
        public void EvaluateRPNTest()
        {
            Assert.AreEqual(result, EvaluateRPN(rpn));

            rpn[^1] = 2;
            _ = Assert.ThrowsException<ArgumentException>(() => EvaluateRPN(rpn));
            _ = Assert.ThrowsException<ArgumentException>(() => EvaluateRPN(rpn));
            rpn[^1] = _7;
            _ = Assert.ThrowsException<ArgumentException>(() => EvaluateRPN(rpn));

            rpn[^1] = operators["+"];
        }

        [TestMethod()]
        public void InfixToRPNTest()
        {
            Assert.IsTrue(rpn.SequenceEqual(InfixToRPN(infix)));
        }
    }
}