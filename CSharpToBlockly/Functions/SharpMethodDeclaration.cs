using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace CSharpToBlockly.Functions
{
    internal class SharpMethodDeclaration : ISharpMethodDeclaration
    {
        ILogger<SharpMethodDeclaration> _logger;
        IServiceProvider _serviceProvider;
        ParsePersistence _parsePersistence;
        public SharpMethodDeclaration(ILogger<SharpMethodDeclaration> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }
        public void ParseNode(ParsePersistenceLocation location)
        {
            var detail = _parsePersistence.Nodes[location];
            
            _logger.LogTrace("Parse {Node.Kind}", detail.Node.Kind());
            if (!(detail.Node is MethodDeclarationSyntax))
            {
                return;
            }

            var methodNode = detail.Node as MethodDeclarationSyntax;
            if (methodNode == null)
            {
                return;
            }
            string returnTypeVoid = string.Empty;
            if (methodNode.ReturnType is PredefinedTypeSyntax
                && ((PredefinedTypeSyntax)methodNode.ReturnType).Keyword.ValueText == "void"
                )
            {
                returnTypeVoid = "no";

            }

            var methodXml = new XElement("block",
                            new XAttribute("type", $"procedures_def{returnTypeVoid}return")
                            , "");

            if (methodNode.ParameterList != null)
            {
                var parms = new XElement("mutation", "");
                foreach (var parm in methodNode.ParameterList.Parameters)
                {
                    parms.Add(new XElement("arg", new XAttribute("name", parm.Identifier.ValueText)));
                }
                methodXml.Add(parms);
            }
            methodXml.Add(new XElement("field", new XAttribute("name", "NAME"), new XText(methodNode.Identifier.Text)));

            var methodBlockXml = new XElement("statement", new XAttribute("name", "STACK"));
            methodXml.Add(methodBlockXml);
            int childIdx = 0;
            foreach (var child in detail.Node.ChildNodes())
            {
                var sharpParse = _serviceProvider.GetRequiredService<SharpParse>();
                var childLocation = location.CreateChildNode($"{childIdx}");
                _parsePersistence.Nodes.TryAdd(childLocation, new ParsePersistenceDetail() { Doc = detail.Doc, LastNode = detail.LastNode, Node = child });
                sharpParse.ParseNode(childLocation);
                childIdx++;
            }

            detail.Doc.Add(methodXml);

            detail.LastNode = methodXml;

        }
    }
}
