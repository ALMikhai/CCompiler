using System;

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
        public bool IsConst { get; set; }
        public bool IsVolatile { get; set; }
        public SymbolTypeKind SymbolTypeKind { get; set; }

        public SymbolType(bool isConst, bool isVolatile, SymbolTypeKind symbolTypeKind)
        {
            IsConst = isConst;
            IsVolatile = isVolatile;
            SymbolTypeKind = symbolTypeKind;
        }
        
        public override string ToString()
        {
            return $"{(IsConst ? "const " : "")}{(IsVolatile ? "volatile " : "")}{SymbolTypeKind}";
        }
    }

    public class FuncType : SymbolType
    {
        public FuncType(SymbolType returnType, SymbolTable arguments) : base(true, false, SymbolTypeKind.FUNC)
        {
            
        }
    }

    public class StructType : SymbolType
    {
        public string Name { get; }
        public SymbolTable Members { get; }

        public StructType(bool isConst, bool isVolatile, string name, SymbolTable members) : base(isConst, isVolatile, SymbolTypeKind.STRUCT)
        {
            Name = name;
            Members = members;
        }
    }

    public class PointerType : SymbolType
    {
        public PointerType(bool isConst, bool isVolatile, SymbolType type) : base(isConst, isVolatile, SymbolTypeKind.POINTER)
        {
            
        }
    }
    
    public class ArrayType : SymbolType
    {
        public ArrayType(bool isConst, bool isVolatile, SymbolType type) : base(isConst, isVolatile, SymbolTypeKind.POINTER)
        {
            
        }
    }
}