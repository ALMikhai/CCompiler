using System.Collections.Generic;

namespace CCompiler.SemanticAnalysis
{
    public class EnvironmentSnapshot
    {
        public Table<Symbol> SymbolTable { get; } = new Table<Symbol>();
        public Table<StructType> StructTable { get; } = new Table<StructType>();

        public override string ToString()
        {
            return $"Structs {StructTable}\nSymbols{SymbolTable}";
        }
    }
}