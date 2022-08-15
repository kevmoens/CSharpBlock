using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace CSharpToBlockly.Functions
{
    internal interface ISharpMethodDeclaration
    {
        void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node);
    }
}