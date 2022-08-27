namespace CSharpToBlockly.Variables
{
    public interface ISharpFieldDeclarationSyntax
    {
        void ParseNode(ParsePersistenceLocation location);
    }
}