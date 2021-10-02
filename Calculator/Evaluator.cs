using System;
using System.Collections.Generic;
using BigNumbers;
using static BigNumbers.BigNumberMath;

namespace Calculator
{
    public static class Evaluator
    {
        public class Operator
        {
            public Func<BigNumber, BigNumber,BigNumber> Operate { get; }
            public int Precedence { get; }
            public bool Unary { get; }

            public Operator(Func<BigNumber, BigNumber, BigNumber> operate, int precedence, bool twoOperands)
            {
                Operate = operate ?? throw new ArgumentNullException(nameof(operate));
                Precedence = precedence;
                Unary = twoOperands;
            }
        }

        public static readonly Dictionary<string, Operator> operators = new Dictionary<string, Operator>
        {
            { "(", new Operator((n, n1) => null, 0, false) },
            { ")", new Operator((n, n1) => null, 0, false) },
            { "+", new Operator((n, n1) => n + n1, 3, false) },
            { "-", new Operator((n, n1) => n - n1, 3, false) },
            { "*", new Operator((n, n1) => n * n1, 2, false) },
            { "/", new Operator((n, n1) => DivideWithDecimals(n, n1), 2, false) },
            { "mod", new Operator((n, n1) => n % n1, 2, false) },
            { "pow", new Operator((n, n1) => Exponent(n, n1), 1, false) },
            { "pos", new Operator((n, n1) => n, 2, true) },
            { "neg", new Operator((n, n1) => -n, 2, true) },
            { "abs", new Operator((n, n1) => n.Abs(), 0, true) },
            { "fact", new Operator((n, n1) => Factorial(n), 0, true) },
            { "sin", new Operator((n, n1) => Sinus(n), 0, true) },
            { "cos", new Operator((n, n1) => Cosinus(n), 0, true) },
            { "tan", new Operator((n, n1) => Tangent(n), 0, true) },
            { "cot", new Operator((n, n1) => Cotangent(n), 0, true) }
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
                else if (token is Operator)
                {
                    stack.Push(Operate(token as Operator, stack));
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
        private static BigNumber Operate(Operator op, Stack<BigNumber> stack)
        {
            BigNumber num = stack.Pop();

            if (op.Unary)
            {
                return op.Operate(num, null);
            }
            else
            {
                return op.Operate(stack.Pop(), num);
            }
        }

        /// <summary>
        /// Converts an infix expression to Reverse Polish notation expression.
        /// </summary>
        /// <param name="infix">An array of operators and <c>BigNumber</c>s in infix order.</param>
        /// <returns>
        /// An array of operators and <c>BigNumber</c>s
        /// which is RPN equivalent for <c>infix</c>.
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public static object[] InfixtoRPN(object[] infix)
        {
            Stack<Operator> ops = new Stack<Operator>();
            List<object> rpn = new List<object>();

            foreach (object token in infix)
            {
                if (token is BigNumber)
                {
                    rpn.Add(token);
                }
                else if (token is Operator)
                {
                    Operator t = token as Operator;
                    if (t == operators["("])
                    {
                        ops.Push(t);
                    }
                    else if (t == operators[")"])
                    {
                        while (ops.Peek() != operators["("])
                        {
                            rpn.Add(ops.Pop());
                        }
                        _ = ops.Pop();
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid token in RPN expression.");
                }
            }
            throw new NotImplementedException();
        }

        private static bool UnwindOperators(Stack<string> ops, Operator nextToken)
        {
            throw new NotImplementedException();
        }
    }
}