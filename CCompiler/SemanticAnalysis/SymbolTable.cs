using System.Collections.Generic;
using System.Linq;
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
                foreach (var s in member.Value.ToString().Split('\n'))
                    result.Append($"\t{s}\n");

            return "{\n" + result.ToString() + "}";
        }

        public Dictionary<string, T> GetData() => _members;

        public override bool Equals(object? obj)
        {
            if (!(obj is Table<T> table)) return false;
            var currentMembers = _members.ToList();
            var otherMembers = table._members.ToList();
            if (currentMembers.Count != otherMembers.Count)
                return false;
            for (var i = 0; i < currentMembers.Count; i++)
                if (currentMembers[i].Value.Equals(otherMembers[i].Value) == false)
                    return false;

            return true;
        }
    }
}