using CSharpToBlockly.Functions;
using CSharpToBlockly.Variables;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace CSharpToBlockly
{
    public class SharpParse
    {
        ILogger<SharpParse> _logger;
        IServiceProvider _serviceProvider;
        public SharpParse(ILogger<SharpParse> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        //Look at this site to compare block to SyntaxTree
        //https://blockly-demo.appspot.com/static/demos/code/index.html
        public XElement Parse(string code)
        {
            _logger.LogInformation("Parse START");
            XElement rootElement = new XElement("xml", "");
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.Parse, SourceCodeKind.Regular));
            SyntaxNode node = tree.GetRoot();
            XElement lastNode = new XElement("Empty", "");
            ParseNode(ref rootElement, ref lastNode, node);
            return rootElement;

        }
        internal void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node)
        {
            switch (node.Kind().ToString())
            {
                case "CompilationUnit":
                case "MethodDeclaration":                    
                    if (node.Kind().ToString() == "MethodDeclaration")
                    {
                        var sharpMethod = _serviceProvider.GetRequiredService<ISharpMethodDeclaration>();
                        sharpMethod.ParseNode(ref doc, ref LastNode, node);
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
                    //doc.Add(new XElement("MissingNode", node.GetType().Name));
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