using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    internal class SharpIdentifierNameSyntax : ISharpIdentifierNameSyntax
    {

        ILogger<SharpIdentifierNameSyntax> _logger;
        IServiceProvider _serviceProvider;
        public SharpIdentifierNameSyntax(ILogger<SharpIdentifierNameSyntax> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node, bool isSet)
        {

            _logger.LogTrace("Parse {Node.RawKind}", node.RawKind);

            if (!(node is IdentifierNameSyntax))
            {
                return;
            }

            var identifierNode = node as IdentifierNameSyntax;


            if (isSet)
            {

                doc.Add(new XElement("field", new XAttribute("name", "VAR"), identifierNode.Identifier.ValueText));
                return;
            }

            doc.Add(new XElement("block", new XAttribute("type", "variables_get"), new XElement("field", new XAttribute("name", "VAR"), identifierNode.Identifier.ValueText)));

        }
    }
}
