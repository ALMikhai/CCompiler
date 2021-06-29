namespace CCompiler.SemanticAnalysis
{
    public class SemanticEnvironment
    {
        public SymbolTable SymbolTable { get; } = new SymbolTable();
    }
}