using System;
using CCompiler.CodeGenerator;
using CCompiler.Parser;
using CCompiler.Tokenizer;
using Mono.Cecil;
using Mono.Cecil.Cil;

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
            var retType = (Type as FuncType).ReturnType;
            var function = new MethodDefinition(Id, MethodAttributes.Public | MethodAttributes.Static,
                retType.ToTypeReference(ref assembly));
            // TODO add params
            var il = function.Body.GetILProcessor();
            // TODO generate compaund

            il.Emit(OpCodes.Ret);
            assembly.AddMethod(function);
        }

        public override string ToString() =>
            $"{Type.GetFullName()}" + (IsDefined ? $"\n{CompoundStat.Snapshot} " : " ") + $":: {Id}";
    }
}