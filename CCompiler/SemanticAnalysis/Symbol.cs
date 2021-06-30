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
        public EnvironmentSnapshot Snapshot { get; private set; }
        public bool IsDefined { get; private set; }
        
        public FuncSymbol(string id, FuncType type) : base(id, type)
        {
            IsDefined = false;
        }
        public FuncSymbol(string id, FuncType type, EnvironmentSnapshot snapshot) : base(id, type)
        {
            Snapshot = snapshot;
            IsDefined = true;
        }

        public void SetSnapshot(EnvironmentSnapshot snapshot)
        {
            Snapshot = snapshot;
            IsDefined = true;
        }
        
        public override string ToString()
        {
            return $"{Type}" + (IsDefined ? $"\n{Snapshot} " : " ") + $":: {Id}";
        }
    }
}