using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    internal interface ISharpExpressionSyntax
    {
        void ParseNode(ref XElement doc, ref XElement LastNode, SyntaxNode node, bool createBlock);
    }
}