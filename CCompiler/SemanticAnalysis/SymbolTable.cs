using System.Collections.Generic;
using System.Text;

namespace CCompiler.SemanticAnalysis
{
    public class Table<T>
    {
        private Dictionary<string, T> _members;
        
        public Table()
        {
            _members = new Dictionary<string, T>();
        }

        public void Push(string id, T member) => _members.Add(id, member);
        
        public bool Exist(string id) => _members.ContainsKey(id);
        public T Get(string id) => _members[id];

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var member in _members)
            {
                result.Append(member.Value + "\n");
            }

            return "{\n" + result.ToString() + "}";
        }

        public Dictionary<string, T> GetData() => _members;
    }
}