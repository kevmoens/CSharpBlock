using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
        ParsePersistence _parsePersistence;
        public SharpIdentifierNameSyntax(ILogger<SharpIdentifierNameSyntax> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }

        public void ParseNode(ParsePersistenceLocation location, bool isSet)
        {
            var detail = _parsePersistence.Nodes[location];
            _logger.LogTrace("Parse {Node.RawKind}", detail.Node.RawKind);

            if (!(detail.Node is IdentifierNameSyntax))
            {
                return;
            }

            var identifierNode = detail.Node as IdentifierNameSyntax;


            if (isSet)
            {

                detail.Doc.Add(new XElement("field", new XAttribute("name", "VAR"), identifierNode.Identifier.ValueText));
                return;
            }

            detail.Doc.Add(new XElement("block", new XAttribute("type", "variables_get"), new XElement("field", new XAttribute("name", "VAR"), identifierNode.Identifier.ValueText)));
            if (!_parsePersistence.Variables.Any((item) => item.Name == identifierNode.Identifier.ValueText))
            {
                //TODO Hold Type
                _parsePersistence.Variables.Add(new ParsePersistenceVariable() { Name = identifierNode.Identifier.ValueText });
            }
        }
    }
}
