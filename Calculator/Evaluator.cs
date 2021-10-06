using BigNumbers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BigNumbers.BigNumberMath;

namespace Evaluation
{
    public static class Evaluator
    {
        public class Operator
        {
            public string Token { get; }
            public Func<BigNumber, BigNumber, BigNumber> Operate { get; }
            public int Precedence { get; }
            public bool Unary { get; }

            public Operator(string token, Func<BigNumber, BigNumber, BigNumber> operate, int precedence, bool twoOperands)
            {
                Token = token ?? throw new ArgumentNullException(nameof(token));
                Operate = operate ?? throw new ArgumentNullException(nameof(operate));
                Precedence = precedence;
                Unary = twoOperands;
            }

            public override string ToString()
            {
                return Token;
            }
        }

        public static readonly Dictionary<string, Operator> operators = new Dictionary<string, Operator>
        {
            { "(", new Operator("(", (n, n1) => null, 1000, false) },
            { ")", new Operator(")", (n, n1) => null, 1000, false) },
            { "+", new Operator("+", (n, n1) => n + n1, 4, false) },
            { "-", new Operator("-", (n, n1) => n - n1, 4, false) },
            { "*", new Operator("*", (n, n1) => n * n1, 3, false) },
            { "/", new Operator("/", (n, n1) => DivideWithDecimals(n, n1), 3, false) },
            { "mod", new Operator("mod", (n, n1) => n % n1, 3, false) },
            { "pow", new Operator("pow", (n, n1) => Power(n, n1), 1, false) },
            { "pos", new Operator("pos", (n, n1) => n, 2, true) },
            { "neg", new Operator("neg", (n, n1) => -n, 2, true) },
            { "abs", new Operator("abs", (n, n1) => n.Abs(), 0, true) },
            { "floor", new Operator("floor", (n, n1) => Floor(n), 0, true) },
            { "ceil", new Operator("ceil", (n, n1) => Ceil(n), 0, true) },
            { "fact", new Operator("fact", (n, n1) => Factorial(n), 0, true) },
            { "sin", new Operator("sin", (n, n1) => Sinus(n), 0, true) },
            { "cos", new Operator("cos", (n, n1) => Cosinus(n), 0, true) },
            { "tan", new Operator("tan", (n, n1) => Tangent(n), 0, true) },
            { "cot", new Operator("cot", (n, n1) => Cotangent(n), 0, true) }
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

            return op.Unary ? op.Operate(num, null) : op.Operate(stack.Pop(), num);
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
        public static object[] InfixToRPN(object[] infix)
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
                        while (UnwindOperators(ops, t))
                        {
                            rpn.Add(ops.Pop());
                        }
                        ops.Push(t);
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid token in RPN expression.");
                }
            }

            while (ops.Count > 0)
            {
                rpn.Add(ops.Pop());
            }
            return rpn.ToArray();
        }

        /// <summary>
        /// Checks if operator stack should be unwinded.
        /// </summary>
        /// <param name="ops">The stack of operators.</param>
        /// <param name="nextToken">Next token in the expression.</param>
        /// <returns>
        /// <c>true</c> if operator stack should be unwinded,
        /// otherwise <c>false</c>.
        /// </returns>
        private static bool UnwindOperators(Stack<Operator> ops, Operator nextToken)
        {
            if (ops.Count == 0) { return false; }

            return ops.Peek().Precedence <= nextToken.Precedence;
        }

        private enum TokenType { space, number, symbolOperator, stringOperator }

        /// <summary>
        /// Converts an infix expression string to an array of tokens.
        /// </summary>
        /// <param name="expression">The infix expression.</param>
        /// <returns>An array of tokens.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static object[] Tokenize(string expression)
        {
            Operator lpar = operators["("];
            Operator rpar = operators[")"];
            Operator mul = operators["*"];

            List<object> infix = new List<object>();
            TokenType lastTokenType = TokenType.space;
            string tmpToken = "";
            expression += " ";      // To add the last token

            foreach (char c in expression)
            {
                TokenType currentTokenType;
                if (c == ' ') { currentTokenType = TokenType.space; }
                else if ((c >= '0' && c <= '9') || c == '.') { currentTokenType = TokenType.number; }
                else if (c >= 'a' && c <= 'z') { currentTokenType = TokenType.stringOperator; }
                else { currentTokenType = TokenType.symbolOperator; }

                if (currentTokenType != lastTokenType || (currentTokenType == TokenType.symbolOperator && tmpToken.Length > 0))
                {
                    switch (lastTokenType)
                    {
                        case TokenType.space:
                            break;

                        case TokenType.number:
                            try
                            {
                                infix.Add(new BigNumber(tmpToken));
                            }
                            catch (FormatException)
                            {
                                throw new FormatException("Bad number format: " + tmpToken);
                            }
                            break;

                        case TokenType.symbolOperator:
                        case TokenType.stringOperator:
                            try
                            {
                                if (tmpToken == "+")
                                {
                                    object prevToken =
                                        infix.Count > 0 ? infix[^1] : null;

                                    if (!(prevToken is BigNumber || prevToken == rpar) || prevToken == null)
                                    {
                                        tmpToken = "pos";
                                    }
                                }
                                else if (tmpToken == "-")
                                {
                                    object prevToken =
                                        infix.Count > 0 ? infix[^1] : null;

                                    if (!(prevToken is BigNumber || prevToken == rpar) || prevToken == null)
                                    {
                                        tmpToken = "neg";
                                    }
                                }

                                infix.Add(operators[tmpToken]);
                            }
                            catch (KeyNotFoundException)
                            {
                                throw new ArgumentException("Invalid operator: " + tmpToken);
                            }
                            break;

                        default:
                            // Should not get here at all, but just in case...
                            throw new Exception("Invalid lastToken value.");
                    }
                    tmpToken = "";
                }

                tmpToken += c;
                lastTokenType = currentTokenType;
            }

            // Insert * operators
            for (int i = 1; i < infix.Count - 1; i++)
            {
                Operator token = infix[i] as Operator;
                if (token == rpar)
                {
                    object outerToken = infix[i + 1];
                    if (outerToken is BigNumber)
                    {
                        infix.Insert(i + 1, mul);
                        i++;
                    }
                }
                else if (token == lpar)
                {
                    object outerToken = infix[i - 1];
                    if (outerToken is BigNumber || outerToken == rpar)
                    {
                        infix.Insert(i, mul);
                        i++;
                    }
                }
                else if (token is Operator && token.Unary)
                {
                    object prevToken = infix[i - 1];
                    if (prevToken is BigNumber || (prevToken as Operator) == rpar)
                    {
                        infix.Insert(i, mul);
                        i++;
                    }
                }
            }

            return infix.ToArray();
        }

        /// <summary>
        /// Calculates a math expression asynchronously.
        /// </summary>
        /// <param name="expression">The math expression. (infix notation)</param>
        /// <returns>
        /// A <c>Task&lt;BigNumber&gt;</c> which contains the numeric value of expression.
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<BigNumber> CalculateAsync(string expression)
        {
            try
            {
                return await Task.Run(() => EvaluateRPN(InfixToRPN(Tokenize(expression)))); ;
            }
            catch (Exception ex)
            {
                if (ex is ArithmeticException || ex is DivideByZeroException ||
                    ex is ArgumentException || ex is FormatException)
                {
                    throw new ArgumentException(ex.Message);
                }

                throw new ArgumentException("Invalid expression.");
            }
        }
    }
}