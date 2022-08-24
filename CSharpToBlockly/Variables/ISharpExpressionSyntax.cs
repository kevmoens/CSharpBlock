using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    internal interface ISharpExpressionSyntax
    {
        void ParseNode(ParsePersistenceLocation location, bool createBlock);
    }
}