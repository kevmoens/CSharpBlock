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

namespace CSharpToBlockly.Variables
{
    internal class SharpVariableInitializer : ISharpVariableInitializer
    {
        ILogger<SharpVariableInitializer> _logger;
        IServiceProvider _serviceProvider;
        public SharpVariableInitializer(ILogger<SharpVariableInitializer> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node)
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

            var sharpExpressionSyntax = _serviceProvider.GetRequiredService<ISharpExpressionSyntax>();
            sharpExpressionSyntax.ParseNode(ref doc, ref LastNode, valueNode, true);

        }
    }
}
