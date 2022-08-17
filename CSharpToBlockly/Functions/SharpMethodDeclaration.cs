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
        public SharpMethodDeclaration(ILogger<SharpMethodDeclaration> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node)
        {

            _logger.LogTrace("Parse {Node.Kind}", node.Kind());
            if (!(node is MethodDeclarationSyntax))
            {
                return;
            }

            var methodNode = node as MethodDeclarationSyntax;
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

            foreach (var child in node.ChildNodes())
            {
                var sharpParse = _serviceProvider.GetRequiredService<SharpParse>();
                sharpParse.ParseNode(ref methodBlockXml, ref LastNode, child);
            }

            doc.Add(methodXml);

            LastNode = methodXml;

        }
    }
}
