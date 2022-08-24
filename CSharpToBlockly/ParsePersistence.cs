using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpToBlockly
{
    public class ParsePersistence
    {
        public XElement Document { get; set; } = new XElement("xml", "");
        public SyntaxNode? TopNode { get; set; }
        public Dictionary<ParsePersistenceLocation, ParsePersistenceDetail> Nodes { get; set; } = new ();
        public HashSet<string> Variables { get; set; } = new HashSet<string>();
        public void Reset()
        {
            Nodes = new Dictionary<ParsePersistenceLocation, ParsePersistenceDetail> ();
            Variables = new HashSet<string> ();
        }
    }
    
    public class ParsePersistenceDetail
    {
        public XElement Doc { get; set; }
        public XElement? LastNode { get; set; }
        public SyntaxNode Node { get; set; }
    }
}
