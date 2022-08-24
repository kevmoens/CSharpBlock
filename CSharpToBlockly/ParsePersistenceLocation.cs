using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToBlockly
{
    /// <summary>
    /// Used to describe location in tree
    /// 0
    ///     First Node in tree
    /// 0.0
    ///     First Node under Top Node
    /// 0.0.1
    ///     Second Node under top node's top child
    /// 0:0
    ///     First token under first top node
    ///     
    /// </summary>
    public record ParsePersistenceLocation
    {
        string _location;
        public ParsePersistenceLocation(string location)
        {
            _location = location;
        }

        public override string ToString()
        {
            return _location;
        }

        public ParsePersistenceLocation GetParent()
        {
            string current = TruncateToken();
            var items = current.Split(".");
            if (items.Length == 0)
            {
                return new ParsePersistenceLocation("0");
            }            
            return new ParsePersistenceLocation(string.Join(".", items.Take(items.Length - 1)));
        }

        public ParsePersistenceLocation CreateChildNode(string childSuffix)
        {
            string current = TruncateToken();
            return new ParsePersistenceLocation(current + $".{childSuffix}");
        }
        
        private string TruncateToken()
        {
            var current = _location;
            if (_location.IndexOf(":") > -1)
            {
                current = _location.Substring(0, _location.IndexOf(":"));
            }

            return current;
        }
    }
}
