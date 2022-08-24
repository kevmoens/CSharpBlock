using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    internal interface ISharpVariableInitializer
    {
        void ParseNode(ParsePersistenceLocation location);
    }
}