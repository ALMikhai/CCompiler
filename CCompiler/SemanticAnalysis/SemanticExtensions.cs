using System;
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
                (node as InitDeclarator).CheckSemantic(type, ref environment);
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
                    symbolType = structSpec.ParseStructType(ref environment);
                
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
        public override void CheckSemantic(SymbolType type, ref SemanticEnvironment environment) =>
            throw new NotImplementedException("initializing by InitializerList is not supported");
    }

    public partial class InitDeclaratorByExp
    {
        public override void CheckSemantic(SymbolType type, ref SemanticEnvironment environment) =>
            throw new NotImplementedException();
    }

    public partial class InitDeclarator
    {
        public virtual void CheckSemantic(SymbolType type, ref SemanticEnvironment environment)
        {
            environment.PushSymbol(Declarator.ParseSymbol(type, ref environment));
        }
    }

    public partial class Declarator
    {
        public Symbol ParseSymbol(SymbolType type, ref SemanticEnvironment environment)
        {
            Symbol result;
            if (DirectDeclarator is Id id)
            {
                result = new VarSymbol(id.IdName, type);
            }
            else if (DirectDeclarator is GenericDeclaration genericDeclaration)
            {
                result = genericDeclaration.ParseSymbol(type, ref environment);
            }
            else
            {
                throw new ArgumentException();
            }
            
            if (Pointer is Pointer pointer)
            {
                result = pointer.ParseSymbol(result, ref environment);
            }

            return result;
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
            if (Left is Id id)
            {
                if (ParamList is EmptyExp)
                {
                    return new FuncSymbol(id.IdName, new FuncType(type, new SymbolTable()));
                }
                else if (ParamList is ParamList paramList)
                {
                    environment.PushSnapshot();
                    foreach (var node in paramList.Nodes)
                    {
                        environment.PushSymbol((node as ParamDecl).ParseSymbol(ref environment));
                    }

                    return new FuncSymbol(id.IdName, new FuncType(type, environment.PopSnapshot().SymbolTable));
                }
                else if (ParamList is IdList idList)
                {
                    var symbolTable = new SymbolTable();
                    foreach (var node in idList.Nodes)
                    {
                        symbolTable.PushSymbol(new VarSymbol((node as Id).IdName,
                            new SymbolType(false, false, SymbolTypeKind.INT)));
                    }
                    return new FuncSymbol(id.IdName, new FuncType(type, symbolTable));
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            
            throw new NotImplementedException();
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

    public partial class ArrayDecl
    {
        public override Symbol ParseSymbol(SymbolType type, ref SemanticEnvironment environment)
        {
            if (Left is Id id)
            {
                // TODO Check ConstExp.
                return new ArraySymbol(id.IdName, new ArrayType(type.IsConst, type.IsVolatile, type));
            }
            
            throw new NotImplementedException();
        }
    }

    public partial class Pointer
    {
        public Symbol ParseSymbol(Symbol symbol, ref SemanticEnvironment environment)
        {
            if (PointerNode is Pointer pointer)
                symbol = pointer.ParseSymbol(symbol, ref environment);

            var type = new PointerType(false, false, symbol.Type);
            
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

            return new PointerSymbol(symbol, type);
        }
    }

    public partial class StructSpec
    {
        public SymbolType ParseStructType(ref SemanticEnvironment environment)
        {
            if (StructDeclList is EmptyExp)
            {
                if (environment.StructExist(Id.IdName))
                {
                    return environment.GetStructType(Id.IdName);
                }
                
                throw new SemanticException($"storage size of ‘{Id.IdName}’ isn’t known");
            }

            var structTable = new SymbolTable();

            if (StructDeclList is StructDeclList structDeclList)
            {
                foreach (var node in structDeclList.Nodes)
                {
                    var structDecl = node as StructDecl;
                    var type = DeclSpecs.NodeToType(structDecl.DeclSpec, ref environment);

                    foreach (var declaratorListNode in structDecl.DeclaratorList.Nodes)
                    {
                        structTable.PushSymbol((declaratorListNode as Declarator).ParseSymbol(type, ref environment));
                    }
                }
            }

            var structType = new StructType(false, false, Id.IdName, structTable);
            environment.PushStructType(structType);
            return structType;
        }
    }
}
