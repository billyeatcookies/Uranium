﻿using System;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Binding.NodeKinds;

namespace Compiler.CodeAnalysis.Syntax.Expression
{
    //Basically just the teacher that only gives
    //pop quizzes and tests
    //It just evaluates
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;
        public Evaluator(BoundExpression root)
        {
            _root = root;
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            //if it's a literal expression, return it's value
            if (node is BoundLiteralExpression n) 
            {
                return n.Value;
            }
            //if it's a Unary expression, we just evaluate the operand
            //and return it's value according to the symbol
            else if(node is BoundUnaryExpression u)
            {
                var operand = EvaluateExpression(u.Operand);
                switch(u.Op.Kind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        return (int)operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -(int)operand;
                    case BoundUnaryOperatorKind.LogicalNegation:
                        return !(bool)operand;
                }

                Console.Error.WriteLine($"Unexpected unary operator {u.Kind}");
            }
            //If it's none of the above, we check out last resort
            //A BoundBinaryExpression, here we evaluate the left and right sides of both expressions
            //then return a value based off of the current operator kind
            else if (node is BoundBinaryExpression b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.Op.Kind)
                {
                    //Universal
                    case BoundBinaryOperatorKind.LogicalEquals:
                        return Equals(left, right); 
                    case BoundBinaryOperatorKind.NotEquals:
                        return !Equals(left, right);

                    //Int
                    case BoundBinaryOperatorKind.Addition:
                        return (int)left + (int)right;
                    case BoundBinaryOperatorKind.Subtraction:
                        return (int)left - (int)right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return (int)left * (int)right;
                    case BoundBinaryOperatorKind.Division:
                        return (int)left / (int)right;
                    case BoundBinaryOperatorKind.Pow:
                        var result = (int)left;
                        var leftAsInt = (int)left;
                        for(var i = 1; i < (int)right; i++)
                        {
                            result *= leftAsInt;
                        }
                        return result;
                                            
                    //Bool
                    case BoundBinaryOperatorKind.LogicalAND:
                        return (bool)left && (bool)right;
                    case BoundBinaryOperatorKind.LogicalOR:
                        return (bool)left || (bool)right;
                    case BoundBinaryOperatorKind.LogicalXOREquals:
                        var leftBool = (bool)left;
                        var rightBool = (bool)right;
                        return leftBool ^= rightBool;
                    case BoundBinaryOperatorKind.LogicalXOR:
                        return (bool)left ^ (bool)right;

                    
                    default:
                        //We can throw exceptions here because we've exhausted all options,
                        //and this is an internal compiler error, should handle this more gracefully,
                        //but during the development stage, and exception will provide more info,
                        //on the stack trace :)
                        throw new($"Unexpected binary operator {b.Op.Kind}");
                }
            }
            //Same as above ^^ 
            throw new($"Unexpected node {node.Kind}");
        }
    }
}
