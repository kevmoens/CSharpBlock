using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    public class SharpVariableDeclaratorSyntax : ISharpVariableDeclaratorSyntax
    {
        ILogger<SharpVariableDeclaratorSyntax> _logger;
        IServiceProvider _serviceProvider;
        ParsePersistence _parsePersistence;
        public SharpVariableDeclaratorSyntax(ILogger<SharpVariableDeclaratorSyntax> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }

        public void ParseNode(ParsePersistenceLocation location)
        {

            var detail = _parsePersistence.Nodes[location];
            if (!(detail.Node is VariableDeclaratorSyntax))
            {
                return;
            }
            var variable = detail.Node as VariableDeclaratorSyntax;
            if (variable == null)
            {
                return;
            }


            var initializer = variable.Initializer;
            if (initializer == null)
            {
                return;
            }
            var fieldXml = new XElement("field", new XAttribute("name", "VAR"), variable.Identifier.ValueText);
            detail.Doc.Add(fieldXml);
            var valueXml = new XElement("value", new XAttribute("name", "VALUE"));
            detail.Doc.Add(valueXml);


            var sharpVariableInitializer = _serviceProvider.GetRequiredService<ISharpVariableInitializer>();
            var initLocation = location.CreateChildNode("0");
            _parsePersistence.Nodes.TryAdd(initLocation, new ParsePersistenceDetail() { Doc = valueXml, LastNode = detail.LastNode, Node = initializer });
            sharpVariableInitializer.ParseNode(initLocation);


            var tokenType = (SyntaxToken?)_parsePersistence.Nodes.FirstOrDefault((n) =>
                {
                    if (!n.Key.HasParent(location))
                    {
                        return false;
                    }

                    SyntaxToken? tokenValue = (SyntaxToken?)n.Value.Token.Value;
                    if (tokenValue == null)
                    {
                        return false;
                    }
                    return tokenValue.Value.Kind().ToString().Contains("Literal");
                }).Value.Token.Value;

            if (tokenType?.Kind().ToString() == "StringLiteralToken")
            {
                _parsePersistence.Variables.Add(new ParsePersistenceVariable() { Name = variable.Identifier.ValueText, Type = "string" });
            }
            else
            {
                if (tokenType?.Kind().ToString() == "NumericLiteralToken")
                {
                    _parsePersistence.Variables.Add(new ParsePersistenceVariable() { Name = variable.Identifier.ValueText, Type = "numeric" });
                }
                else
                {
                    _parsePersistence.Variables.Add(new ParsePersistenceVariable() { Name = variable.Identifier.ValueText });
                }
            }

        }
    }
}
