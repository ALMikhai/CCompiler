using System;
using CCompiler.CodeGenerator;
using CCompiler.Parser;
using CCompiler.Tokenizer;
using Mono.Cecil;
using Mono.Cecil.Cil;
using OpCodes = Mono.Cecil.Cil.OpCodes;

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
        
        public override string ToString() => $"{Type.GetShortName()} :: {Id}";

        public virtual void Generate(ref Assembly assembly, ref SemanticEnvironment environment) =>
            throw new NotImplementedException();
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
        public enum VarType
        {
            VARIABLE,
            PARAMETER,
            FIELD
        }
        
        private ExpNode _initializer;
        public VarType VariableType { get; set; } = VarType.VARIABLE;
        public VariableDefinition VariableDefinition { get; set; }
        public ParameterDefinition ParameterDefinition { get; set; }
        public FieldDefinition FieldDefinition { get; set; }

        public ExpNode Initializer
        {
            get => _initializer;
            set
            {
                _initializer = value;
                IsInitialized = true;
            }
        }

        public bool IsInitialized { get; private set; } = false;

        public VarSymbol(string id, SymbolType type, Position declPosition) : base(id, type, declPosition)
        {
        }
    }

    public class FuncSymbol : Symbol
    {
        public CompoundStat CompoundStat { get; }
        public MethodDefinition Definition { get; protected set; }
        
        public FuncSymbol(string id, FuncType type, Position declPosition, CompoundStat compoundStat) : base(id, type, declPosition)
        {
            CompoundStat = compoundStat;
        }

        public override void Generate(ref Assembly assembly, ref SemanticEnvironment environment)
        {
            environment.PushSnapshot();
            var funcType = Type as FuncType;
            var retType = funcType.ReturnType;

            var isEntryPoint = false;
            if (Id == "main" && funcType.ReturnType.SymbolTypeKind == SymbolTypeKind.INT &&
                funcType.GetArguments().Count == 0)
            {
                isEntryPoint = true;
                retType = new SymbolType(false, false, SymbolTypeKind.VOID);
            }
            
            var methodDefinition = new MethodDefinition(Id, MethodAttributes.Public | MethodAttributes.Static,
                retType.ToTypeReference(ref assembly)) {Body = {InitLocals = true}};
            Definition = methodDefinition;
            var il = methodDefinition.Body.GetILProcessor();

            foreach (var argument in funcType.GetArguments())
            {
                environment.GetCurrentSnapshot().PushSymbol(argument);
                var parameterDefinition = new ParameterDefinition(argument.Id, ParameterAttributes.None,
                    argument.Type.ToTypeReference(ref assembly));
                ((VarSymbol) argument).ParameterDefinition = parameterDefinition;
                methodDefinition.Parameters.Add(parameterDefinition);
            }

            foreach (var (_, symbol) in CompoundStat.Snapshot.SymbolTable.GetData())
            {
                var variableDefinition = new VariableDefinition(symbol.Type.ToTypeReference(ref assembly));
                var varSymbol = symbol as VarSymbol;
                varSymbol.VariableType = VarSymbol.VarType.VARIABLE;
                varSymbol.VariableDefinition = variableDefinition;
                environment.GetCurrentSnapshot().PushSymbol(symbol);
                methodDefinition.Body.Variables.Add(variableDefinition);
                
                if (varSymbol.IsInitialized)
                {
                    varSymbol.Initializer.Generate(il, environment);
                    il.Emit(OpCodes.Stloc, varSymbol.VariableDefinition);
                }
                else switch (varSymbol.Type)
                {
                    case StructType structType:
                        il.Emit(OpCodes.Ldloca_S, varSymbol.VariableDefinition);
                        il.Emit(OpCodes.Initobj, structType.TypeReference);
                        break;
                    case ArrayType arrayType:
                        arrayType.InsideBrackets.Generate(il, environment);
                        il.Emit(OpCodes.Newarr, arrayType.TypeOfArray.ToTypeReference(ref assembly));
                        il.Emit(OpCodes.Stloc, varSymbol.VariableDefinition);
                        break;
                }
            }

            if (CompoundStat.StatList is StatList statList)
                foreach (var node in statList.Nodes)
                    node.Generate(il, environment);

            il.Emit(OpCodes.Ret);

            if (isEntryPoint)
            {
                assembly.AssemblyDefinition.EntryPoint = methodDefinition;
                assembly.AssemblyDefinition.MainModule.EntryPoint = methodDefinition;
            }
            
            assembly.AddMethod(methodDefinition);
            environment.PopSnapshot();
        }

        public override string ToString() =>
            $"{Type.GetFullName()}" + $"\n{CompoundStat.Snapshot} " + $":: {Id}";
    }
}