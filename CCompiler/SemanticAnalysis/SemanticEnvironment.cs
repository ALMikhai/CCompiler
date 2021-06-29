using System.Collections.Generic;

namespace CCompiler.SemanticAnalysis
{
    public class SemanticEnvironment
    {
        private Stack<EnvironmentSnapshot> _snapshots;

        public SemanticEnvironment()
        {
            _snapshots = new Stack<EnvironmentSnapshot>();
            _snapshots.Push(new EnvironmentSnapshot());
        }

        public void PushSymbol(Symbol symbol)
        {
            if (SymbolExist(symbol.Id))
                throw new SemanticException($"redeclaration of '{symbol.Id}'");

            _snapshots.Peek().SymbolTable.PushSymbol(symbol);
        }
        
        public bool SymbolExist(string id)
        {
            foreach (var snapshot in _snapshots)
            {
                if (snapshot.SymbolTable.SymbolExist(id))
                    return true;
            }

            return false;
        }

        public void PushSnapshot()
        {
            _snapshots.Push(new EnvironmentSnapshot());
        }

        public EnvironmentSnapshot PopSnapshot()
        {
            return _snapshots.Pop();
        }

        public void PushStructType(StructType type)
        {
            if (StructExist(type.Name))
                throw new SemanticException($"redeclaration of '{type.Name}'");
            
            _snapshots.Peek().StructTypes.Add(type.Name, type);
        }

        public bool StructExist(string name)
        {
            foreach (var snapshot in _snapshots)
            {
                if (snapshot.StructTypes.ContainsKey(name))
                    return true;
            }

            return false;
        }

        public StructType GetStructType(string name)
        { 
            foreach (var snapshot in _snapshots)
            {
                if (snapshot.StructTypes.ContainsKey(name))
                    return snapshot.StructTypes[name];
            }

            throw new SemanticException($"struct '{name}' is not define");
        }
    }
}