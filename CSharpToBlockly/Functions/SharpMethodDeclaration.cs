using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml.Linq;

namespace CSharpToBlockly.Functions
{
    internal class SharpMethodDeclaration
    {
        internal static void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node)
        {
            
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
                SharpParse.ParseNode(ref methodBlockXml, ref LastNode, child);
            }

            doc.Add(methodXml);

            LastNode = methodXml;
        
        }
    }
}
