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
        public SharpExpressionSyntax(ILogger<SharpExpressionSyntax> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node, bool createBlock)
        {
            _logger.LogTrace("Parse {Node.Kind}", node.Kind());
            var blockXml = doc.Parent;

            if (node is LiteralExpressionSyntax)
            {
                var literalNode = node as LiteralExpressionSyntax;
                XElement literalXml =  createBlock ? new XElement("block", "") : doc;

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
            else if (node is IdentifierNameSyntax)
            {
                var sharpIdentifierNameSyntax = _serviceProvider.GetRequiredService<ISharpIdentifierNameSyntax>();
                bool isSet = true;
                if (node.Parent is BinaryExpressionSyntax)
                {
                    isSet = false;
                }
                sharpIdentifierNameSyntax.ParseNode(ref doc, ref LastNode, node, isSet);
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
                        LastNode = ParseNodeAdd(doc, blockXml, left, right);
                        break;
                    case "%":
                        LastNode = ParseNodeModulo(doc, blockXml, left, right);
                        break;
                }

            }


        }

        private XElement ParseNodeAdd(XElement LastNode, XElement? parentBlockXml, ExpressionSyntax left, ExpressionSyntax right)
        {
            var blockXml = new XElement("block", new XAttribute("type", "math_arithmetic")
                , new XElement("field", new XAttribute("name", "OP"), "ADD")
                );
            LastNode.Add(blockXml);
            var valueAXml = new XElement("value",
                                new XAttribute("name", "A")
                           );
            var sharpExpressionSyntax = _serviceProvider.GetRequiredService<ISharpExpressionSyntax>();
            sharpExpressionSyntax.ParseNode(ref valueAXml, ref LastNode, left, true);
            blockXml.Add(valueAXml);


            var valueBXml = new XElement("value",
                                new XAttribute("name", "B")
                           );
            
            sharpExpressionSyntax.ParseNode(ref valueBXml, ref LastNode, right, true);
            blockXml.Add(valueBXml);

            return LastNode;
        }

        private XElement ParseNodeModulo(XElement LastNode, XElement? parentBlockXml, ExpressionSyntax left, ExpressionSyntax right)
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
                sharpExpressionSyntax.ParseNode(ref valueDividendShadowXml, ref LastNode, left, false);
            }
            else
            {
                sharpExpressionSyntax.ParseNode(ref valueDividendXml, ref LastNode, left, false);
            }


            if (right is LiteralExpressionSyntax)
            {
                sharpExpressionSyntax.ParseNode(ref valueDivisorShadowXml, ref LastNode, right, false);
            } else
            {
                sharpExpressionSyntax.ParseNode(ref valueDivisorXml, ref LastNode, right, false);
            }

            blockXml.Add(valueDivisorXml);

            return LastNode;
        }
    }
}
