using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CSharpToBlockly.Variables
{
    internal class SharpExpressionSyntax
    {

        internal static void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node)
        {
            if (node is LiteralExpressionSyntax)
            {
                var literalNode = node as LiteralExpressionSyntax;
                var literalXml = new XElement("block", "");

                switch (literalNode.Kind().ToString())
                {
                    case "NumericLiteralExpression":
                        literalXml.Add(new XAttribute("type", "math_number"));
                        literalXml.Add(new XElement("field", new XAttribute("name", "NUM"), literalNode.Token.ValueText));
                        break;

                    case "StringLiteralExpression":
                        literalXml.Add(new XAttribute("type", "text"));
                        literalXml.Add(new XElement("field", new XAttribute("name", "TEXT"), literalNode.Token.ValueText));
                        break;
                }
                doc.Add(literalXml);
            } else if (node is ParenthesizedExpressionSyntax)
            {
                var parenNode = node as ParenthesizedExpressionSyntax;
                //parenNode.Expression //Inner Expression

            } else if (node is BinaryExpressionSyntax)
            {
                var binaryNode = node as BinaryExpressionSyntax;
                var left = binaryNode.Left as ExpressionSyntax;
                switch (binaryNode.OperatorToken.Text)
                {
                    case "+":
                        break;
                }
                


            }


        }
    }
}
