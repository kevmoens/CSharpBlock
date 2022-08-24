using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CSharpToBlockly.Functions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpToBlockly.Variables
{
    internal class SharpVariableInitializer : ISharpVariableInitializer
    {
        ILogger<SharpVariableInitializer> _logger;
        IServiceProvider _serviceProvider;
        ParsePersistence _parsePersistence;
        public SharpVariableInitializer(ILogger<SharpVariableInitializer> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }

        public void ParseNode(ParsePersistenceLocation location)
        {
            var detail = _parsePersistence.Nodes[location];
            if (!(detail.Node is EqualsValueClauseSyntax))
            {
                return;
            }

            var expressionNode = detail.Node as EqualsValueClauseSyntax;
            if (expressionNode == null)
            {
                return;
            }
            _logger.LogTrace("Parse {Node.Kind}", detail.Node.Kind());

            var valueNode = expressionNode.Value;

            var sharpExpressionSyntax = _serviceProvider.GetRequiredService<ISharpExpressionSyntax>();
            var expressionLocation = location.CreateChildNode("0");
            _parsePersistence.Nodes.TryAdd(expressionLocation, new ParsePersistenceDetail() {  Doc = detail.Doc, LastNode = detail.LastNode, Node = valueNode});
            sharpExpressionSyntax.ParseNode(expressionLocation, true);

        }
    }
}
