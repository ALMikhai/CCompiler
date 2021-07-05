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

        public virtual void Generate(ref Assembly assembly) => throw new NotImplementedException();
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
        private ExpNode _initializer;
        public bool IsArg { get; set; } = false;
        public VariableDefinition VariableDefinition { get; set; }
        public ParameterDefinition ParameterDefinition { get; set; }

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
        public CompoundStat CompoundStat { get; private set; }
        public bool IsDefined { get; private set; }
        
        public FuncSymbol(string id, FuncType type, Position declPosition) : base(id, type, declPosition)
        {
            IsDefined = false;
        }

        public void SetCompoundStat(CompoundStat compoundStat)
        {
            CompoundStat = compoundStat;
            IsDefined = true;
        }

        public override void Generate(ref Assembly assembly)
        {
            var funcType = Type as FuncType;
            var retType = funcType.ReturnType;
            var function = new MethodDefinition(Id, MethodAttributes.Public | MethodAttributes.Static,
                retType.ToTypeReference(ref assembly)) {Body = {InitLocals = true}};

            var il = function.Body.GetILProcessor();

            var semanticEnvironment = new SemanticEnvironment();

            foreach (var argument in funcType.GetArguments())
            {
                semanticEnvironment.GetCurrentSnapshot().PushSymbol(argument.Value);
                var parameterDefinition = new ParameterDefinition(argument.Key, ParameterAttributes.None,
                    argument.Value.Type.ToTypeReference(ref assembly));
                ((VarSymbol) argument.Value).ParameterDefinition = parameterDefinition;
                function.Parameters.Add(parameterDefinition);
            }

            foreach (var symbol in CompoundStat.Snapshot.SymbolTable.GetData())
            {
                var variableDefinition = new VariableDefinition(symbol.Value.Type.ToTypeReference(ref assembly));
                var varSymbol = symbol.Value as VarSymbol;
                varSymbol.VariableDefinition = variableDefinition;
                semanticEnvironment.GetCurrentSnapshot().PushSymbol(symbol.Value);
                function.Body.Variables.Add(variableDefinition);
                
                if (varSymbol.IsInitialized)
                {
                    varSymbol.Initializer.Generate(ref function, ref semanticEnvironment);
                    il.Emit(OpCodes.Stloc, varSymbol.VariableDefinition);
                }
            }
            
            //TODO Generate code for statements.
            
            if (Id == "main" && funcType.ReturnType.SymbolTypeKind == SymbolTypeKind.VOID &&
                funcType.GetArguments().Count == 0)
            {
                assembly.AssemblyDefinition.EntryPoint = function;
                assembly.AssemblyDefinition.MainModule.EntryPoint = function;
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Call, assembly.MethodReferences["WriteLine"]);
            }
            
            assembly.MethodDefinitions.Add(Id, function);
            il.Emit(OpCodes.Ret);
            assembly.AddMethod(function);
        }

        public override string ToString() =>
            $"{Type.GetFullName()}" + (IsDefined ? $"\n{CompoundStat.Snapshot} " : " ") + $":: {Id}";
    }
}