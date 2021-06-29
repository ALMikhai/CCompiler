namespace CCompiler.SemanticAnalysis
{
    public class Symbol
    {
        public string Id { get; }
        public SymbolType Type { get; }

        public Symbol(string id, SymbolType type)
        {
            Id = id;
            Type = type;
        }
    }

    public class VarSymbol : Symbol
    {
        public object Value { get; }

        public VarSymbol(string id, SymbolType type, object value) : base(id, type)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"Var \t {Type} \t {Id} \t {Value}";
        }
    }
}