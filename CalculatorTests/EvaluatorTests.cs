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
        private static readonly string expression =
            "5.3 * (2 + abs(sin(-3.1415926535897931/2) + cos(3.1415926535897931/4))) " +
            "- (tan(0) + cot(3.1415926535897931/3)) + fact(4) pow 3 mod 7";
        private static readonly BigNumber _5_3 = new BigNumber(5.3M);
        private static readonly BigNumber _2 = new BigNumber(2M);
        private static readonly BigNumber pi = new BigNumber("3.1415926535897931");
        private static readonly BigNumber _3 = new BigNumber(3M);
        private static readonly BigNumber _4 = new BigNumber(4M);
        private static readonly BigNumber _0 = new BigNumber(0M);
        private static readonly BigNumber _7 = new BigNumber(7M);

        private static readonly object[] infix =
        {
            _5_3, "*", "(", _2, "+", "abs", "(", "sin", "(", "neg", pi, "/", _2, ")", "+", "cos", "(", pi, "/", _4, ")", ")", ")",
            "-", "(", "tan", "(", _0, ")", "+", "cot", "(", pi, "/", _3, ")", ")", "+", "fact", "(", _4, ")", "pow", _3, "mod", _7
        };

        private static readonly object[] rpn =
        {
            _5_3, _2, pi, "neg", _2, "/", "sin", pi, _4, "/", "cos", "+", "abs", "+", "*",
            _0, "tan", pi, _3, "/", "cot", "+", "-", _4, "fact", _3, "pow", _7, "mod", "+"
        };

        private static readonly BigNumber result =
            (_5_3 * (_2 + (Sinus(DivideWithDecimals(-pi, _2)) + Cosinus(DivideWithDecimals(pi, _4))).Abs()))
            - (Tangent(_0) + Cotangent(DivideWithDecimals(pi, _3))) + (Power(Factorial(_4), _3) % _7);
        // == 17.5759

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
            object[] result = InfixToRPN(infix);
            Assert.IsTrue(rpn.SequenceEqual(result));

            infix[^1] = 1;
            _ = Assert.ThrowsException<ArgumentException>(() => InfixToRPN(infix));

            infix[^1] = _7;
        }

        [TestMethod()]
        public void TokenizeTest()
        {
            object[] result = Tokenize(expression);
            Assert.IsTrue(infix.SequenceEqual(result));
        }
    }
}