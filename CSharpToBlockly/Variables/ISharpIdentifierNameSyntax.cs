using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    internal interface ISharpIdentifierNameSyntax
    {
        void ParseNode(ParsePersistenceLocation location, bool isSet);
    }
}