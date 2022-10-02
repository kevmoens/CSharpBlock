using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Linq;
using Neleus.DependencyInjection.Extensions;

namespace CSharpToBlockly.Variables
{
    public class SharpEqualValueClauseSyntax : ISharpSyntax
    {
        ILogger<SharpEqualValueClauseSyntax> _logger;
        IServiceProvider _serviceProvider;
        ParsePersistence _parsePersistence;
        public SharpEqualValueClauseSyntax(ILogger<SharpEqualValueClauseSyntax> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }
        public void ParseNode(ParsePersistenceLocation location, bool unusedvar = false)
        {

            var detail = _parsePersistence.Nodes[location];
            if (detail == null) 
            {
                return;
            }
            if (detail.Node is EqualsValueClauseSyntax)
            {
                _logger.LogTrace("Parse {Node.Kind}", detail.Node.Kind());
                var initializer = detail.Node as EqualsValueClauseSyntax;
                if (initializer?.Parent == null)
                {
                    return;
                }
                var fieldXml = new XElement("field", new XAttribute("name", "VAR"), ((VariableDeclaratorSyntax)initializer.Parent).Identifier.ValueText);
                detail.Doc.Add(fieldXml);
                var valueXml = new XElement("value", new XAttribute("name", "VALUE"));
                detail.Doc.Add(valueXml);

                var sharpVariableInitializer = _serviceProvider.GetRequiredServiceByName<ISharpSyntax>("SharpVariableInitializer");
                var initLocation = location.CreateChildNode("0");
                _parsePersistence.Nodes.TryAdd(initLocation, new ParsePersistenceDetail() { Doc = valueXml, LastNode = detail.LastNode, Node = initializer });
                sharpVariableInitializer.ParseNode(initLocation);

            }
        }
    }
}
