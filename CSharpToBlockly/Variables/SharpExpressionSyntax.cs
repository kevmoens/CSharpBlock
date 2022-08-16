using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpToBlockly.Variables
{
    internal class SharpExpressionSyntax : ISharpExpressionSyntax
    {

        ILogger<SharpExpressionSyntax> _logger;
        IServiceProvider _serviceProvider;
        public SharpExpressionSyntax(ILogger<SharpExpressionSyntax> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node, bool createBlock)
        {
            var blockXml = doc.Parent;

            if (node is LiteralExpressionSyntax)
            {
                var literalNode = node as LiteralExpressionSyntax;
                XElement literalXml = new XElement("block", "");

                switch (literalNode.Kind().ToString())
                {
                    case "NumericLiteralExpression":
                        if (createBlock)
                        {
                            literalXml.Add(new XAttribute("type", "math_number"));
                        }
                        literalXml.Add(new XElement("field", new XAttribute("name", "NUM"), literalNode.Token.ValueText));
                        break;

                    case "StringLiteralExpression":
                        if (createBlock)
                        {
                            literalXml.Add(new XAttribute("type", "text"));
                        }
                        literalXml.Add(new XElement("field", new XAttribute("name", "TEXT"), literalNode.Token.ValueText));
                        break;
                }
                if (createBlock)
                {
                    doc.Add(literalXml);
                }
            }
            else if (node is IdentifierNameSyntax)
            {
                var identifierNode = node as IdentifierNameSyntax;
                doc.Add(new XElement("block", new XAttribute("type", "variables_get"), new XElement("field", new XAttribute("name", "VAR"), identifierNode.Identifier.ToFullString())));
            }
            else if (node is ParenthesizedExpressionSyntax)
            {
                var parenNode = node as ParenthesizedExpressionSyntax;
                //parenNode.Expression //Inner Expression

            }
            else if (node is BinaryExpressionSyntax)
            {
                var binaryNode = node as BinaryExpressionSyntax;
                var left = binaryNode.Left as ExpressionSyntax;
                var right = binaryNode.Right as ExpressionSyntax;
                switch (binaryNode.OperatorToken.Text)
                {
                    case "+":
                        LastNode = ParseNodeAdd(LastNode, blockXml, left, right);
                        break;
                }

            }


        }

        private XElement ParseNodeAdd(XElement LastNode, XElement? blockXml, ExpressionSyntax left, ExpressionSyntax right)
        {
            blockXml.Attribute("type").Value = "math_arithmetic";
            var fieldXml = new XElement("field",
                                new XAttribute("name", "OP")
                                , "ADD"
                           );
            blockXml.Add(fieldXml);

            var addValueAXml = new XElement("value",
                                    new XAttribute("name", "A")
                                    , new XElement("shadow"
                                        , new XAttribute("type", "math_number")
                                        , new XElement("field"
                                            , new XAttribute("name", "NUM")
                                            , "1"
                                        )
                                    )
                               );
            var sharpExpressionSyntax = _serviceProvider.GetRequiredService<ISharpExpressionSyntax>();
            sharpExpressionSyntax.ParseNode(ref addValueAXml, ref LastNode, left, false);
            blockXml.Add(addValueAXml);
            var addValueBXml = new XElement("value",
                                    new XAttribute("name", "B")
                                    , new XElement("shadow"
                                        , new XAttribute("type", "math_number")
                                        , new XElement("field"
                                            , new XAttribute("name", "NUM")
                                            , "1"
                                        )
                                    )
                               );
            sharpExpressionSyntax.ParseNode(ref addValueBXml, ref LastNode, right, false);
            blockXml.Add(addValueBXml);
            return LastNode;
        }
    }
}
