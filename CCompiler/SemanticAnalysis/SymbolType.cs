﻿using System;

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
        POINTER,
        ARRAY
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
        public SymbolType ReturnType { get; }
        public EnvironmentSnapshot Snapshot { get; }

        public FuncType(SymbolType returnType, EnvironmentSnapshot snapshot) : base(true, false, SymbolTypeKind.FUNC)
        {
            ReturnType = returnType;
            Snapshot = snapshot;
        }
        
        public override string ToString()
        {
            return $"{SymbolTypeKind} returning {ReturnType}\nArguments {Snapshot.SymbolTable}";
        }
    }

    public class StructType : SymbolType
    {
        public string Name { get; }
        public Table<Symbol> Members { get; }

        public StructType(bool isConst, bool isVolatile, string name, Table<Symbol> members) : base(isConst, isVolatile, SymbolTypeKind.STRUCT)
        {
            Name = name;
            Members = members;
        }
        
        public override string ToString()
        {
            return $"{SymbolTypeKind} called {Name}\nMembers {Members}";
        }
    }

    public class PointerType : SymbolType
    {
        public SymbolType PointerToType { get; }

        public PointerType(bool isConst, bool isVolatile, SymbolType pointerToType) : base(isConst, isVolatile, SymbolTypeKind.POINTER)
        {
            PointerToType = pointerToType;
        }
        
        public override string ToString()
        {
            return base.ToString() + $" to {PointerToType}";
        }
    }
    
    public class ArrayType : SymbolType
    {
        public SymbolType Type { get; }

        public ArrayType(bool isConst, bool isVolatile, SymbolType type) : base(isConst, isVolatile, SymbolTypeKind.ARRAY)
        {
            Type = type;
        }
        
        public override string ToString()
        {
            return base.ToString() + $" of type {Type}";
        }
    }
}