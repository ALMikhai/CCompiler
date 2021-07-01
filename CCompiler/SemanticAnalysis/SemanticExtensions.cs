using System;
using System.Globalization;
using CCompiler.SemanticAnalysis;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class Node
    {
        public virtual void CheckSemantic(ref SemanticEnvironment environment) =>
            throw new NotImplementedException();
    }
    
    public partial class TranslationUnit
    {
        public override void CheckSemantic(ref SemanticEnvironment environment)
        {
            foreach (var node in Nodes)
                node.CheckSemantic(ref environment);
        }
    }

    public partial class Decl
    {
        public override void CheckSemantic(ref SemanticEnvironment environment)
        {
            var type = DeclSpecs.NodeToType(DeclSpec, ref environment);
            
            foreach (var node in InitDeclaratorList.Nodes)
            {
                environment.PushSymbol((node as InitDeclarator).ParseSymbolByType(type, ref environment));
            }
        }
    }

    public partial class DeclSpecs
    {
        public static SymbolType NodeToType(Node declSpecs, ref SemanticEnvironment environment)
        {
            if (!(declSpecs is NullStat) && !(declSpecs is DeclSpecs))
                throw new ArgumentException($"expected DeclSpecs, actual {declSpecs.GetType()}");
            
            var storageClassSpecExist = false;
            var typeSpecExist = false;

            var symbolType = new SymbolType(false, false, SymbolTypeKind.INT);

            while (!(declSpecs is NullStat))
            {
                var spec = (declSpecs as DeclSpecs).Spec;
                if (spec is StorageClassSpec storageClassSpec)
                {
                    if (storageClassSpecExist)
                        throw new SemanticException("multiple storage classes in declaration specifiers",
                            storageClassSpec.Token);
                    storageClassSpecExist = true;
                }

                if (spec is TypeSpec typeSpec)
                {
                    if (typeSpecExist)
                        throw new SemanticException("two or more data types in declaration specifiers",
                        typeSpec.TokenType);
                    typeSpecExist = true;

                    switch (typeSpec.TokenType.Type)
                    {
                        case KeywordType.INT:
                            symbolType.SymbolTypeKind = SymbolTypeKind.INT;
                            break;
                        case KeywordType.FLOAT:
                            symbolType.SymbolTypeKind = SymbolTypeKind.FLOAT;
                            break;
                        case KeywordType.VOID:
                            symbolType.SymbolTypeKind = SymbolTypeKind.VOID;
                            break;
                        default:
                            throw new SemanticException("this data type is not supported",
                                typeSpec.TokenType);
                    }
                }

                if (spec is StructSpec structSpec)
                    symbolType = structSpec.ParseType(ref environment);
                
                if (spec is TypeQualifier typeQualifier)
                {
                    switch (typeQualifier.Token.Type)
                    {
                        case KeywordType.CONST:
                            symbolType.IsConst = true;
                            break;
                        case KeywordType.VOLATILE:
                            symbolType.IsVolatile = true;
                            break;
                    }
                }

                declSpecs = (declSpecs as DeclSpecs).NextSpec;
            }

            return symbolType;
        }
    }

    public partial class InitDeclaratorByList
    {
        public override Symbol ParseSymbolByType(SymbolType type, ref SemanticEnvironment environment) =>
            throw new NotImplementedException("initializing by InitializerList is not supported");
    }

    public partial class InitDeclaratorByExp
    {
        public override Symbol ParseSymbolByType(SymbolType type, ref SemanticEnvironment environment)
        {
            var symbol = Declarator.ParseSymbol(type, ref environment);
            var valueType = (Initializer as ExpNode).GetType(ref environment);
            if (symbol.Type.Equals(valueType))
                return symbol;

            throw new SemanticException(
                $"trying to assign to variable of type {symbol.Type} value with type {valueType}");
        }
    }

    public partial class InitDeclarator
    {
        public virtual Symbol ParseSymbolByType(SymbolType type, ref SemanticEnvironment environment)
        {
            return Declarator.ParseSymbol(type, ref environment);
        }
    }

    public partial class Declarator
    {
        public Symbol ParseSymbol(SymbolType type, ref SemanticEnvironment environment)
        {
            if (Pointer is Pointer pointer)
            {
                type = pointer.ParseType(type, ref environment);
            }

            return DirectDeclarator switch
            {
                Id id => new VarSymbol(id.IdName, type),
                Declarator declarator => declarator.ParseSymbol(type, ref environment),
                GenericDeclaration genericDeclaration => genericDeclaration.ParseSymbol(type, ref environment),
                _ => throw new ArgumentException()
            };
        }
    }

    public abstract partial class GenericDeclaration
    {
        public virtual Symbol ParseSymbol(SymbolType type, ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class FuncDecl
    {
        public override Symbol ParseSymbol(SymbolType type, ref SemanticEnvironment environment)
        {
            FuncType funcType;
            
            if (ParamList is EmptyExp)
            {
                funcType = new FuncType(type, new EnvironmentSnapshot());
            }
            else if (ParamList is ParamList paramList)
            {
                environment.PushSnapshot();
                foreach (var node in paramList.Nodes)
                {
                    environment.PushSymbol((node as ParamDecl).ParseSymbol(ref environment));
                }

                funcType = new FuncType(type, environment.PopSnapshot());
            }
            else if (ParamList is IdList idList)
            {
                environment.PushSnapshot();
                foreach (var node in idList.Nodes)
                {
                    var varSymbol = new VarSymbol((node as Id).IdName,
                        new SymbolType(false, false, SymbolTypeKind.INT));
                    environment.PushSymbol(varSymbol);
                }
                funcType = new FuncType(type, environment.PopSnapshot());
            }
            else
            {
                throw new ArgumentException();
            }
            
            return Left switch
            {
                Id id => new FuncSymbol(id.IdName, funcType),
                Declarator declarator => declarator.ParseSymbol(funcType, ref environment),
                GenericDeclaration genericDeclaration => genericDeclaration.ParseSymbol(funcType, ref environment),
                _ => throw new ArgumentException()
            };
        }
    }

    public partial class ArrayDecl
    {
        public override Symbol ParseSymbol(SymbolType type, ref SemanticEnvironment environment)
        {
            var arrayType = new ArrayType(type.IsConst, type.IsVolatile, type);
            return Left switch
            {
                Id id => new VarSymbol(id.IdName, arrayType),
                Declarator declarator => declarator.ParseSymbol(arrayType, ref environment),
                GenericDeclaration genericDeclaration => genericDeclaration.ParseSymbol(arrayType, ref environment),
                _ => throw new ArgumentException()
            };
        }
    }

    public partial class Pointer
    {
        public PointerType ParseType(SymbolType symbolType, ref SemanticEnvironment environment)
        {
            if (PointerNode is Pointer pointer)
                symbolType = pointer.ParseType(symbolType, ref environment);

            var type = new PointerType(false, false, symbolType);
            
            if (!(TypeQualifierList is NullStat))
            {
                foreach (var node in (TypeQualifierList as List).Nodes)
                {
                    switch ((node as TypeQualifier).Token.Type)
                    {
                        case KeywordType.CONST:
                            type.IsConst = true;
                            break;
                        case KeywordType.VOLATILE:
                            type.IsVolatile = true;
                            break;
                    }
                }
            }

            return type;
        }
    }
    
    public partial class ParamDecl
    {
        public Symbol ParseSymbol(ref SemanticEnvironment environment)
        {
            var type = DeclSpecs.NodeToType(DeclSpec, ref environment);
            return Declarator.ParseSymbol(type, ref environment);
        }
    }

    public partial class StructSpec
    {
        public StructType ParseType(ref SemanticEnvironment environment)
        {
            if (StructDeclList is EmptyExp)
            {
                if (environment.StructExist(Id.IdName))
                {
                    return environment.GetStructType(Id.IdName);
                }
                
                throw new SemanticException($"storage size of ‘{Id.IdName}’ isn’t known");
            }

            var symbolTable = new Table<Symbol>();

            if (StructDeclList is StructDeclList structDeclList)
            {
                foreach (var node in structDeclList.Nodes)
                {
                    var structDecl = node as StructDecl;
                    var type = DeclSpecs.NodeToType(structDecl.DeclSpec, ref environment);

                    foreach (var declaratorListNode in structDecl.DeclaratorList.Nodes)
                    {
                        var symbol = (declaratorListNode as Declarator).ParseSymbol(type, ref environment);
                        symbolTable.Push(symbol.Id, symbol);
                    }
                }
            }

            var structType = new StructType(false, false, Id.IdName, symbolTable);
            environment.PushStructType(structType);
            return structType;
        }
    }

    public partial class FuncDef
    {
        public override void CheckSemantic(ref SemanticEnvironment environment)
        {
            var returnType = new SymbolType(false, false, SymbolTypeKind.INT);
            if (DeclSpec is DeclSpecs declSpecs)
            {
                returnType = DeclSpecs.NodeToType(declSpecs, ref environment);
            }

            var symbol = Declarator.ParseSymbolByType(returnType, ref environment) as FuncSymbol;
            if (!(DeclList is NullStat))
                throw new SemanticException("old-style (K&R) function definition is not supported");
            
            environment.PushSnapshot((symbol.Type as FuncType).Snapshot);

            if (CompoundStat.DeclList is DeclList declList)
            {
                foreach (var node in declList.Nodes)
                {
                    node.CheckSemantic(ref environment);
                }
            }
            
            symbol.SetSnapshot(environment.PopSnapshot());
            environment.PushSymbol(symbol);
            
            // TODO проверка StatList
        }
    }

    public partial class Const
    {
        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            return Token.TokenType switch
            {
                TokenType.FLOAT => new SymbolType(true, false, SymbolTypeKind.FLOAT),
                TokenType.INT => new SymbolType(true, false, SymbolTypeKind.INT),
                TokenType.CHAR => new SymbolType(true, false, SymbolTypeKind.INT),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        public override bool IsLValue() => false;
    }

    public partial class String
    {
        public override bool IsLValue() => false;
        public override SymbolType GetType(ref SemanticEnvironment environment) => new SymbolType(true, false, SymbolTypeKind.STRING);
    }

    public partial class Id
    {
        public override bool IsLValue() => true;
        public override SymbolType GetType(ref SemanticEnvironment environment) => environment.GetSymbol(IdName).Type;
    }
    
    public partial class AccessingArrayElement
    {
        public override bool IsLValue() => true;
        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            if (PostfixNode.GetType(ref environment) is ArrayType prefixType)
            {
                var symbolType = Exp.GetType(ref environment);
                if (symbolType.SymbolTypeKind == SymbolTypeKind.INT)
                {
                    return prefixType.TypeOfArray;
                }
                
                throw new SemanticException("array subscript is not an integer");
            }

            throw new SemanticException("subscripted value is neither array nor pointer nor vector");
        }
    }

    public partial class MemberCall
    {
        public override bool IsLValue() => true;
        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            if (_callType == CallType.VALUE)
            {
                if (PostfixNode.GetType(ref environment) is StructType structType)
                {
                    var name = (Id as Id).IdName;
                    if (structType.Members.Exist(name))
                    {
                        return structType.Members.Get(name).Type;
                    }
                    
                    throw new SemanticException($"‘{structType.Name}’ has no member named ‘{name}’");
                }
                throw new SemanticException($"request for member in something not a structure");
            }
            else
            {
                if (PostfixNode.GetType(ref environment) is PointerType {PointerToType: StructType structType})
                {
                    var name = (Id as Id).IdName;
                    if (structType.Members.Exist(name))
                    {
                        return structType.Members.Get(name).Type;
                    }
                    
                    throw new SemanticException($"‘{structType.Name}’ has no member named ‘{name}’");
                }
                throw new SemanticException($"invalid type argument of ‘->’");
            }
        }
    }

    public partial class PostfixIncDec
    {
        public override bool IsLValue() => false;
        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            if (!PrefixNode.IsLValue())
                throw new SemanticException("lvalue required as inc or dec operand");
            var symbolType = PrefixNode.GetType(ref environment);
            if (symbolType.IsScalar)
            {
                return symbolType;
            }

            throw new SemanticException("INT or FLOAT required for inc or dec operand");
        }
    }

    public partial class PrefixIncDec
    {
        public override bool IsLValue() => false;
        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            if (!PostfixNode.IsLValue())
                throw new SemanticException("lvalue required as inc or dec operand");
            var symbolType = PostfixNode.GetType(ref environment);
            if (symbolType.IsScalar)
            {
                return symbolType;
            }

            throw new SemanticException("INT or FLOAT required for inc or dec operand");
        }
    }

    public partial class UnaryExp
    {
        public override bool IsLValue() => UnaryOperator.Operator.Type == OperatorType.MULT;
        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            var symbolType = UnaryExpNode.GetType(ref environment);
            if (UnaryOperator.Operator.Type == OperatorType.MULT)
            {
                if (UnaryExpNode.IsLValue())
                {
                    if (symbolType is PointerType pointerType)
                    {
                        return pointerType.PointerToType;
                    }
                    throw new SemanticException("invalid type argument of unary");
                }
                throw new SemanticException("lvalue required as operand");
            }

            if (UnaryOperator.Operator.Type == OperatorType.BITAND)
            {
                if (UnaryExpNode.IsLValue())
                {
                    return new PointerType(false, false, symbolType);
                }
                throw new SemanticException("lvalue required as operand");
            }
            
            if (symbolType.IsScalar)
            {
                return symbolType;
            }
            
            throw new SemanticException("INT or FLOAT required as operand");
        }
    }
    
    public partial class AdditiveExp
    {
        public override bool IsLValue() => false;
        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class MultExp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ShiftExp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class RelationalExp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class EqualityExp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class AndExp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ExclusiveOrExp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class InclusiveOrExp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class LogicalAndExp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class LogicalOrExp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class AssignmentExp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Exp
    {
        public override bool IsLValue()
        {
            throw new NotImplementedException();
        }

        public override SymbolType GetType(ref SemanticEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }
}
