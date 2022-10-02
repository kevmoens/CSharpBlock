using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neleus.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    public class SharpFieldDeclarationSyntax : ISharpSyntax
    {
        ILogger<SharpFieldDeclarationSyntax> _logger;
        IServiceProvider _serviceProvider;
        ParsePersistence _parsePersistence;
        public SharpFieldDeclarationSyntax(ILogger<SharpFieldDeclarationSyntax> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }

        public void ParseNode(ParsePersistenceLocation location, bool unusedvar = false)
        {

            var detail = _parsePersistence.Nodes[location];
            if (!(detail.Node is FieldDeclarationSyntax))
            {
                return;
            }
            var declareNode = detail.Node as FieldDeclarationSyntax;
            if (declareNode == null)
            {
                return;
            }

            var type = declareNode.Declaration.Type;


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
                if (variable.Initializer != null)
                {
                    var initLocation = location.CreateChildNode("0");
                    _parsePersistence.Nodes.Add(initLocation, new ParsePersistenceDetail() { Doc = blockXml,  LastNode=  detail.LastNode, Node = variable.Initializer });
                    var varDeclarator = _serviceProvider.GetRequiredServiceByName<ISharpSyntax>("SharpVariableDeclaratorSyntax");
                    varDeclarator.ParseNode(initLocation);
                }
                else
                {
                    if (blockType == "math_number")
                    {
                        _parsePersistence.Variables.Add(new ParsePersistenceVariable() { Name = variable.Identifier.ValueText, Type = "numeric" });
                    }
                    else
                    {
                        if (blockType == "text")
                        {
                            _parsePersistence.Variables.Add(new ParsePersistenceVariable() { Name = variable.Identifier.ValueText, Type = "string" });
                        }
                        else
                        {
                            _parsePersistence.Variables.Add(new ParsePersistenceVariable() { Name = variable.Identifier.ValueText });
                        }
                    }
                }
            }

            detail.Doc.Add(blockXml);

            detail.LastNode = blockXml;
        }
    }
}
