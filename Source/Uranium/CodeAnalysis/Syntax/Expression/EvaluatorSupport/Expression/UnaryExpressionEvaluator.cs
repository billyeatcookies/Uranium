﻿using System;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression
{
    internal static class UnaryExpressionEvaluator
    {
        public static object EvaluateBoundUnaryExpression(BoundUnaryExpression u, Evaluator eval)
        {
            var operand = ExpressionEvaluator.EvaluateExpression(u.Operand, eval);
            switch (u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
            }
            Console.Error.WriteLine($"Unexpected unary operator {u.Kind}");
            return new object();
        }


    }
}