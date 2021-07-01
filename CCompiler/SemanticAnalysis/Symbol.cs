using CCompiler.Tokenizer;

namespace CCompiler.SemanticAnalysis
{
    public class Symbol
    {
        public string Id { get; }
        public SymbolType Type { get; }
        public Position DeclPosition { get; }

        public Symbol(string id, SymbolType type, Position declPosition)
        {
            Id = id;
            Type = type;
            DeclPosition = declPosition;
        }
        
        public override string ToString()
        {
            return $"{Type} :: {Id}";
        }

        public override bool Equals(object? obj) // TODO пока так, но не уверен
        {
            if (obj is Symbol symbol)
            {
                return Type.Equals(symbol.Type);
            }

            return false;
        }
    }

    public class VarSymbol : Symbol
    {
        public VarSymbol(string id, SymbolType type, Position declPosition) : base(id, type, declPosition)
        {
        }
    }

    public class FuncSymbol : Symbol
    {
        public EnvironmentSnapshot Snapshot { get; private set; }
        public bool IsDefined { get; private set; }
        
        public FuncSymbol(string id, FuncType type, Position declPosition) : base(id, type, declPosition)
        {
            IsDefined = false;
        }
        public FuncSymbol(string id, FuncType type, EnvironmentSnapshot snapshot, Position declPosition) : base(id, type, declPosition)
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