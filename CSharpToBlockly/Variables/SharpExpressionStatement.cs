﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    public class SharpExpressionStatement
    {

        internal static void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node)
        {
            if (node is ExpressionStatementSyntax)
            {
                var literalNode = node as ExpressionStatementSyntax;
                foreach (var child in literalNode.ChildNodes())
                {
                    switch (child.GetType().Name )
                    {
                        case "AssignmentExpressionSyntax":
                            var assign = child as AssignmentExpressionSyntax;
                            var left = assign.Left as IdentifierNameSyntax;
                            var blockXml = new XElement("block", new XAttribute("type", "variables_set"));
                            blockXml.Add(new XElement("field", new XAttribute("name", "VAR"), left.Identifier.Value));
                            var valueXml = new XElement("value", new XAttribute("name", "VALUE"));
                            var right = assign.Right as ExpressionSyntax;
                            SharpExpressionSyntax.ParseNode(ref valueXml, ref LastNode, right);

                            blockXml.Add(valueXml);
                            doc.Add(blockXml);                            
                            LastNode = valueXml;
                            break;
                    }
                }
            }

        }
    }
}
