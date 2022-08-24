using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    internal class SharpLocalDeclarationStatement : ISharpLocalDeclarationStatement
    {
        ILogger<SharpLocalDeclarationStatement> _logger;
        IServiceProvider _serviceProvider;
        ParsePersistence _parsePersistence;
        public SharpLocalDeclarationStatement(ILogger<SharpLocalDeclarationStatement> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }

        public void ParseNode(ParsePersistenceLocation location)
        {
            var detail = _parsePersistence.Nodes[location];
            if (!(detail.Node is LocalDeclarationStatementSyntax))
            {
                return;
            }

            var declareNode = detail.Node as LocalDeclarationStatementSyntax;
            if (declareNode == null)
            {
                return;
            }

            _logger.LogTrace("Parse {Node.Kind}", detail.Node.Kind());
            //TODO Handle Usings
            //declareNode.UsingKeyword

            //TODO Handle Await 
            //declareNode.WithAwaitKeyword
            
            
            string blockType = "text";
            if (declareNode.Declaration.Type.IsVar)
            {

            }
            else
            {
                switch (declareNode.Declaration.Type.ToString())
                {
                    case "int":
                        blockType = "math_number";
                        break;
                    default:
                        blockType = declareNode.Declaration.Type.ToString();
                        break;
                }
            }

            var blockXml = new XElement("block", new XAttribute("type", "variables_set"));
            foreach (var variable in declareNode.Declaration.Variables)
            {
                if (!_parsePersistence.Variables.Contains(variable.Identifier.ValueText))
                {
                    _parsePersistence.Variables.Add(variable.Identifier.ValueText);
                }
                
                if (variable.Initializer != null)
                {
                    var initializer = variable.Initializer;
                    var fieldXml = new XElement("field", new XAttribute("name", "VAR"), variable.Identifier.ValueText);
                    blockXml.Add(fieldXml);
                    var valueXml = new XElement("value", new XAttribute("name", "VALUE"));
                    blockXml.Add(valueXml);

                    var sharpVariableInitializer = _serviceProvider.GetRequiredService<ISharpVariableInitializer>();
                    var initLocation = location.CreateChildNode("0");
                    _parsePersistence.Nodes.TryAdd(initLocation, new ParsePersistenceDetail() { Doc = valueXml, LastNode = detail.LastNode, Node = initializer });
                    sharpVariableInitializer.ParseNode(initLocation);
                }
            }
            
            detail.Doc.Add(blockXml);

            detail.LastNode = blockXml;
        }
    }
}
