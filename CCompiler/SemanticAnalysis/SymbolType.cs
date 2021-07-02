using System.Collections.Generic;
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

        public override string ToString() => GetFullName();
        public virtual string GetFullName() =>
            $"{(IsConst ? "const " : "")}{(IsVolatile ? "volatile " : "")}{SymbolTypeKind}";
        public virtual string GetShortName() => GetFullName();
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
        public override string GetFullName() =>
            $"{SymbolTypeKind} returning {ReturnType}\nArguments{Snapshot.SymbolTable}";
        public override string GetShortName() => $"{SymbolTypeKind} returning {ReturnType}";
    }

    public class StructType : SymbolType
    {
        public Position DeclPosition { get; }
        public string Name { get; }
        public Table<Symbol> Members { get; }

        public StructType(bool isConst, bool isVolatile, string name, Table<Symbol> members, Position declPosition) : base(isConst, isVolatile, SymbolTypeKind.STRUCT)
        {
            Name = name;
            Members = members;
            DeclPosition = declPosition;
        }
        
        public override bool Equals(object? obj)
        {
            if (!(obj is StructType structType)) return false;
            return Name == structType.Name && Members.Equals(structType.Members);
        }
        public override string GetFullName() => $"{SymbolTypeKind} called {Name}\nMembers {Members}";
        public override string GetShortName() => $"{SymbolTypeKind} called {Name}";
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
        public override string GetFullName() => base.GetFullName() + $" to {PointerToType}";
        public override string GetShortName() => GetFullName();
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
        
        public override string GetFullName() => base.GetFullName() + $" of type {TypeOfArray}";
        public override string GetShortName() => GetFullName();
    }
}