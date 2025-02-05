﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Parsing.ParserSupport.Expression;

namespace Uranium.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        //Why forcefully override when I can avoid it 
        //This method works on all children of the class
        //This also allows for a central method, instead of 20 million implementations.
        //This should work fine because metadata
        public IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    if (property.GetValue(this) is SyntaxNode child)
                    {
                        yield return child;
                    }
                }
                else if (typeof(SeparatedSyntaxList).IsAssignableFrom(property.PropertyType))
                {
                    var list = property.GetValue(this) as SeparatedSyntaxList;
                    if(list is not null)
                    {
                        foreach(var item in list.GetWithSeparators())
                        {
                            if(item is not null)
                            {
                                yield return item;
                            }
                        }
                    }
                }

                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = property.GetValue(this) as IEnumerable<SyntaxNode>;
                    if(children is not null)
                    {
                        foreach(var child in children)
                        {
                            if(child is not null)
                            {
                                yield return child;
                            }
                        }
                    }
                }
            }
        }

        private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            //Variable to not bother changing the console colour if we're not printing to the console
            //That just makes no sense!
            var isToConsole = writer == Console.Out;
            var marker = isLast ? "└───" : "├───";

            
            if(isToConsole)
            {
                Console.ForegroundColor = ConsoleColor.Gray; 
            }

            writer.Write(indent);
            writer.Write(marker);
            
            if(isToConsole)
            {
                Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Green : ConsoleColor.Cyan;
            }

            writer.Write(node.Kind);

            if (node is SyntaxToken token && token.Value is not null)
            {
                writer.Write(" " + token.Value);
            }

            writer.WriteLine();

            if (isToConsole) Console.ResetColor();

            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();
            foreach (var child in node.GetChildren())
            {
                PrettyPrint(writer, child, indent, child == lastChild);
            }
        }

        public void WriteTo(TextWriter writer)
        {
            writer.WriteLine();
            PrettyPrint(writer, this);
        }

        public override string ToString()
        {
            using var writer = new StringWriter();

            WriteTo(writer);

            return writer.ToString();
        }
    }
}
