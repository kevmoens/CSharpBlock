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
        ParsePersistence _parsePersistence;
        public SharpExpressionStatement(ILogger<SharpExpressionStatement> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }
        public void ParseNode(ParsePersistenceLocation location)
        {
            var detail = _parsePersistence.Nodes[location];
            if (detail.Node is ExpressionStatementSyntax)
            {
                _logger.LogTrace("Parse {Node.Kind}", detail.Node.Kind());
                var literalNode = detail.Node as ExpressionStatementSyntax;
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
                            var leftLocation = location.CreateChildNode("0");
                            _parsePersistence.Nodes.TryAdd(leftLocation, new ParsePersistenceDetail() { Doc = blockXml, LastNode = detail.LastNode, Node = left });
                            sharpExpressionSyntax.ParseNode(leftLocation, false);
                            
                            var valueXml = new XElement("value", new XAttribute("name", "VALUE"));
                            var right = assign.Right as ExpressionSyntax;
                            blockXml.Add(valueXml);
                            
                            var rightLocation = location.CreateChildNode("1");
                            _parsePersistence.Nodes.TryAdd(rightLocation, new ParsePersistenceDetail() { Doc = valueXml, LastNode = detail.LastNode, Node = right});
                            sharpExpressionSyntax.ParseNode(rightLocation, true);

                            detail.Doc.Add(blockXml);
                            detail.LastNode = valueXml;
                            break;
                    }
                }
            }

        }
    }
}
