using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    internal class SharpLocalDeclarationStatement : ISharpLocalDeclarationStatement
    {
        ILogger<SharpLocalDeclarationStatement> _logger;
        IServiceProvider _serviceProvider;
        public SharpLocalDeclarationStatement(ILogger<SharpLocalDeclarationStatement> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node)
        {

            if (!(node is LocalDeclarationStatementSyntax))
            {
                return;
            }

            var declareNode = node as LocalDeclarationStatementSyntax;
            if (declareNode == null)
            {
                return;
            }

            _logger.LogTrace("Parse {Node.Kind}", node.Kind());
            //TODO Handle Usings
            //declareNode.UsingKeyword

            //TODO Handle Await 
            //declareNode.WithAwaitKeyword

            var variablesXml = new XElement("variables", "");
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
                variablesXml.Add(new XElement("variable", variable.Identifier.ValueText));
                if (variable.Initializer != null)
                {
                    var initializer = variable.Initializer;
                    var fieldXml = new XElement("field", new XAttribute("name", "VAR"), variable.Identifier.ValueText);
                    blockXml.Add(fieldXml);
                    var valueXml = new XElement("value", new XAttribute("name", "VALUE"));
                    blockXml.Add(valueXml);

                    var sharpVariableInitializer = _serviceProvider.GetRequiredService<ISharpVariableInitializer>();
                    sharpVariableInitializer.ParseNode(ref valueXml, ref LastNode, initializer);
                }
            }
            doc.Add(variablesXml);

            doc.Add(blockXml);

            LastNode = blockXml;
        }
    }
}
