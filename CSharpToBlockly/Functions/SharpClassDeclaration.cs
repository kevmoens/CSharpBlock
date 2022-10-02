using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpToBlockly.Functions
{
    internal class SharpClassDeclaration : ISharpSyntax
    {

        ILogger<SharpClassDeclaration> _logger;
        IServiceProvider _serviceProvider;
        ParsePersistence _parsePersistence;
        public SharpClassDeclaration(ILogger<SharpClassDeclaration> logger, IServiceProvider serviceProvider, ParsePersistence parsePersistence)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parsePersistence = parsePersistence;
        }
        public void ParseNode(ParsePersistenceLocation location, bool unusedvar = false)
        {            
            var detail = _parsePersistence.Nodes[location];

            _logger.LogTrace("Parse {Node.Kind}", detail?.Node?.Kind());
            if (!(detail?.Node is ClassDeclarationSyntax))
            {
                return;
            }

            var classNode = detail.Node as ClassDeclarationSyntax;
            if (classNode == null)
            {
                return;
            }
            //var procXML = new XElement("block", new XAttribute("type", "procedures_defreturn")
            //    , new XElement("field", new XAttribute("name", "NAME"), classNode.Identifier.ValueText)
            //    );
            //detail.Doc.Add(procXML);
            //if (classNode.TypeParameterList?.Parameters != null)
            //{
            //    var mutationParms = new XElement("mutation", "");
            //    for(int i = 0; i < classNode.TypeParameterList?.Parameters.Count; i++)
            //    {
            //        var varName = classNode.TypeParameterList?.Parameters[i].Identifier.ValueText;
            //        if (varName != null)
            //        {
            //            mutationParms.Add(new XElement("arg", new XAttribute("name", varName),""));                    
            //        }
            //    }
            //}

            int childIdx = 0;
            foreach (var child in detail.Node.ChildNodes())
            {
                var sharpParse = _serviceProvider.GetRequiredService<SharpParse>();
                var childLocation = location.CreateChildNode($"{childIdx}");
                _parsePersistence.Nodes.TryAdd(childLocation, new ParsePersistenceDetail() { Doc = detail.Doc, LastNode = detail.LastNode, Node = child });
                sharpParse.ParseNode(childLocation);
                childIdx++;
            }
        }
    }
}
