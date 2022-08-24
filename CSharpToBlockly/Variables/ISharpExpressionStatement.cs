using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace CSharpToBlockly.Variables
{
    public interface ISharpExpressionStatement
    {
        void ParseNode(ParsePersistenceLocation location);
    }
}