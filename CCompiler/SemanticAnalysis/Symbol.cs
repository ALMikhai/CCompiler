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
        
        public override string ToString()
        {
            return $"{Type} :: {Id}";
        }
    }

    public class VarSymbol : Symbol
    {
        public VarSymbol(string id, SymbolType type) : base(id, type)
        {
        }
    }
    
    public class ArraySymbol : Symbol
    {
        public ArraySymbol(string id, ArrayType type) : base(id, type)
        {
        }
    }

    public class PointerSymbol : Symbol
    {
        public Symbol Symbol { get; }

        public PointerSymbol(Symbol symbol, PointerType type) : base(symbol.Id, type)
        {
            Symbol = symbol;
        }

        public PointerSymbol(Symbol symbol) : this(symbol, new PointerType(false, false, symbol.Type))
        {
        }
    }

    public class FuncSymbol : Symbol
    {
        public FuncSymbol(string id, FuncType type) : base(id, type)
        {
        }
    }
}