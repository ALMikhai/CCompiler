using System.Collections.Generic;

namespace CCompiler.SemanticAnalysis
{
    public class EnvironmentSnapshot
    {
        public SymbolTable SymbolTable { get; } = new SymbolTable();
        public Dictionary<string, StructType> StructTypes { get; } = new Dictionary<string, StructType>();
    }
}