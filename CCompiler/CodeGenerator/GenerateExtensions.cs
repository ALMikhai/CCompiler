using System;
using CCompiler.SemanticAnalysis;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TokenType = CCompiler.Tokenizer.TokenType;

namespace CCompiler.Parser
{
    public partial class Node
    {
        public virtual void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment) =>
            throw new NotImplementedException();
    }

    public partial class Const
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            switch (Token.TokenType)
            {
                case TokenType.INT:
                    il.Emit(OpCodes.Ldc_I8, (long) Token.Value);
                    break;
                case TokenType.FLOAT:
                    il.Emit(OpCodes.Ldc_R8, (double) Token.Value);
                    break;
                case TokenType.CHAR:
                    il.Emit(OpCodes.Ldc_I8, (long) Token.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public partial class Id
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            var symbol = environment.GetSymbol(IdName) as VarSymbol;
            if (symbol.IsArg)
                il.Emit(OpCodes.Ldarg, symbol.ParameterDefinition);
            else
                il.Emit(OpCodes.Ldloc, symbol.VariableDefinition);
        }
    }
}