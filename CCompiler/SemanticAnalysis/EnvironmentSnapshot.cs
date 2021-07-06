namespace CCompiler.SemanticAnalysis
{
    public class EnvironmentSnapshot
    {
        public Table<Symbol> SymbolTable { get; } = new Table<Symbol>();
        public Table<StructType> StructTable { get; } = new Table<StructType>();

        public void PushSymbol(Symbol symbol)
        {
            if (SymbolExist(symbol.Id))
                throw new SemanticException($"redeclaration of '{symbol.Id}'", symbol.DeclPosition);

            SymbolTable.Push(symbol.Id, symbol);
        }

        public bool SymbolExist(string id) => SymbolTable.Exist(id);

        public void PushStructType(StructType type)
        {
            if (StructExist(type.Name))
                throw new SemanticException($"redeclaration of '{type.Name}'", type.DeclPosition);
            
            StructTable.Push(type.Name, type);
        }

        public bool StructExist(string name) => StructTable.Exist(name);

        public override string ToString() => $"Structs {StructTable}\nSymbols {SymbolTable}";
    }
}