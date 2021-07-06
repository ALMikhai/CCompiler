using System;
using CCompiler.SemanticAnalysis;
using CCompiler.Tokenizer;
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
            var symbol = environment.GetSymbol(IdName);
            switch (symbol)
            {
                case VarSymbol {IsArg: true} varSymbol:
                    il.Emit(OpCodes.Ldarg, varSymbol.ParameterDefinition);
                    break;
                case VarSymbol varSymbol:
                    il.Emit(OpCodes.Ldloc, varSymbol.VariableDefinition);
                    break;
                case FuncSymbol funcSymbol:
                    il.Emit(OpCodes.Call, environment.MethodDefinitions[funcSymbol.Id]);
                    break;
            }
        }
    }

    public partial class String
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            il.Emit(OpCodes.Ldstr, Utils.ConvertCFormatToCsFormat(Str));
        }
    }

    public partial class FuncCall
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var list = new ExpList();
            if (ExpList is ExpList expList)
                list = expList;

            foreach (var node in list.Nodes)
                node.Generate(ref methodDefinition, ref environment);
            
            PostfixNode.Generate(ref methodDefinition, ref environment);
        }
    }

    public partial class ExpStat
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            if (ExpNode is ExpNode expNode)
                expNode.Generate(ref methodDefinition, ref environment);
        }
    }
}