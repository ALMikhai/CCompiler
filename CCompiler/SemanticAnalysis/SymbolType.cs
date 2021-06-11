using System;
using CCompiler.Parser;
using CCompiler.Tokenizer;

namespace CCompiler.SemanticAnalysis
{
    public enum SymbolTypeKind
    {
        VOID,
        INT,
        FLOAT,
        STRING,
        STRUCT,
        FUNC,
        POINTER
    }
    
    public class SymbolType
    {
        public bool IsConst { get; }
        public bool IsVolatile { get; }
        public SymbolTypeKind SymbolTypeKind { get; }

        public SymbolType(bool isConst, bool isVolatile, SymbolTypeKind symbolTypeKind)
        {
            IsConst = isConst;
            IsVolatile = isVolatile;
            SymbolTypeKind = symbolTypeKind;
        }

        public virtual bool EqualType(SymbolType other) => SymbolTypeKind == other.SymbolTypeKind;
    }

    public class FuncType : SymbolType
    {
        public FuncType(SymbolType returnType, SymbolTable arguments) : base(true, false, SymbolTypeKind.FUNC)
        {
            
        }

        public override bool EqualType(SymbolType other)
        {
            throw new NotImplementedException();
        }
    }

    public class StructType : SymbolType
    {
        public StructType(bool isConst, bool isVolatile, string name, SymbolTable members) : base(isConst, isVolatile, SymbolTypeKind.STRUCT)
        {
            
        }
        
        public override bool EqualType(SymbolType other)
        {
            throw new NotImplementedException();
        }
    }

    public class PointerType : SymbolType
    {
        public PointerType(bool isConst, bool isVolatile, SymbolType type) : base(isConst, isVolatile, SymbolTypeKind.POINTER)
        {
            
        }
        
        public override bool EqualType(SymbolType other)
        {
            throw new NotImplementedException();
        }
    }
}