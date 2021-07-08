using System;
using CCompiler.SemanticAnalysis;
using CCompiler.Tokenizer;
using Mono.Cecil;
using Mono.Cecil.Cil;
using PointerType = CCompiler.SemanticAnalysis.PointerType;
using TokenType = CCompiler.Tokenizer.TokenType;

namespace CCompiler.Parser
{
    public partial class Node
    {
        public virtual void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment) =>
            throw new NotImplementedException();
        public virtual void Store(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment, Node right) =>
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
                case VarSymbol varSymbol:
                    switch (varSymbol.VariableType)
                    {
                        case VarSymbol.VarType.VARIABLE:
                            il.Emit(OpCodes.Ldloc, varSymbol.VariableDefinition);
                            break;
                        case VarSymbol.VarType.PARAMETER:
                            il.Emit(OpCodes.Ldarg, varSymbol.ParameterDefinition);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case FuncSymbol funcSymbol:
                    il.Emit(OpCodes.Call, funcSymbol.Definition);
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
            
            var il = methodDefinition.Body.GetILProcessor();
            if (PostfixNode is Id id) // Push null to stack if function return void.
            {
                var type = (environment.GetSymbol(id.IdName) as FuncSymbol).Type as FuncType;
                if (type.ReturnType.SymbolTypeKind == SymbolTypeKind.VOID)
                    il.Emit(OpCodes.Ldnull);
            }
            else
                throw new NotImplementedException();
        }
    }

    public partial class ExpStat
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            if (!(ExpNode is ExpNode expNode)) return;
            expNode.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Pop);
        }
    }

    public partial class MemberCall
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            PostfixNode.Generate(ref methodDefinition, ref environment);
            StructType structType;
            if (TypeOfCall == CallType.VALUE)
                structType = PostfixNode.GetType(ref environment) as StructType;
            else
                structType = (PostfixNode.GetType(ref environment) as PointerType).PointerToType as StructType;
            var id = Id as Id;
            var member = structType.Members.Get(id.IdName) as VarSymbol;
            var il = methodDefinition.Body.GetILProcessor();
            il.Emit(OpCodes.Ldfld, member.FieldDefinition);
        }
    }

    public partial class MemberCall
    {
        public override void Store(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment, Node right)
        {
            var il = methodDefinition.Body.GetILProcessor();
            
            if (PostfixNode is Id id)
            {
                var varSymbol = environment.GetSymbol(id.IdName) as VarSymbol;
                if (TypeOfCall == MemberCall.CallType.VALUE)
                    il.Emit(OpCodes.Ldloca_S, varSymbol.VariableDefinition);
                else
                    il.Emit(OpCodes.Ldloc, varSymbol.VariableDefinition);
            }
            else
            {
                throw new NotImplementedException();
            }
                    
            right.Generate(ref methodDefinition, ref environment);
                    
            StructType structType;
            if (TypeOfCall == MemberCall.CallType.VALUE)
                structType = PostfixNode.GetType(ref environment) as StructType;
            else
                structType =
                    (PostfixNode.GetType(ref environment) as PointerType)
                    .PointerToType as StructType;
                    
            var member = structType.Members.Get((Id as Id).IdName) as VarSymbol;
            il.Emit(OpCodes.Stfld, member.FieldDefinition);
        }
    }

    public partial class Id
    {
        public override void Store(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment, Node right)
        {
            var il = methodDefinition.Body.GetILProcessor();
            right.Generate(ref methodDefinition, ref environment);
            var symbol = environment.GetSymbol(IdName);
            switch (symbol)
            {
                case VarSymbol varSymbol:
                    switch (varSymbol.VariableType)
                    {
                        case VarSymbol.VarType.VARIABLE:
                            il.Emit(OpCodes.Stloc, varSymbol.VariableDefinition);
                            break;
                        case VarSymbol.VarType.PARAMETER:
                            il.Emit(OpCodes.Starg, varSymbol.ParameterDefinition);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
            }
        }
    }

    public partial class AccessingArrayElement
    {
        public override void Store(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment, Node right)
        {
            var il = methodDefinition.Body.GetILProcessor();
            PostfixNode.Generate(ref methodDefinition, ref environment);
            Exp.Generate(ref methodDefinition, ref environment);
            right.Generate(ref methodDefinition, ref environment);
            switch ((right as ExpNode).GetType(ref environment).SymbolTypeKind)
            {
                case SymbolTypeKind.INT:
                    il.Emit(OpCodes.Stelem_I8);
                    break;
                case SymbolTypeKind.FLOAT:
                    il.Emit(OpCodes.Stelem_R8);
                    break;
                default:
                    il.Emit(OpCodes.Stelem_Ref);
                    break;
            }
        }
    }

    public partial class UnaryExp
    {
        public override void Store(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment, Node right)
        {
            var il = methodDefinition.Body.GetILProcessor();
            UnaryExpNode.Generate(ref methodDefinition, ref environment);
            right.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Stind_Ref);
        }
    }
    
    public partial class AssignmentExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            Left.Store(ref methodDefinition, ref environment, Right);
            Left.Generate(ref methodDefinition, ref environment);
        }
    }

    public partial class UnaryExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            switch (UnaryOperator.Operator.Type)
            {
                case OperatorType.MULT:
                    UnaryExpNode.Generate(ref methodDefinition, ref environment);
                    if (UnaryExpNode.GetType(ref environment) is PointerType {PointerToType: StructType structType})
                        il.Emit(OpCodes.Ldobj, structType.TypeReference);
                    else
                        il.Emit(OpCodes.Ldind_Ref);
                    break;
                case OperatorType.BITAND:
                    if (UnaryExpNode is Id id)
                    {
                        var varSymbol = environment.GetSymbol(id.IdName) as VarSymbol;
                        il.Emit(OpCodes.Ldloca_S, varSymbol.VariableDefinition);
                        il.Emit(OpCodes.Conv_U);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;
                case OperatorType.ADD:
                    UnaryExpNode.Generate(ref methodDefinition, ref environment);
                    break;
                case OperatorType.SUB:
                    il.Emit(OpCodes.Ldc_I8, 0L);
                    UnaryExpNode.Generate(ref methodDefinition, ref environment);
                    il.Emit(OpCodes.Sub);
                    break;
                case OperatorType.TILDE:
                    UnaryExpNode.Generate(ref methodDefinition, ref environment);
                    il.Emit(OpCodes.Not);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }

    public partial class AccessingArrayElement
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            PostfixNode.Generate(ref methodDefinition, ref environment);
            Exp.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Ldelem_Ref);
        }
    }

    public partial class EmptyExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
        }
    }
    
    public partial class IfStat
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            var elseLabel = il.Create(OpCodes.Nop);
            var endLabel = il.Create(OpCodes.Nop);
            Exp.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Brfalse, elseLabel);
            Stat1.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Br, endLabel);
            il.Append(elseLabel);
            Stat2.Generate(ref methodDefinition, ref environment);
            il.Append(endLabel);
        }
    }

    public partial class CompoundStat
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            if (DeclList is DeclList)
                throw new NotImplementedException("Declaring variables in a nested block is prohibited");
            if (StatList is StatList statList)
                foreach (var node in statList.Nodes)
                    node.Generate(ref methodDefinition, ref environment);
        }
    }

    public partial class ForStat
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            var startLabel = il.Create(OpCodes.Nop);
            var checkLabel = il.Create(OpCodes.Nop);
            var expLabel = il.Create(OpCodes.Nop);
            var endLabel = il.Create(OpCodes.Nop);
            environment.LoopsLabels.Push(new WhileStat.Labels(expLabel, endLabel));
            
            Exp1.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Pop);
            il.Emit(OpCodes.Br, checkLabel);
            il.Append(startLabel);
            Stat.Generate(ref methodDefinition, ref environment);
            il.Append(expLabel);
            Exp3.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Pop);
            il.Append(checkLabel);
            Exp2.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Brtrue, startLabel);
            il.Append(endLabel);
            
            environment.LoopsLabels.Pop();
        }
    }

    public partial class WhileStat
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            var startLabel = il.Create(OpCodes.Nop);
            var endLabel = il.Create(OpCodes.Nop);
            environment.LoopsLabels.Push(new Labels(startLabel, endLabel));
            il.Append(startLabel);
            if (WhileType == WhileType.WHILE)
            {
                Exp.Generate(ref methodDefinition, ref environment);
                il.Emit(OpCodes.Brfalse, endLabel);
            }
            Stat.Generate(ref methodDefinition, ref environment);
            if (WhileType == WhileType.DOWHILE)
            {
                Exp.Generate(ref methodDefinition, ref environment);
                il.Emit(OpCodes.Brfalse, endLabel);
            }
            il.Emit(OpCodes.Br, startLabel);
            il.Append(endLabel);
            environment.LoopsLabels.Pop();
        }
    }

    public partial class JumpStat
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            switch (Token.Type)
            {
                case KeywordType.CONTINUE:
                    il.Emit(OpCodes.Br, environment.LoopsLabels.Peek().Start);
                    break;
                case KeywordType.BREAK:
                    il.Emit(OpCodes.Br, environment.LoopsLabels.Peek().End);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }

    public partial class ReturnStat
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            if (Exp is ExpNode exp)
                exp.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Ret);
        }
    }

    public partial class AdditiveExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            Right.Generate(ref methodDefinition, ref environment);
            switch (Token.Type)
            {
                case OperatorType.ADD:
                    il.Emit(OpCodes.Add);
                    break;
                case OperatorType.SUB:
                    il.Emit(OpCodes.Sub);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public partial class MultExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            Right.Generate(ref methodDefinition, ref environment);
            switch (Token.Type)
            {
                case OperatorType.MULT:
                    il.Emit(OpCodes.Mul);
                    break;
                case OperatorType.DIV:
                    il.Emit(OpCodes.Div);
                    break;
                case OperatorType.MOD:
                    il.Emit(OpCodes.Rem);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public partial class ShiftExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            Right.Generate(ref methodDefinition, ref environment);
            switch (Token.Type)
            {
                case OperatorType.LSHIFT:
                    il.Emit(OpCodes.Shl);
                    break;
                case OperatorType.RSHIFT:
                    il.Emit(OpCodes.Shr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public partial class RelationalExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            Right.Generate(ref methodDefinition, ref environment);
            switch (Token.Type)
            {
                case OperatorType.MORE:
                    il.Emit(OpCodes.Cgt);
                    break;
                case OperatorType.LESS:
                    il.Emit(OpCodes.Clt);
                    break;
                case OperatorType.MOREEQ:
                    il.Emit(OpCodes.Clt);
                    il.Emit(OpCodes.Ldc_I8, 0L);
                    il.Emit(OpCodes.Ceq);
                    break;
                case OperatorType.LESSEQ:
                    il.Emit(OpCodes.Cgt);
                    il.Emit(OpCodes.Ldc_I8, 0L);
                    il.Emit(OpCodes.Ceq);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public partial class EqualityExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            Right.Generate(ref methodDefinition, ref environment);
            switch (Token.Type)
            {
                case OperatorType.EQ:
                    il.Emit(OpCodes.Ceq);
                    break;
                case OperatorType.NEQ:
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Ldc_I8, 0L);
                    il.Emit(OpCodes.Ceq);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public partial class AndExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            Right.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.And);
        }
    }

    public partial class ExclusiveOrExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            Right.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Xor);
        }
    }

    public partial class InclusiveOrExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            Right.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Or);
        }
    }

    public partial class LogicalAndExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Ldc_I8, 0L);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Ldc_I8, 0L);
            il.Emit(OpCodes.Ceq);
            
            Right.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Ldc_I8, 0L);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Ldc_I8, 0L);
            il.Emit(OpCodes.Ceq);
            
            il.Emit(OpCodes.And);
        }
    }

    public partial class LogicalOrExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            Right.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Or);
        }
    }

    public partial class ConditionalExp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var ifStat = new IfStat(Condition, Exp1, Exp2);
            ifStat.Generate(ref methodDefinition, ref environment);
        }
    }

    public partial class PrefixIncDec
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var assignmentExp = new AssignmentExp(
                new OperatorToken(TokenType.OPERATOR, "", 0, OperatorType.EQ),
                PostfixNode,
                new AdditiveExp(
                    new OperatorToken(TokenType.OPERATOR, "", 0, _opType == OpType.INC ? OperatorType.ADD : OperatorType.SUB), 
                    PostfixNode,
                    new Const(new Token(TokenType.INT, "1", 1L), new Position(0, 0)), 
                    new Position(0, 0)),
                new Position(0, 0));
            assignmentExp.Generate(ref methodDefinition, ref environment);
        }
    }

    public partial class Exp
    {
        public override void Generate(ref MethodDefinition methodDefinition, ref SemanticEnvironment environment)
        {
            var il = methodDefinition.Body.GetILProcessor();
            Left.Generate(ref methodDefinition, ref environment);
            il.Emit(OpCodes.Pop);
            Right.Generate(ref methodDefinition, ref environment);
        }
    }
}