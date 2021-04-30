﻿using System;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Types;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression
{
    internal static class BinaryExpressionEvaluator
    {
        public static object EvaluateBoundBinaryExpression(BoundBinaryExpression b, Evaluator eval)
        {
            var left = ExpressionEvaluator.EvaluateExpression(b.Left, eval);
            var right = ExpressionEvaluator.EvaluateExpression(b.Right, eval);
            return b.Op.Kind switch
            {
                //Universal
                BoundBinaryOperatorKind.LogicalEquals => EqualityEvaluator.LeftEqualsRight(left, right),
                BoundBinaryOperatorKind.NotEquals => !EqualityEvaluator.LeftEqualsRight(left, right),

                //Int
                BoundBinaryOperatorKind.Addition => (int)left + (int)right,
                BoundBinaryOperatorKind.Subtraction => (int)left - (int)right,
                BoundBinaryOperatorKind.Multiplication => (int)left * (int)right,
                BoundBinaryOperatorKind.Division => (int)left / (int)right,
                BoundBinaryOperatorKind.LesserThan => (int)left < (int)right,
                BoundBinaryOperatorKind.LesserThanEquals => (int)left <= (int)right,
                BoundBinaryOperatorKind.GreaterThan => (int)left > (int)right,
                BoundBinaryOperatorKind.GreaterThanEquals => (int)left >= (int)right,

                BoundBinaryOperatorKind.Pow => (int)Math.Pow((int)left, (int)right),

                //Bool
                BoundBinaryOperatorKind.LogicalAND => (bool)left && (bool)right,
                BoundBinaryOperatorKind.LogicalOR => (bool)left || (bool)right,
                BoundBinaryOperatorKind.LogicalXOR => (bool)left ^ (bool)right,

                //We can throw exceptions here because we've exhausted all options,
                //and this is an internal Uranium error, should handle this more gracefully,
                //but during the development stage, and exception will provide more info,
                //on the stack trace :)
                _ => throw new($"Unexpected binary operator {b.Op.Kind}"),
            };
        }

    }
}