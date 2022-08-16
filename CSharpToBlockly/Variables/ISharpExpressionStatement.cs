using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    public interface ISharpExpressionStatement
    {
        void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node);
    }
}