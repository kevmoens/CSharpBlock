using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    internal class SharpVariableInitializer
    {

        internal static void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node)
        {

            if (!(node is EqualsValueClauseSyntax))
            {
                return;
            }

            var expressionNode = node as EqualsValueClauseSyntax;
            if (expressionNode == null)
            {
                return;
            }

            var valueNode = expressionNode.Value;

            SharpExpressionSyntax.ParseNode(ref doc, ref LastNode, valueNode, true);
            
        }
    }
}
