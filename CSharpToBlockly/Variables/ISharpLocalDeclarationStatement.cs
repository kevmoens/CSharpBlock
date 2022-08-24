using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    internal interface ISharpLocalDeclarationStatement
    {
        void ParseNode(ParsePersistenceLocation location);
    }
}