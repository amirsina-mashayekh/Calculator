using System;
using System.Collections.Generic;
using BigNumbers;
using static BigNumbers.BigNumberMath;

namespace Calculator
{
    public static class Evaluator
    {
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
        /// <param name="Operator">The operator to be applied.</param>
        /// <param name="stack">The stack of <c>BigNumber</c>s.</param>
        /// <returns>A <c>BigNumber</c> showing the result of operation.</returns>
        private static BigNumber Operate(string Operator, Stack<BigNumber> stack)
        {
            BigNumber num = stack.Pop();

            return Operator switch
            {
                "+" => stack.Pop() + num,
                "-" => stack.Pop() - num,
                "*" => stack.Pop() * num,
                "/" => DivideWithDecimals(stack.Pop(), num),
                "%" => stack.Pop() % num,
                "^" => Exponent(stack.Pop(), num),
                "|" => num.Abs(),
                "!" => Factorial(num),
                "sin" => Sinus(num),
                "cos" => Cosinus(num),
                "tan" => Tangent(num),
                "cot" => Cotangent(num),
                _ => throw new ArgumentException("Invalid operator: " + Operator),
            };
        }
    }
}
