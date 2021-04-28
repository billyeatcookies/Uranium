﻿using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Syntax.Expression;

namespace Uranium.CodeAnalysis.Parsing.ParserSupport.Expression
{
    internal static class BinaryExpressionParser
    {
        //Allowing for proper operator precedence
        public static ExpressionSyntax Parse(Parser parser, int parentPrecedence = 0)
        {
            const int MIN_PRECEDENCE = 0;
            ExpressionSyntax left;  
            var unaryOperatorPrecedence = parser.Current.Kind.GetUnaryOperatorPrecedence();

            if(unaryOperatorPrecedence != MIN_PRECEDENCE && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = parser.NextToken();
                var operand = Parse(parser, unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = PrimaryExpressionParser.Parse(parser);
            }

            while(true)
            {
                var precedence = parser.Current.Kind.GetBinaryOperatorPrecedence();

                if(precedence <= MIN_PRECEDENCE || precedence <= parentPrecedence)
                {
                    break;
                }
                
                var operatorToken = parser.NextToken();
                var right = Parse(parser, precedence);
                
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }
            return left;
        }

    }
}
