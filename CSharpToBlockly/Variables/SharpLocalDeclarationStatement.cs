using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neleus.DependencyInjection.Extensions;
using System.Linq;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    internal class SharpLocalDeclarationStatement : ISharpSyntax
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

        public void ParseNode(ParsePersistenceLocation location, bool unusedvar = false)
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
            switch (declareNode.Declaration.Type.ToString())
            {
                case "int":
                    blockType = "math_number";
                    break;
                default:
                    blockType = declareNode.Declaration.Type.ToString();
                    break;
            }

            var blockXml = new XElement("block", new XAttribute("type", "variables_set"));
            foreach (var variable in declareNode.Declaration.Variables)
            {
                
                if (variable.Initializer != null)
                {
                    if (variable.Initializer is EqualsValueClauseSyntax)
                    {

                        var initLocation = location.CreateChildNode("0");
                        _parsePersistence.Nodes.Add(initLocation, new ParsePersistenceDetail() { Doc = blockXml, LastNode = detail.LastNode, Node = variable.Initializer });
                        var varDeclarator = _serviceProvider.GetRequiredServiceByName<ISharpSyntax>("SharpEqualValueClauseSyntax");
                        varDeclarator.ParseNode(initLocation);
                    } else
                    {
                        var initLocation = location.CreateChildNode("0");
                        _parsePersistence.Nodes.Add(initLocation, new ParsePersistenceDetail() { Doc = blockXml, LastNode = detail.LastNode, Node = variable.Initializer });
                        var varDeclarator = _serviceProvider.GetRequiredServiceByName<ISharpSyntax>("SharpVariableDeclaratorSyntax");
                        varDeclarator.ParseNode(initLocation);
                    }

                } else
                {
                    if (blockType == "math_number")
                    {
                        _parsePersistence.Variables.Add(new ParsePersistenceVariable() { Name = variable.Identifier.ValueText, Type = "numeric" });
                    } else
                    {
                        if (blockType == "text")
                        {
                            _parsePersistence.Variables.Add(new ParsePersistenceVariable() { Name = variable.Identifier.ValueText, Type = "string" });
                        } else
                        {
                            _parsePersistence.Variables.Add(new ParsePersistenceVariable() { Name = variable.Identifier.ValueText});
                        }
                    }
                }
            }
            
            detail.Doc.Add(blockXml);

            detail.LastNode = blockXml;
        }
    }
}
