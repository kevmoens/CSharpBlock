using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToBlockly
{
    public interface ISharpSyntax
    {
        void ParseNode(ParsePersistenceLocation location, bool isSetCreateBlock = false);
    }
}
