using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace CSharpToBlockly.Functions
{
    internal interface ISharpMethodDeclaration
    {
        void ParseNode(ParsePersistenceLocation location);
    }
}