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
        public SemanticModel? SemanticModel { get; set; } 
        public HashSet<ParsePersistenceVariable> Variables { get; set; } = new HashSet<ParsePersistenceVariable>();
        public void Reset()
        {
            Nodes = new Dictionary<ParsePersistenceLocation, ParsePersistenceDetail> ();
            Variables = new HashSet<ParsePersistenceVariable> ();
        }
    }
    
    public class ParsePersistenceDetail
    {
        public XElement Doc { get; set; }
        public XElement? LastNode { get; set; }
        public SyntaxNode Node { get; set; }
        public SyntaxToken? Token { get; set; }
    }
    public class ParsePersistenceVariable
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
    }
}
