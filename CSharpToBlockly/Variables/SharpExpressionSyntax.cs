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
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpToBlockly.Variables
{
    internal class SharpExpressionSyntax : ISharpExpressionSyntax
    {

        ILogger<SharpExpressionSyntax> _logger;
        IServiceProvider _serviceProvider;
        ParsePersistence _parsePersistence;
        public SharpExpressionSyntax(ILogger<SharpExpressionSyntax> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }

        public void ParseNode(ParsePersistenceLocation location, bool createBlock)
        {
            var detail = _parsePersistence.Nodes[location];
            _logger.LogTrace("Parse {Node.Kind}", detail.Node.Kind());
            var blockXml = detail.Doc.Parent;

            if (detail.Node is LiteralExpressionSyntax)
            {
                var literalNode = detail.Node as LiteralExpressionSyntax;
                XElement literalXml =  createBlock ? new XElement("block", "") : detail.Doc;

                switch (literalNode.Kind().ToString())
                {
                    case "NumericLiteralExpression":
                        if (createBlock)
                        {
                            literalXml.Add(new XAttribute("type", "math_number"));
                        }
                        literalXml.Add(new XElement("field", new XAttribute("name", "NUM"), literalNode.Token.ValueText));
                        var numericLocation = location.CreateToken("0");
                        _parsePersistence.Nodes.TryAdd(numericLocation, new ParsePersistenceDetail() { Doc = detail.Doc, LastNode = literalXml, Node = literalNode, Token = literalNode.Token });
                        break;

                    case "StringLiteralExpression":
                        if (createBlock)
                        {
                            literalXml.Add(new XAttribute("type", "text"));
                        }
                        literalXml.Add(new XElement("field", new XAttribute("name", "TEXT"), literalNode.Token.ValueText));

                        var stringLocation = location.CreateToken("0");
                        _parsePersistence.Nodes.TryAdd(stringLocation, new ParsePersistenceDetail() { Doc = detail.Doc, LastNode = literalXml, Node = literalNode, Token = literalNode.Token });
                        break;
                }
                if (createBlock)
                {
                    detail.Doc.Add(literalXml);
                }
            }

            //
            // n = 1; -- This is for the n and is the AssignmentExpressionSyntax.Left
            //      //doc = <block type="variables_set"/>
            //      //Should Add 
            //          < field name = ""VAR"" > n </ field >
            //          < value name = ""VALUE"" >
            //            < block type = ""math_number"" >
            //              < field name = ""NUM"" > 1 </ field >
            //            </ block >
            //          </ value >
            //
            else if (detail.Node is IdentifierNameSyntax)
            {
                var sharpIdentifierNameSyntax = _serviceProvider.GetRequiredService<ISharpIdentifierNameSyntax>();
                bool isSet = true;
                if (detail.Node.Parent is BinaryExpressionSyntax)
                {
                    isSet = false;
                }
                sharpIdentifierNameSyntax.ParseNode(location, isSet);
            }
            else if (detail.Node is ParenthesizedExpressionSyntax)
            {
                var parenNode = detail.Node as ParenthesizedExpressionSyntax;
                //parenNode.Expression //Inner Expression

            }
            else if (detail.Node is BinaryExpressionSyntax)
            {
                var binaryNode = detail.Node as BinaryExpressionSyntax;
                var left = binaryNode.Left as ExpressionSyntax;
                var right = binaryNode.Right as ExpressionSyntax;
                switch (binaryNode.OperatorToken.Text)
                {
                    case "+":
                        var literals = right.DescendantTokens().Where(t => t.Kind().ToString().Contains("Literal")).ToArray();
                        if (literals.Any() && literals[0].Kind().ToString() == "StringLiteralToken")
                        {
                            detail.LastNode = ParseNodeAppendText(location, detail.Doc, blockXml, left, right);
                        }
                        else
                        {
                            detail.LastNode = ParseNodeAdd(location, detail.Doc, blockXml, left, right);
                        }
                        break;
                    case "%":
                        detail.LastNode = ParseNodeModulo(location, detail.Doc, blockXml, left, right);
                        break;
                }

            }


        }

        private XElement ParseNodeAdd(ParsePersistenceLocation location, XElement LastNode, XElement? parentBlockXml, ExpressionSyntax left, ExpressionSyntax right)
        {
            var blockXml = new XElement("block", new XAttribute("type", "math_arithmetic")
                , new XElement("field", new XAttribute("name", "OP"), "ADD")
                );
            LastNode.Add(blockXml);
            var valueAXml = new XElement("value",
                                new XAttribute("name", "A")
                           );
            blockXml.Add(valueAXml);
            var sharpExpressionSyntax = _serviceProvider.GetRequiredService<ISharpExpressionSyntax>();
            var leftLocation = location.CreateChildNode("0");
            _parsePersistence.Nodes.TryAdd(leftLocation, new ParsePersistenceDetail() { Doc = valueAXml, LastNode = parentBlockXml, Node = left });
            sharpExpressionSyntax.ParseNode(leftLocation, true);


            var valueBXml = new XElement("value",
                                new XAttribute("name", "B")
                           );
            blockXml.Add(valueBXml);
            var rightLocation = location.CreateChildNode("1");
            _parsePersistence.Nodes.TryAdd(rightLocation, new ParsePersistenceDetail() { Doc = valueBXml, LastNode = parentBlockXml, Node = right});
            sharpExpressionSyntax.ParseNode(rightLocation, true);

            return LastNode;
        }

        private XElement ParseNodeAppendText(ParsePersistenceLocation location, XElement LastNode, XElement? parentBlockXml, ExpressionSyntax left, ExpressionSyntax right)
        {
            LastNode.Attribute("name").Value = "TEXT";
            parentBlockXml.Attribute("type").Value = "text_append";

            var sharpExpressionSyntax = _serviceProvider.GetRequiredService<ISharpExpressionSyntax>();
            var rightLocation = location.CreateChildNode("0");
            _parsePersistence.Nodes.TryAdd(rightLocation, new ParsePersistenceDetail() { Doc = LastNode, LastNode = parentBlockXml, Node = right });
            sharpExpressionSyntax.ParseNode(rightLocation, true);

            return LastNode;
        }

        private XElement ParseNodeModulo(ParsePersistenceLocation location, XElement LastNode, XElement? parentBlockXml, ExpressionSyntax left, ExpressionSyntax right)
        {
            var blockXml = new XElement("block", new XAttribute("type", "math_modulo"));
            LastNode.Add(blockXml);
            var valueDividendShadowXml = new XElement("shadow", new XAttribute("type", "math_number"));
            var valueDividendXml = new XElement("value",
                                new XAttribute("name", "DIVIDEND")                                
                                , valueDividendShadowXml
                           );
            blockXml.Add(valueDividendXml);

            var valueDivisorShadowXml = new XElement("shadow",
                                        new XAttribute("type", "math_number")
                                        , new XElement("field", new XAttribute("name", "NUM"), 64)
                                    );
            var valueDivisorXml = new XElement("value",
                                    new XAttribute("name", "DIVISOR")
                                    , valueDivisorShadowXml
                               );
            var sharpExpressionSyntax = _serviceProvider.GetRequiredService<ISharpExpressionSyntax>();
            
            if (left is LiteralExpressionSyntax)
            {
                var leftLocation = location.CreateChildNode("0");
                _parsePersistence.Nodes.TryAdd(leftLocation, new ParsePersistenceDetail() { Doc = valueDividendShadowXml, LastNode = parentBlockXml, Node = left });
                sharpExpressionSyntax.ParseNode(leftLocation, false);

            }
            else
            {
                var leftLocation = location.CreateChildNode("0");
                _parsePersistence.Nodes.TryAdd(leftLocation, new ParsePersistenceDetail() { Doc = valueDividendXml, LastNode = parentBlockXml, Node = left });
                sharpExpressionSyntax.ParseNode(leftLocation, false);
            }


            if (right is LiteralExpressionSyntax)
            {
                var leftLocation = location.CreateChildNode("1");
                _parsePersistence.Nodes.TryAdd(leftLocation, new ParsePersistenceDetail() { Doc = valueDivisorShadowXml, LastNode = parentBlockXml, Node = right });
                sharpExpressionSyntax.ParseNode(leftLocation, false);
            } else
            {
                var leftLocation = location.CreateChildNode("1");
                _parsePersistence.Nodes.TryAdd(leftLocation, new ParsePersistenceDetail() { Doc = valueDivisorXml, LastNode = parentBlockXml, Node = right });
                sharpExpressionSyntax.ParseNode(leftLocation, false);
            }

            blockXml.Add(valueDivisorXml);

            return LastNode;
        }
    }
}
