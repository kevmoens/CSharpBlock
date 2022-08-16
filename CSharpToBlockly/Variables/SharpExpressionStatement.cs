using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpToBlockly.Variables
{
    public class SharpExpressionStatement : ISharpExpressionStatement
    {

        ILogger<SharpExpressionStatement> _logger;
        IServiceProvider _serviceProvider;
        public SharpExpressionStatement(ILogger<SharpExpressionStatement> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node)
        {
            if (node is ExpressionStatementSyntax)
            {
                var literalNode = node as ExpressionStatementSyntax;
                foreach (var child in literalNode.ChildNodes())
                {
                    switch (child.GetType().Name)
                    {
                        case "AssignmentExpressionSyntax":
                            var assign = child as AssignmentExpressionSyntax;
                            var left = assign.Left as IdentifierNameSyntax;
                            var blockXml = new XElement("block", new XAttribute("type", "variables_set"));
                            blockXml.Add(new XElement("field", new XAttribute("name", "VAR"), left.Identifier.Value));
                            var valueXml = new XElement("value", new XAttribute("name", "VALUE"));
                            var right = assign.Right as ExpressionSyntax;
                            blockXml.Add(valueXml);
                            var sharpExpressionSyntax = _serviceProvider.GetRequiredService<ISharpExpressionSyntax>();
                            sharpExpressionSyntax.ParseNode(ref valueXml, ref LastNode, right, true);

                            doc.Add(blockXml);
                            LastNode = valueXml;
                            break;
                    }
                }
            }

        }
    }
}
