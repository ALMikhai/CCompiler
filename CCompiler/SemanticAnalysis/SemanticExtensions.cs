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
            var type = DeclSpecs.Parse(DeclSpec);
            
            foreach (var node in InitDeclaratorList.Nodes)
            {
                environment.SymbolTable.PushSymbol((node as InitDeclarator).CreateSymbol(type));
            }
        }
    }

    public partial class DeclSpecs
    {
        public static SymbolType Parse(Node declSpecs)
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
                        default:
                            throw new SemanticException("this data type is not supported",
                                typeSpec.TokenType);
                    }
                }

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

                if (spec is StructSpec)
                    throw new NotImplementedException();

                declSpecs = (declSpecs as DeclSpecs).NextSpec;
            }

            return symbolType;
        }
    }

    public partial class InitDeclaratorByList
    {
        public override Symbol CreateSymbol(SymbolType type) =>
            throw new NotImplementedException("initializing by InitializerList is not supported");
    }

    public partial class InitDeclaratorByExp
    {
        public override Symbol CreateSymbol(SymbolType type)
        {
            throw new NotImplementedException();
            //Declarator.CheckSemantic(ref environment);
            /*
             * if (environment.LastAddedSymbol.Type == Initializer.GetType)
             *      ok
             * else
             *      throw exception
             */
            // TODO Сравнить тип полученный в Declarator и тип полученный в Initializer где Initializer это exp.
        }
    }

    public partial class InitDeclarator
    {
        public virtual Symbol CreateSymbol(SymbolType type)
        {
            return Declarator.CreateSymbol(type);
        }
    }

    public partial class Declarator
    {
        public Symbol CreateSymbol(SymbolType type)
        {
            if (DirectDeclarator is Id id)
            {
                return new VarSymbol(id.IdName, type, 0);
            }

            if (DirectDeclarator is GenericDeclaration genericDeclaration)
            {
                // TODO norm them => symbol = genericDeclaration.Parse(type);
                // TODO Create new symbol.
            }
            
            if (!(Pointer is NullStat))
            {
                // TODO Create new pointer symbol.
            }

            throw new NotImplementedException();
        }
    }

    public partial class FuncDecl
    {
        public Symbol Parse(SymbolType symbolType)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ArrayDecl
    {
        public Symbol Parse(SymbolType symbolType)
        {
            throw new NotImplementedException();
        }
    }
}
