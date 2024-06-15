using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FunctionComposeLibrary
{
    internal struct Operation
    {
        internal readonly Func<Expression, Expression, BinaryExpression>? ExpressionHandler;
        internal readonly int Priority;

        internal Operation(Func<Expression, Expression, BinaryExpression>? expressionHandler, int priority)
        {
            ExpressionHandler = expressionHandler;
            Priority = priority;
        }
    }

}
