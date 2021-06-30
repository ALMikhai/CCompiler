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

            _snapshots.Peek().SymbolTable.Push(symbol.Id, symbol);
        }

        public bool SymbolExist(string id) => _snapshots.Peek().SymbolTable.Exist(id);

        public void PushSnapshot() => _snapshots.Push(new EnvironmentSnapshot());

        public void PushSnapshot(EnvironmentSnapshot snapshot)
        {
            PushSnapshot();
            foreach (var pair in snapshot.SymbolTable.GetData())
            {
                PushSymbol(pair.Value);
            }
            foreach (var pair in snapshot.StructTable.GetData())
            {
                PushStructType(pair.Value);
            }
        }

        public EnvironmentSnapshot PopSnapshot() => _snapshots.Pop();

        public void PushStructType(StructType type)
        {
            if (StructExist(type.Name))
                throw new SemanticException($"redeclaration of '{type.Name}'");
            
            _snapshots.Peek().StructTable.Push(type.Name, type);
        }

        public bool StructExist(string name) => _snapshots.Peek().StructTable.Exist(name);

        public StructType GetStructType(string name)
        {
            foreach (var snapshot in _snapshots)
            {
                if (snapshot.StructTable.Exist(name))
                    return snapshot.StructTable.Get(name);
            }

            throw new SemanticException($"struct '{name}' is not define");
        }

        public Symbol GetSymbol(string id)
        {
            foreach (var snapshot in _snapshots)
            {
                if (snapshot.SymbolTable.Exist(id))
                    return snapshot.SymbolTable.Get(id);
            }

            throw new SemanticException($"symbol '{id}' is not define");
        }
    }
}
