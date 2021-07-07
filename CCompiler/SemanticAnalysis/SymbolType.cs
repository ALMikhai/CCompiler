using System;
using System.Collections.Generic;
using System.Linq;
using CCompiler.CodeGenerator;
using CCompiler.Tokenizer;
using Mono.Cecil;

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
        public virtual TypeReference ToTypeReference(ref Assembly assembly)
        {
            return SymbolTypeKind switch
            {
                SymbolTypeKind.VOID => assembly.AssemblyDefinition.MainModule.TypeSystem.Void,
                SymbolTypeKind.INT => assembly.AssemblyDefinition.MainModule.TypeSystem.Int64,
                SymbolTypeKind.FLOAT => assembly.AssemblyDefinition.MainModule.TypeSystem.Double,
                SymbolTypeKind.STRING => assembly.AssemblyDefinition.MainModule.TypeSystem.String,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public class FuncType : SymbolType
    {
        public SymbolType ReturnType { get; }
        public EnvironmentSnapshot ArgumentsSnapshot { get; }

        public FuncType(SymbolType returnType, EnvironmentSnapshot argumentsSnapshot) : base(true, false, SymbolTypeKind.FUNC)
        {
            ReturnType = returnType;
            ArgumentsSnapshot = argumentsSnapshot;
        }
        
        public override bool Equals(object? obj)
        {
            if (!(obj is FuncType funcType)) return false;
            if (ReturnType.Equals(funcType.ReturnType) == false ||
                ArgumentsSnapshot.SymbolTable.Equals(funcType.ArgumentsSnapshot.SymbolTable) == false ||
                ArgumentsSnapshot.StructTable.Equals(funcType.ArgumentsSnapshot.StructTable) == false)
                return false;

            return true;
        }

        public List<Symbol> GetArguments()
        {
            return ArgumentsSnapshot.SymbolTable.GetData().Values.ToList();
        }
        public override string GetFullName() =>
            $"{SymbolTypeKind} returning {ReturnType.GetShortName()}\nArguments{ArgumentsSnapshot.SymbolTable}";

        public override string GetShortName() => GetFullName();
        public override TypeReference ToTypeReference(ref Assembly assembly)
        {
            throw new NotImplementedException();
        }
    }

    public class StructType : SymbolType
    {
        public Position DeclPosition { get; }
        public string Name { get; }
        public Table<Symbol> Members { get; }
        public TypeReference TypeReference { get; set; } // TODO For generation.

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
        public override TypeReference ToTypeReference(ref Assembly assembly) => TypeReference;

        public void Generate(ref Assembly assembly, ref SemanticEnvironment environment)
        {
            var mainModule = assembly.AssemblyDefinition.MainModule;
            var structDefinition = new TypeDefinition("app", Name,
                TypeAttributes.SequentialLayout | TypeAttributes.Public | TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit,
                mainModule.ImportReference(typeof(ValueType)));
            
            foreach (var (id, symbol) in Members.GetData())
            {
                var fieldDefinition = new FieldDefinition(id, FieldAttributes.Public, symbol.Type.ToTypeReference(ref assembly));
                ((VarSymbol) symbol).VariableType = VarSymbol.VarType.FIELD;
                ((VarSymbol) symbol).FieldDefinition = fieldDefinition;
                structDefinition.Fields.Add(fieldDefinition);
            }
            
            mainModule.Types.Add(structDefinition);
            TypeReference = mainModule.ImportReference(structDefinition);
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

        public override string GetFullName() => base.GetFullName() + $" to {PointerToType}";
        public override string GetShortName() => GetFullName();

        public override TypeReference ToTypeReference(ref Assembly assembly) =>
            new Mono.Cecil.PointerType(PointerToType.ToTypeReference(ref assembly));
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

        public override TypeReference ToTypeReference(ref Assembly assembly) =>
            new Mono.Cecil.ArrayType(TypeOfArray.ToTypeReference(ref assembly));
    }
}