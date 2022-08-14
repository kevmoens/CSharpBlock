using CSharpToBlockly.Functions;
using CSharpToBlockly.Variables;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml.Linq;

namespace CSharpToBlockly
{
    public class SharpParse
    {
        //Look at this site to compare block to SyntaxTree
        //https://blockly-demo.appspot.com/static/demos/code/index.html
        public static XElement Parse(string code)
        {
            XElement rootElement = new XElement("xml", "");
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            SyntaxNode node = tree.GetRoot();
            XElement lastNode = new XElement("Empty", "");
            ParseNode(ref rootElement, ref lastNode, node);
            return rootElement;

        }
        internal static void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node)
        {
            switch (node.Kind().ToString())
            {
                case "CompilationUnit":
                case "MethodDeclaration":                    
                    if (node.Kind().ToString() == "MethodDeclaration")
                    {
                        SharpMethodDeclaration.ParseNode(ref doc, ref LastNode, node);
                    }
                    //doc.Add(new XElement("MissingNode", node.GetType().Name));                    
                    foreach (var child in node.ChildNodes())
                    {
                        if (LastNode.Parent != null)
                        {
                            var nextNode = new XElement("next", "");
                            LastNode.Add(nextNode);
                            ParseNode(ref nextNode, ref LastNode, child);
                        }
                        else
                        {                            
                            ParseNode(ref doc, ref LastNode, child);
                        }
                    }
                    break;
                case "LocalDeclarationStatement":                    
                    SharpLocalDeclarationStatement.ParseNode(ref doc, ref LastNode, node);
                    break;
                case "ExpressionStatement":
                    SharpExpressionStatement.ParseNode(ref doc, ref LastNode, node);
                    break;
                case "GlobalStatement":
                default:
                    doc.Add(new XElement("MissingNode", node.GetType().Name));
                    var lastNode = new XElement("Empty", "");
                    foreach (var child in node.ChildNodes())
	                {
                        ParseNode(ref doc, ref lastNode, child);
                        if (lastNode.Name != "Empty")
                        {
                            LastNode = lastNode;
                        }
	                }       
                    break;
            }
        }
    }
}