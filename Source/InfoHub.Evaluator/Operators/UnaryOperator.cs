using System;
using System.Linq.Expressions;

namespace InfoHub.Evaluator.Operators
{
    internal class UnaryOperator : Operator<Func<Expression, UnaryExpression>>
    {
        public UnaryOperator(string value, int precedence, bool leftassoc, Func<Expression, UnaryExpression> func)
            : base(value, precedence, leftassoc, func)
        {
            Arguments = 1;
        }
    }
}