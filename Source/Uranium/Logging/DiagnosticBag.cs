﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Symbols;

namespace Uranium.Logging
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new();

        private readonly string BadTokenString = SyntaxKind.BadToken.ToString("G");

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.AsReadOnly().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Report(TextSpan span,string message)
        {
            var diag = new Diagnostic(span, message);
            _diagnostics.Add(diag);
        }

        public void Concat(DiagnosticBag other) => AddRange(other);

        public void AddRange(DiagnosticBag other) => _diagnostics.AddRange(other._diagnostics);

        /*public void ReportNumberStartWithUnderscore(TextSpan span, string text, Type type)
        {
            var message = $"UR00001: {text} : {type} cannot start with an underscore!";
            Report(span, message);
        }*/

        public void ReportInvalidNumber(TextSpan span, string text, Type type)
        {
            var message = $"UR00002: at {type} {text} : Could not parse {text} to {type.ToString()[7..]}.";
            Report(span, message);
        }

        public void ReportInvalidToken(TextSpan span, SyntaxToken actualKind, SyntaxKind expectedKind)
        {
            var expectedText = expectedKind == SyntaxKind.EndOfFile ? "EndOfFileToken" : $"{TextChecker.GetText(expectedKind)}";
            if(expectedText.Equals(BadTokenString, StringComparison.OrdinalIgnoreCase))
            {
                expectedText = "IdentifierToken";
            }

            var message = $"UR00003: Unexpected token: `{actualKind.Text}`. Expected: `{expectedText}`.";
            Report(span, message);
        }

        public void ReportInvalidToken(TextSpan span, SyntaxToken actualKind, params SyntaxKind[] expectedKinds)
        {
            var textBuilder = new StringBuilder();
            string expectedText;
            for(int i = 0; i < expectedKinds.Length; i++)
            {
                var currentTokenText = TextChecker.GetText(expectedKinds[i]);
                if(currentTokenText.Equals(BadTokenString, StringComparison.OrdinalIgnoreCase))
                {
                    currentTokenText = "IdentifierToken";
                }

                if(i == 0)
                {
                    textBuilder.Append($"{currentTokenText}");
                }
                else
                {
                    textBuilder.Append($", {currentTokenText}");
                }
            }

            expectedText = textBuilder.ToString();

            var message = $"UR00003: Unexpected token: `{actualKind.Text}`. Expected one of the following: `{expectedText}`.";
            Report(span, message);
        }

        public void ReportUnfinishedMultiLineComment(TextSpan span, int position)
        {
            var message = $"UR00004: Unfinished comment at index: {position}.";
            Report(span, message);
        }


        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType)
        {
            var message = $"UR00005: Unary operator {operatorText} is not defined for {operandType}.";
            Report(span, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan span, string? operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            var message = $"UR00006: Binary operator {operatorText} is undefined for {leftType} and {rightType}.";
            Report(span, message);
        }

        public void ReportUndefinedName(TextSpan span, string name)
        {
            var message = $"UR00007: The type or namespace {name} does not exist.";
            Report(span, message);
        }

        public void ReportVariableAlreadyDeclared(TextSpan span, string name)
        {
            var message = $"UR00008: The variable {name} already exists in the current scope!";
            Report(span, message);
        }

        public void ReportCannotConvert(TextSpan span, TypeSymbol converterType, TypeSymbol converteetype)
        {
            var message = $"UR00009: Cannot convert from type '{converterType}' to type '{converteetype}'.";
            Report(span, message);
        }

        public void ReportCannotAssign(TextSpan identifier, TextSpan equals, string name)
        {
            var totalSpan = new TextSpan(identifier.Start, identifier.Length + equals.Length + 1);
            var message = $"UR00010: Cannot assign a value to {name} because it is marked as let or const, meaning that it is read only.";
            Report(totalSpan, message);
        }

        /*public void ReportNoSemiColon(TextSpan span)
        {
            var message = $"UR00011: Line cannot end without a semi colon";
            Report(span, message);
        }
        
        public void ReportCannotStartWithNumber(TextSpan span, string text)
        {
            var message = $"UR00012: A file cannot start with a number. {text}";
            Report(span, message);
        }*/

        public void ReportInvalidCompoundOperator(TextSpan span, SyntaxToken token)
        {
            var message = $"UR00013: Cannot have a compound operator ({token.Text}) without a variable. Error at index: {token.Span.Start} through {token.Span.End}";
            Report(span, message);
        }


        public void ReportInvalidEqualsToken(TextSpan span)
        {
            var message = $"UR00014: Cannot have an equals expression without an identifier token";
            Report(span, message);
        }
        public void ReportInvalidDecimal(TextSpan span, string text, SyntaxKind previousKeyword)
        {
            var message = $"UR00015: A variable of type {TextChecker.GetText(previousKeyword)} cannot have any decimals. {text}";
            Report(span, message);
        }

        public void ReportUnfinishedSingleLineString(string text, TextSpan span)
        {
            var message = $"UR00016: Cannot have a single line string go through multiple lines ${text}";
            Report(span, message);
        }
        
        public void ReportUnfinishedString(string text, TextSpan span)
        {
            var message = $"UR00017: Cannot have a string that has no matching \". ${text}";
            Report(span, message);
        }

        public void ReportInvalidChar(TextSpan span)
        {
            var message = "UR00018: Cannot have more than one character within a char";
            Report(span, message);
        }

        public void ReportInvalidConstKeyword(TextSpan span)
        {
            var message = $"UR00019: Cannot have a const keyword without a variable type keyword after";
            Report(span, message);
        }

        public void ReportUndefinedFunction(TextSpan span, string text)
        {
            var message = $"UR00020: The function {text} does not exist";
            Report(span, message);
        }

        public void ReportWrongArgumentCount(int providedCount, int requiredCount, string text, TextSpan span)
        {
            var message = $"UR00021: The function '{text}' was expecting {requiredCount} arguments, but was given {providedCount} arguments"; 
            Report(span, message);
        }

        public void ReportInvalidParameter(TextSpan span, string functionName, string paramName, TypeSymbol expected, TypeSymbol actual)
        {
            var message = $"UR00022: The argument '{paramName}' of the function '{functionName}' expected a value of type {expected} but got a value of type {actual}";
            Report(span, message);
        }

        public void ReportExpressionMustHaveValue(TextSpan span)
        {
            var message = $"UR00023: Expression must have a value";
            Report(span, message);
        }
       
        public void ReportImplicitNullAssignment(TextSpan span)
        {
            var message = $"UR00020: Cannot have an implicit typed variable be assigned to null";
            Report(span, message);
        }

    }
}
