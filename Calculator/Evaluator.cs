using System;
using System.Collections.Generic;
using BigNumbers;
using static BigNumbers.BigNumberMath;

namespace Calculator
{
    public static class Evaluator
    {
        private class Operator
        {
            public string Token { get; }
            public int Precedence { get; }
            public Func<BigNumber, BigNumber,BigNumber> Operate { get; }
            public bool Unary { get; }

            public Operator(string token, int precedence, Func<BigNumber, BigNumber, BigNumber> operate, bool twoOperands)
            {
                Token = token ?? throw new ArgumentNullException(nameof(token));
                Precedence = precedence;
                Operate = operate ?? throw new ArgumentNullException(nameof(operate));
                Unary = twoOperands;
            }
        }

        private static readonly Operator[] Operators =
        {
            new Operator("+", 0, (n, n1) => n + n1, false),
            new Operator("-", 0, (n, n1) => n - n1, false),
            new Operator("*", 0, (n, n1) => n * n1, false),
            new Operator("/", 0, (n, n1) => DivideWithDecimals(n, n1), false),
            new Operator("mod", 0, (n, n1) => n % n1, false),
            new Operator("pow", 0, (n, n1) => Exponent(n, n1), false),
            new Operator("pos", 0, (n, n1) => n, true),
            new Operator("neg", 0, (n, n1) => -n, true),
            new Operator("abs", 0, (n, n1) => n.Abs(), true),
            new Operator("fact", 0, (n, n1) => Factorial(n), true),
            new Operator("sin", 0, (n, n1) => Sinus(n), true),
            new Operator("cos", 0, (n, n1) => Cosinus(n), true),
            new Operator("tan", 0, (n, n1) => Tangent(n), true),
            new Operator("cot", 0, (n, n1) => Cotangent(n), true)
        };

        /// <summary>
        /// Evaluates a Reverse Polish notation expression.
        /// </summary>
        /// <param name="rpn">An array of operators and <c>BigNumber</c>s in RPN order.</param>
        /// <returns>A <c>BigNumber</c> showing the result of expression.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static BigNumber EvaluateRPN(object[] rpn)
        {
            Stack<BigNumber> stack = new Stack<BigNumber>();

            foreach (object token in rpn)
            {
                if (token is BigNumber)
                {
                    stack.Push(token as BigNumber);
                }
                else if (token is string)
                {
                    stack.Push(Operate(token as string, stack));
                }
                else
                {
                    throw new ArgumentException("Invalid token in RPN expression.");
                }
            }

            return stack.Count == 1 ? stack.Pop() : throw new ArgumentException("Invalid expression.");
        }

        /// <summary>
        /// Applies the operator on last <c>BigNumber</c>(s) in a stack.
        /// </summary>
        /// <param name="op">The operator to be applied.</param>
        /// <param name="stack">The stack of <c>BigNumber</c>s.</param>
        /// <returns>A <c>BigNumber</c> showing the result of operation.</returns>
        private static BigNumber Operate(string op, Stack<BigNumber> stack)
        {
            BigNumber num = stack.Pop();

            foreach (Operator o in Operators)
            {
                if (o.Token == op)
                {
                    if (o.Unary)
                    {
                        return o.Operate(num, null);
                    }
                    else
                    {
                        return o.Operate(stack.Pop(), num);
                    }
                }
            }

            throw new ArgumentException("Invalid operator: " + op);
        }
    }
}