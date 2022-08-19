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
using Microsoft.CodeAnalysis.CSharp;

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
                _logger.LogTrace("Parse {Node.Kind}", node.Kind());
                var literalNode = node as ExpressionStatementSyntax;
                switch (literalNode.Expression.Kind().ToString())
                {
                    case "SimpleAssignmentExpression":
                        var simpleExpression = literalNode.Expression as AssignmentExpressionSyntax;

                        break;
                }
                foreach (var child in literalNode.ChildNodes())
                {
                    switch (child.GetType().Name)
                    {
                        case "AssignmentExpressionSyntax":
                            var assign = child as AssignmentExpressionSyntax;
                            var left = assign.Left as ExpressionSyntax;
                            var blockXml = new XElement("block", new XAttribute("type", "variables_set"));

                            var sharpExpressionSyntax = _serviceProvider.GetRequiredService<ISharpExpressionSyntax>();
                            sharpExpressionSyntax.ParseNode(ref blockXml, ref LastNode, left, false);
                            
                            var valueXml = new XElement("value", new XAttribute("name", "VALUE"));
                            var right = assign.Right as ExpressionSyntax;
                            blockXml.Add(valueXml);
                            
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
