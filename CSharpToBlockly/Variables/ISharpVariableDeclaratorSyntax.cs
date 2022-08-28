namespace CSharpToBlockly.Variables
{
    public interface ISharpVariableDeclaratorSyntax
    {
        void ParseNode(ParsePersistenceLocation location);
    }
}