﻿using System.Collections.Generic;

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
        public bool IsScalar { get; }
        public SymbolTypeKind SymbolTypeKind { get; set; }

        public SymbolType(bool isConst, bool isVolatile, SymbolTypeKind symbolTypeKind)
        {
            IsConst = isConst;
            IsVolatile = isVolatile;
            SymbolTypeKind = symbolTypeKind;
            IsScalar = symbolTypeKind == SymbolTypeKind.INT || symbolTypeKind == SymbolTypeKind.FLOAT;
        }

        public override bool Equals(object? obj)
        {
            if (obj is SymbolType type)
            {
                return SymbolTypeKind == type.SymbolTypeKind;
            }

            return false;
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
        
        public override bool Equals(object? obj)
        {
            if (!(obj is FuncType funcType)) return false;
            if (ReturnType.Equals(funcType.ReturnType) == false ||
                Snapshot.SymbolTable.Equals(funcType.Snapshot.SymbolTable) == false ||
                Snapshot.StructTable.Equals(funcType.Snapshot.StructTable) == false)
                return false;

            return true;
        }

        public Dictionary<string, Symbol> GetArguments()
        {
            return Snapshot.SymbolTable.GetData();
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
        
        public override bool Equals(object? obj)
        {
            if (!(obj is StructType structType)) return false;
            return Name == structType.Name && Members.Equals(structType.Members);
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
        
        public override bool Equals(object? obj)
        {
            if (obj is PointerType type)
            {
                return PointerToType.Equals(type.PointerToType);
            }

            return false;
        }
        
        public override string ToString()
        {
            return base.ToString() + $" to {PointerToType}";
        }
    }
    
    public class ArrayType : SymbolType
    {
        public SymbolType TypeOfArray { get; }

        public ArrayType(bool isConst, bool isVolatile, SymbolType typeOfArray) : base(isConst, isVolatile, SymbolTypeKind.ARRAY)
        {
            TypeOfArray = typeOfArray;
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is ArrayType type)
            {
                return TypeOfArray.Equals(type.TypeOfArray);
            }

            return false;
        }
        
        public override string ToString()
        {
            return base.ToString() + $" of type {TypeOfArray}";
        }
    }
}