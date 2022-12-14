using CSharpToBlockly.Functions;
using CSharpToBlockly.Variables;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neleus.DependencyInjection.Extensions;
using System.Xml.Linq;

namespace CSharpToBlockly
{
    public class SharpParse
    {
        ILogger<SharpParse> _logger;
        IServiceProvider _serviceProvider;
        ParsePersistence _parsePersistence;
        public SharpParse(ILogger<SharpParse> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }
        public List<string> Usings { get; set; } = new List<string>();

        //Look at this site to compare block to SyntaxTree
        //https://blockly-demo.appspot.com/static/demos/code/index.html
        public XElement Parse(string code)
        {
            _logger.LogInformation("Parse START");
            XElement rootElement = new XElement("xml", "");
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.Parse, SourceCodeKind.Regular));
            var compilation = CSharpCompilation.Create("HelloWorld")
                .AddSyntaxTrees(tree);
            SemanticModel model = compilation.GetSemanticModel(tree);
            _parsePersistence.SemanticModel = model;
            var location = new ParsePersistenceLocation("0");            
            SyntaxNode node = tree.GetRoot();
            XElement lastNode = new XElement("Empty", "");
            _parsePersistence.Document = rootElement;
            _parsePersistence.TopNode = node;
            _parsePersistence.Reset();
            _parsePersistence.Nodes.TryAdd(location, new ParsePersistenceDetail() { Doc = rootElement, LastNode = lastNode, Node = node });
            ParseNode(location);
            var variablesXml = new XElement("variables", "");
            if (_parsePersistence.Variables.Count > 0)
            {
                foreach (var variable in _parsePersistence.Variables)
                {
                    variablesXml.Add(new XElement("variable", variable.Name));
                }
                rootElement.AddFirst(variablesXml);
            }
            return rootElement;

        }
        internal void ParseNode(ParsePersistenceLocation location)
        {
            var detail = _parsePersistence.Nodes[location];
            if (detail == null)
            {
                return;
            }
            var node = detail.Node;  
            if (node == null)
            { 
                return;
            }
            _logger.LogTrace("Parse {Node.Kind}", node?.Kind());
            switch (node?.Kind().ToString())
            {
                case "NamespaceDeclaration":
                case "CompilationUnit":
                case "MethodDeclaration":                    
                    if (node.Kind().ToString() == "MethodDeclaration")
                    {
                        var sharpMethod = _serviceProvider.GetRequiredServiceByName<ISharpSyntax>("SharpMethodDeclaration");
                        sharpMethod.ParseNode(location);
                    }
                    //doc.Add(new XElement("MissingNode", node.GetType().Name));
                    int childIdx = 0;
                    foreach (var child in node.ChildNodes())
                    {
                        if (detail.LastNode.Parent != null)
                        {
                            var nextNode = new XElement("next", "");
                            detail.LastNode.Add(nextNode);
                            var childLocation = location.CreateChildNode(childIdx.ToString());
                            _parsePersistence.Nodes.TryAdd(childLocation, new ParsePersistenceDetail() { Doc = nextNode, LastNode = detail.LastNode, Node = child });
                            ParseNode(childLocation);
                            detail.LastNode = _parsePersistence.Nodes[childLocation].LastNode;
                        }
                        else
                        {
                            var childLocation = location.CreateChildNode(childIdx.ToString());
                            _parsePersistence.Nodes.TryAdd(childLocation, new ParsePersistenceDetail() { Doc = detail.Doc, LastNode = detail.LastNode, Node = child });
                            ParseNode(childLocation);
                            detail.LastNode = _parsePersistence.Nodes[childLocation].LastNode;
                        }
                        childIdx++;
                    }
                    break;
                case "GlobalStatement":
                    var lastNode = new XElement("Empty", "");
                    int cIdx = 0;
                    foreach (var child in node.ChildNodes())
                    {
                        var childLocation = location.CreateChildNode(cIdx.ToString());
                        _parsePersistence.Nodes.TryAdd(childLocation, new ParsePersistenceDetail() { Doc = detail.Doc, LastNode = detail.LastNode, Node = child });
                        ParseNode(childLocation);                        
                        if ((_parsePersistence.Nodes[childLocation].LastNode?.Name ?? "Empty") != "Empty")
                        {
                            detail.LastNode = _parsePersistence.Nodes[childLocation].LastNode;
                        }
                        cIdx++;
                    }       
                    break;
                case "UsingDirective":
                    Usings.Add(node.ToFullString());
                    break;
                case "ClassDeclaration":
                case "FieldDeclaration":
                case "LocalDeclarationStatement":
                case "ExpressionStatement":
                    var sharpClass = _serviceProvider.GetRequiredServiceByName<ISharpSyntax>($"Sharp{node?.Kind().ToString()}");
                    sharpClass.ParseNode(location);
                    break;
                default:
                    System.Diagnostics.Debug.Print($"Warning Missing Kind: {node?.Kind()} location {location}");
                    break;
            }
        }
    }
}