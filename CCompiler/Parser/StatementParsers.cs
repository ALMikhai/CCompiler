using System;
using System.Reflection;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class SyntaxParser
    {
        /*
         * stat	: labeled_stat
            | exp_stat
            | compound_stat
            | selection_stat
            | iteration_stat
            | jump_stat
            ;
         */

        private IParseResult ParseStat()
        {
            var expStat = ParseExpStat();
            if (expStat.IsSuccess)
                return expStat;

            var compoundStat = ParseCompoundStat();
            if (compoundStat.IsSuccess)
                return compoundStat;

            var selectionStat = ParseSelectionStat();
            if (selectionStat.IsSuccess)
                return selectionStat;

            var iterationStat = ParseIterationStat();
            if (iterationStat.IsSuccess)
                return iterationStat;

            var jumpStat = ParseJumpStat();
            if (jumpStat.IsSuccess)
                return jumpStat;

            return new FailedParseResult("expected statement", _currentToken);
        }
        
        /*
         *jump_stat	: 'goto' id ';'
            | 'continue' ';'
            | 'break' ';'
            | 'return' exp ';'
            | 'return'	';'
            ;
         */

        private IParseResult ParseJumpStat()
        {
            if (AcceptKeyword(KeywordType.CONTINUE))
            {
                if (ExceptOp(OperatorType.SEMICOLON))
                {
                    return new SuccessParseResult(new JumpStat(KeywordType.CONTINUE));
                }
            }

            if (AcceptKeyword(KeywordType.BREAK))
            {
                if (ExceptOp(OperatorType.SEMICOLON))
                {
                    return new SuccessParseResult(new JumpStat(KeywordType.BREAK));
                }
            }
            
            if (AcceptKeyword(KeywordType.RETURN))
            {
                if (AcceptOp(OperatorType.SEMICOLON))
                {
                    return new SuccessParseResult(new JumpStat(KeywordType.RETURN));
                }

                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;
                
                if (ExceptOp(OperatorType.SEMICOLON))
                {
                    return new SuccessParseResult(new ReturnStat(exp.ResultNode));
                }
            }
            
            return new FailedParseResult("expected jump statement", _currentToken);
        }
        
        /*
         * selection_stat : 'if' '(' exp ')' stat
            | 'if' '(' exp ')' stat 'else' stat
            | 'switch' '(' exp ')' stat
            ;
         */

        private IParseResult ParseSelectionStat()
        {
            if (AcceptKeyword(KeywordType.IF))
            {
                if (ExceptOp(OperatorType.LRBRACKET))
                {
                    var exp = ParseExp();
                    if (!exp.IsSuccess)
                    {
                        return exp;
                    }
                    
                    if (ExceptOp(OperatorType.RRBRACKET))
                    {
                        var stat = ParseStat();
                        if (!stat.IsSuccess)
                        {
                            return stat;
                        }

                        if (AcceptKeyword(KeywordType.ELSE))
                        {
                            var stat2 = ParseJumpStat();
                            if (!stat2.IsSuccess)
                            {
                                return stat2;
                            }

                            return new SuccessParseResult(new IfStat(exp.ResultNode, stat.ResultNode,
                                stat2.ResultNode));
                        }

                        return new SuccessParseResult(new IfStat(exp.ResultNode, stat.ResultNode, new NullStat()));
                    }
                }
            }

            if (AcceptKeyword(KeywordType.SWITCH))
            {
                if (ExceptOp(OperatorType.LRBRACKET))
                {
                    var exp = ParseExp();
                    if (!exp.IsSuccess)
                    {
                        return exp;
                    }

                    if (ExceptOp(OperatorType.RRBRACKET))
                    {
                        var stat = ParseStat();
                        if (!stat.IsSuccess)
                        {
                            return stat;
                        }
                        
                        return new SuccessParseResult(new SwitchStat(exp.ResultNode, stat.ResultNode));
                    }
                }
            }
            
            return new FailedParseResult("expected selection statement", _currentToken);
        }
        
        /*
         * exp_stat	: exp ';'
			|	';'
			;
         */

        private IParseResult ParseExpStat()
        {
            if (AcceptOp(OperatorType.SEMICOLON))
            {
                return new SuccessParseResult(new NullStat());
            }

            var exp = ParseExp();
            if (!exp.IsSuccess)
                return exp;

            if (ExceptOp(OperatorType.SEMICOLON))
            {
                return exp;
            }
            
            return new FailedParseResult("expected expression statement", _currentToken);
        }
        
        /*
         * iteration_stat : 'while' '(' exp ')' stat
            | 'do' stat 'while' '(' exp ')' ';'
            | 'for' '(' exp ';' exp ';' exp ')' stat
            | 'for' '(' exp ';' exp ';'	')' stat
            | 'for' '(' exp ';'	';' exp ')' stat
            | 'for' '(' exp ';'	';'	')' stat
            | 'for' '('	';' exp ';' exp ')' stat
            | 'for' '('	';' exp ';'	')' stat
            | 'for' '('	';'	';' exp ')' stat
            | 'for' '('	';'	';'	')' stat
            ;
         */

        private IParseResult ParseIterationStat()
        {
            if (AcceptKeyword(KeywordType.WHILE))
            {
                if (ExceptOp(OperatorType.LRBRACKET))
                {
                    var exp = ParseExp();
                    if (!exp.IsSuccess)
                        return exp;

                    if (ExceptOp(OperatorType.RRBRACKET))
                    {
                        var stat = ParseStat();
                        if (!stat.IsSuccess)
                            return stat;

                        return new SuccessParseResult(new WhileStat(exp.ResultNode, stat.ResultNode, WhileType.WHILE));
                    }
                }
            }
            
            if (AcceptKeyword(KeywordType.DO))
            {
                var stat = ParseStat();
                if (!stat.IsSuccess)
                    return stat;
                
                if (ExceptKeyword(KeywordType.WHILE))
                {
                    if (ExceptOp(OperatorType.LRBRACKET))
                    {
                        var exp = ParseExp();
                        if (!exp.IsSuccess)
                            return exp;
                        
                        if (ExceptOp(OperatorType.RRBRACKET))
                            if (ExceptOp(OperatorType.SEMICOLON))
                                return new SuccessParseResult(new WhileStat(exp.ResultNode, stat.ResultNode, WhileType.DOWHILE));
                    }
                }
            }

            if (AcceptKeyword(KeywordType.FOR))
            {
                if (ExceptOp(OperatorType.LRBRACKET))
                {
                    var exp1 = ParseExpStat();
                    if (!exp1.IsSuccess)
                        return exp1;

                    var exp2 = ParseExpStat();
                    if (!exp2.IsSuccess)
                        return exp2;

                    if (AcceptOp(OperatorType.RRBRACKET))
                    {
                        var stat = ParseStat();
                        if (!stat.IsSuccess)
                            return stat;

                        return new SuccessParseResult(new ForStat(exp1.ResultNode, exp2.ResultNode, new NullStat(),
                            stat.ResultNode));
                    }
                    
                    var exp3 = ParseExp();
                    if (!exp3.IsSuccess)
                        return exp3;

                    if (ExceptOp(OperatorType.RRBRACKET))
                    {
                        var stat = ParseStat();
                        if (!stat.IsSuccess)
                            return stat;

                        return new SuccessParseResult(new ForStat(exp1.ResultNode, exp2.ResultNode, exp3.ResultNode,
                            stat.ResultNode));
                    }
                }
            }
            
            return new FailedParseResult("expected iteration statement", _currentToken);
        }
        
        /*
         * compound_stat : '{' decl_list stat_list '}'
            | '{'		stat_list '}t	'
                                        | '{' decl_lis	'}'
            | '{'			'}'
            ;
         */

        private IParseResult ParseCompoundStat()
        {
            if (AcceptOp(OperatorType.LFBRACKET))
            {
                var declList = ParseDeclList();
                var statList = ParseStatList();

                if (ExceptOp(OperatorType.RFBRACKET))
                {
                    if (declList.IsSuccess && statList.IsSuccess)
                    {
                        return new SuccessParseResult(new CompoundStat(declList.ResultNode,
                            statList.ResultNode));
                    }

                    if (declList.IsSuccess)
                    {
                        return new SuccessParseResult(new CompoundStat(declList.ResultNode, new NullStat()));
                    }

                    if (statList.IsSuccess)
                    {
                        return new SuccessParseResult(new Pointer(new NullStat(), statList.ResultNode));
                    }
                    
                    return new SuccessParseResult(new NullStat());
                }
            }
            
            return new FailedParseResult("expected compound statement", _currentToken);
        }

        /*
         * decl_list : decl
			| decl_list decl
			;
         */
        
        private IParseResult ParseDeclList()
        {
            return ParseList(ParseDecl, DeclList.Instance);
        }
        
        /*
         * stat_list : stat
			| stat_list stat
			;
         */
        
        private IParseResult ParseStatList()
        {
            return ParseList(ParseStat, StatList.Instance);
        }
        
        
        /*
         * decl	: decl_specs init_declarator_list ';'
            | decl_specs			';'
            ;
         */
        
        private IParseResult ParseDecl()
        {
            var declSpecs = ParseDeclSpecs();
            if (declSpecs.IsSuccess)
            {
                var initDeclaratorList = ParseInitDeclaratorList();
                if (initDeclaratorList.IsSuccess)
                    if (ExceptOp(OperatorType.SEMICOLON))
                        return new SuccessParseResult(new Decl(declSpecs.ResultNode, initDeclaratorList.ResultNode));
                    else
                        if (ExceptOp(OperatorType.SEMICOLON))
                            return new SuccessParseResult(new Decl(declSpecs.ResultNode, new NullStat()));
            }
            
            return new FailedParseResult("expected decl", _currentToken);
        }
        
        /*
         * decl_specs : storage_class_spec decl_specs
            | storage_class_spec
            | type_spec decl_specs
            | type_spec
            | type_qualifier decl_specs
            | type_qualifier
            ;
         */
        
        private IParseResult ParseDeclSpecs()
        {
            var storageClassSpec = ParseStorageClassSpec();
            if (storageClassSpec.IsSuccess)
            {
                var declSpecs = ParseDeclSpecs();
                if (declSpecs.IsSuccess)
                    return new SuccessParseResult(new DeclSpecs(storageClassSpec.ResultNode, declSpecs.ResultNode));
                else
                    return new SuccessParseResult(new DeclSpecs(storageClassSpec.ResultNode, new NullStat()));
            }
            
            var typeSpec = ParseTypeSpec();
            if (typeSpec.IsSuccess)
            {
                var declSpecs = ParseDeclSpecs();
                if (declSpecs.IsSuccess)
                    return new SuccessParseResult(new DeclSpecs(typeSpec.ResultNode, declSpecs.ResultNode));
                else
                    return new SuccessParseResult(new DeclSpecs(typeSpec.ResultNode, new NullStat()));
            }
            
            var typeQualifier = ParseTypeQualifier();
            if (typeQualifier.IsSuccess)
            {
                var declSpecs = ParseDeclSpecs();
                if (declSpecs.IsSuccess)
                    return new SuccessParseResult(new DeclSpecs(typeQualifier.ResultNode, declSpecs.ResultNode));
                else
                    return new SuccessParseResult(new DeclSpecs(typeQualifier.ResultNode, new NullStat()));
            }
            
            return new FailedParseResult("expected decl specs", _currentToken);
        }
        
        /*
         * storage_class_spec : 'auto' | 'register' | 'static' | 'extern' | 'typedef'
			;
         */

        public IParseResult ParseStorageClassSpec()
        {
            if (AcceptKeyword(KeywordType.AUTO) || AcceptKeyword(KeywordType.REGISTER) ||
                AcceptKeyword(KeywordType.STATIC) || AcceptKeyword(KeywordType.EXTERN) ||
                AcceptKeyword(KeywordType.TYPEDEF))
            {
                return new SuccessParseResult(new StorageClassSpec(_acceptedToken as KeywordToken));
            }
            
            return new FailedParseResult("expected storage class spec", _currentToken);
        }
        
        /*
         * type_spec : 'void' | 'char' | 'short' | 'int' | 'long' | 'float'
			| 'double' | 'signed' | 'unsigned'
			| struct_or_union_spec
			| enum_spec
			| typedef_name
			;
         */

        private IParseResult ParseTypeSpec()
        {
            if (AcceptKeyword(KeywordType.VOID) || AcceptKeyword(KeywordType.CHAR)  || AcceptKeyword(KeywordType.UNSIGNED) ||
                AcceptKeyword(KeywordType.SHORT) || AcceptKeyword(KeywordType.INT) || AcceptKeyword(KeywordType.LONG) ||
                AcceptKeyword(KeywordType.FLOAT) || AcceptKeyword(KeywordType.DOUBLE) || AcceptKeyword(KeywordType.SIGNED))
            {
                return new SuccessParseResult(new TypeSpec(_acceptedToken as KeywordToken));
            }
            
            // TODO struct_or_union_spec
            // TODO enum_spec
            // TODO typedef_name
            
            return new FailedParseResult("expected type spec", _currentToken);
        }
        
        /*
         * type_qualifier : 'const' | 'volatile'
            ;
         */

        private IParseResult ParseTypeQualifier()
        {
            if (AcceptKeyword(KeywordType.CONST) || AcceptKeyword(KeywordType.VOLATILE))
            {
                return new SuccessParseResult(new TypeQualifier(_acceptedToken as KeywordToken));
            }
            
            return new FailedParseResult("expected type qualifier", _currentToken);
        }

        /*
         * init_declarator_list	: init_declarator
            | init_declarator_list ',' init_declarator
            ;
         */
        
        private IParseResult ParseInitDeclaratorList()
        {
            return ParseList(ParseInitDeclarator, InitDeclaratorList.Instance, OperatorType.COMMA);
        }
        
        /*
         * init_declarator : declarator
            | declarator '=' initializer
            ;
         */
        
        private IParseResult ParseInitDeclarator()
        {
            var declarator = ParseDeclarator();
            if (!declarator.IsSuccess)
                return declarator;

            if (AcceptOp(OperatorType.ASSIGN))
            {
                var initializer = ParseInitializer();
                if (!initializer.IsSuccess)
                    return initializer;

                return new SuccessParseResult(new InitDeclarator(declarator.ResultNode, initializer.ResultNode));
            }

            return declarator;
        }
        
        /*
         * declarator : pointer direct_declarator
            |	direct_declarator
            ;
         */
        
        private IParseResult ParseDeclarator()
        {
            var pointer = ParsePointer();
            var directDeclarator = ParseDirectDeclarator();

            if (directDeclarator.IsSuccess)
            {
                if (pointer.IsSuccess)
                    return new SuccessParseResult(new Declarator(pointer.ResultNode, directDeclarator.ResultNode));
                else
                    return new SuccessParseResult(new Declarator(new NullStat(), directDeclarator.ResultNode));
            }
            
            return new FailedParseResult("expected declarator", _currentToken);
        }
        
        /*
         * pointer : '*' type_qualifier_list
            | '*'
            | '*' type_qualifier_list pointer
            | '*'			pointer
            ;
         */
        
        private IParseResult ParsePointer()
        {
            if (!AcceptOp(OperatorType.MULT))
                return new FailedParseResult("expected pointer", _currentToken);

            var typeQualifierList = ParseTypeQualifierList();
            var pointer = ParsePointer();
            
            if (pointer.IsSuccess && typeQualifierList.IsSuccess)
            {
                return new SuccessParseResult(new Pointer(pointer.ResultNode,
                    typeQualifierList.ResultNode));
            }

            if (pointer.IsSuccess)
            {
                return new SuccessParseResult(new Pointer(pointer.ResultNode, new NullStat()));
            }

            if (typeQualifierList.IsSuccess)
            {
                return new SuccessParseResult(new Pointer(new NullStat(),
                    typeQualifierList.ResultNode));
            }
            
            return new SuccessParseResult(new Pointer(new NullStat(), new NullStat()));
        }
        
        /*
         * type_qualifier_list : type_qualifier
            | type_qualifier_list type_qualifier
            ;
         */

        private IParseResult ParseTypeQualifierList()
        {
            return ParseList(ParseTypeQualifier, TypeQualifierList.Instance);
        }
        
        /*
         * direct_declarator : id
            | '(' declarator ')'
            | direct_declarator '[' conditional_exp ']'
            | direct_declarator '['		']'
            | direct_declarator '(' param_list ')'
            | direct_declarator '(' id_list ')'
            | direct_declarator '('		')'
            ;
         */
        
        private IParseResult ParseDirectDeclarator()
        {
            IParseResult left = null;
            
            var primaryExp = ParsePrimaryExp();
            if (primaryExp.IsSuccess && ((PrimaryExp) primaryExp.ResultNode).Token.TokenType == TokenType.IDENTIFIER)
            {
                left = primaryExp;
            }
            else if (AcceptOp(OperatorType.LRBRACKET))
            {
                var declarator = ParseDeclarator();
                if (!declarator.IsSuccess)
                    return declarator;
                if (ExceptOp(OperatorType.RRBRACKET))
                {
                    left = declarator;
                }
            }

            if (left == null)
            {
                return new FailedParseResult("expected parse direct declarator", _currentToken);
            }
            
            while (AcceptOp(OperatorType.LSBRACKET) || AcceptOp(OperatorType.LRBRACKET))
            {
                var op = _acceptedToken as OperatorToken;
                
                if (op.Type == OperatorType.LSBRACKET)
                {
                    if (AcceptOp(OperatorType.RSBRACKET))
                    {
                        left = new SuccessParseResult(new DirectDeclarator(left.ResultNode, op,
                            new NullStat()));
                        continue;
                    }

                    var conditionalExp = ParseConditionalExp();
                    if (!conditionalExp.IsSuccess)
                        return conditionalExp;

                    if (ExceptOp(OperatorType.RSBRACKET))
                    {
                        left = new SuccessParseResult(new DirectDeclarator(left.ResultNode, op,
                            conditionalExp.ResultNode));
                        continue;
                    }
                }
                
                if (op.Type == OperatorType.LRBRACKET)
                {
                    if (AcceptOp(OperatorType.RSBRACKET))
                    {
                        left = new SuccessParseResult(new DirectDeclarator(left.ResultNode, op,
                            new NullStat()));
                        continue;
                    }

                    var paramList = ParseParamList();
                    if (paramList.IsSuccess)
                    {
                        if (ExceptOp(OperatorType.RSBRACKET))
                        {
                            left = new SuccessParseResult(new DirectDeclarator(left.ResultNode, op,
                                paramList.ResultNode));
                            continue;
                        }
                    }

                    var idList = ParseIdList();
                    if (idList.IsSuccess)
                    {
                        if (ExceptOp(OperatorType.RSBRACKET))
                        {
                            left = new SuccessParseResult(new DirectDeclarator(left.ResultNode, op,
                                idList.ResultNode));
                            continue;
                        }
                    }
                }
                
                break;
            }

            return left;
        }
        
        /*
         * param_list : param_decl
            | param_list ',' param_decl
            ;
         */
        
        private IParseResult ParseParamList()
        {
            return ParseList(ParseParamDecl, ParamList.Instance, OperatorType.COMMA);
        }
        
        /*
         * param_decl : decl_specs declarator
            | decl_specs abstract_declarator
            | decl_specs
            ;
         */
        
        private IParseResult ParseParamDecl()
        {
            var declSpecs = ParseDeclSpecs();
            if (!declSpecs.IsSuccess)
                return declSpecs; // TODO return declSpecs or FailedParseResult?

            var declarator = ParseDeclarator();
            if (declarator.IsSuccess)
                return new SuccessParseResult(new ParamDecl(declSpecs.ResultNode, declarator.ResultNode));

            var abstractDeclarator = ParseAbstractDeclarator();
            if (abstractDeclarator.IsSuccess)
                return new SuccessParseResult(new ParamDecl(declSpecs.ResultNode, abstractDeclarator.ResultNode));
            
            return new SuccessParseResult(new ParamDecl(declSpecs.ResultNode, new NullStat()));
        }
        
        /*
         * abstract_declarator : pointer
            | pointer direct_abstract_declarator
            |	direct_abstract_declarator
            ;
         */
        
        private IParseResult ParseAbstractDeclarator()
        {
            var pointer = ParsePointer();
            var directAbstractDeclarator = ParseDirectAbstractDeclarator();

            if (pointer.IsSuccess && directAbstractDeclarator.IsSuccess)
            {
                return new SuccessParseResult(new AbstractDeclarator(pointer.ResultNode,
                    directAbstractDeclarator.ResultNode));
            }

            if (pointer.IsSuccess)
            {
                return new SuccessParseResult(new AbstractDeclarator(pointer.ResultNode, new NullStat()));
            }

            if (directAbstractDeclarator.IsSuccess)
            {
                return new SuccessParseResult(new AbstractDeclarator(new NullStat(),
                    directAbstractDeclarator.ResultNode));
            }
            
            return new FailedParseResult("expected abstract declarator", _currentToken);
        }
        
        /*
         * direct_abstract_declarator : '(' abstract_declarator ')'
            | direct_abstract_declarator '[' const_exp ']'
            |				'[' const_exp ']'
            | direct_abstract_declarator '['	']'
            |				'['	']'
            | direct_abstract_declarator '(' param_list ')'
            |				'(' param_list ')'
            | direct_abstract_declarator '('		')'
            |				'('		')'
            ;
         */
        
        private IParseResult ParseDirectAbstractDeclarator()
        {
            IParseResult left = null;

            while (true)
            {
                if (AcceptOp(OperatorType.LRBRACKET))
                {
                    var abstractDeclarator = ParseAbstractDeclarator();
                    if (abstractDeclarator.IsSuccess)
                    {
                        if (ExceptOp(OperatorType.RRBRACKET))
                        {
                            left = new SuccessParseResult(new DirectAbstractDeclarator(
                                left == null ? new NullStat() : left.ResultNode,
                                OperatorType.LRBRACKET, abstractDeclarator.ResultNode));
                            continue;
                        }
                    }

                    var paramTypeList = ParseParamList();
                    if (paramTypeList.IsSuccess)
                    {
                        if (ExceptOp(OperatorType.RRBRACKET))
                        {
                            left = new SuccessParseResult(new DirectAbstractDeclarator(
                                left == null ? new NullStat() : left.ResultNode,
                                OperatorType.LRBRACKET, paramTypeList.ResultNode));
                            continue;
                        }
                    }

                    if (ExceptOp(OperatorType.RRBRACKET))
                    {
                        left = new SuccessParseResult(new DirectAbstractDeclarator(
                            left == null ? new NullStat() : left.ResultNode,
                            OperatorType.LRBRACKET, new NullStat()));
                        continue;
                    }
                }

                if (AcceptOp(OperatorType.LSBRACKET))
                {
                    var @const = ParseConst();
                    if (@const.IsSuccess)
                    {
                        if (ExceptOp(OperatorType.RSBRACKET))
                        {
                            left = new SuccessParseResult(new DirectAbstractDeclarator(
                                left == null ? new NullStat() : left.ResultNode,
                                OperatorType.LSBRACKET, @const.ResultNode));
                            continue;
                        }
                    }
                
                    if (ExceptOp(OperatorType.RSBRACKET))
                    {
                        left = new SuccessParseResult(new DirectAbstractDeclarator(
                            left == null ? new NullStat() : left.ResultNode,
                            OperatorType.LSBRACKET, new NullStat()));
                        continue;
                    }
                }
                
                break;
            }

            return left ?? new FailedParseResult("expected direct abstract declarator", _currentToken);
        }
        
        /*
         * initializer : assignment_exp
            | '{' initializer_list '}'
            | '{' initializer_list ',' '}' TODO is not working now.
            ;
         */
        
        private IParseResult ParseInitializer()
        {
            if (AcceptOp(OperatorType.LFBRACKET))
            {
                var initializerList = ParseInitializerList();
                if (!initializerList.IsSuccess)
                    return initializerList;

                if (ExceptOp(OperatorType.RFBRACKET))
                {
                    return new SuccessParseResult(new Initializer(initializerList.ResultNode));
                }
            }

            return ParseAssignmentExp();
        }
        
        /*
         * initializer_list	: initializer
            | initializer_list ',' initializer
            ;
         */
        
        private IParseResult ParseInitializerList()
        {
            return ParseList(ParseInitializer, InitializerList.Instance, OperatorType.COMMA);
        }

        delegate List ListCtor();

        private IParseResult ParseList(Parser parser, ListCtor ctor, OperatorType separator)
        {
            var list = ctor();
            
            do
            {
                var parseResult = parser();
                if (!parseResult.IsSuccess)
                    return parseResult;

                list.Add(parseResult.ResultNode);
            } while (AcceptOp(separator));

            if (list.Nodes.Count == 0)
                return new FailedParseResult("expected list", _currentToken);

            return new SuccessParseResult(list);
        }
        
        private IParseResult ParseList(Parser parser, ListCtor ctor)
        {
            var list = ctor();
            
            do
            {
                var parseResult = parser();
                if (!parseResult.IsSuccess)
                    break;
        
                list.Add(parseResult.ResultNode);
            } while (true);
        
            if (list.Nodes.Count == 0)
                return new FailedParseResult("expected list", _currentToken);
            
            return new SuccessParseResult(list);
        }
        
        /*
         * id_list : id
            | id_list ',' id
         */
        
        private IParseResult ParseIdList()
        {
            return ParseList(ParseId, IdList.Instance, OperatorType.COMMA);
        }

        private IParseResult ParseId()
        {
            if (Accept(TokenType.IDENTIFIER))
                return new SuccessParseResult(new Const(_acceptedToken));
            
            return new FailedParseResult("expected identifier", _currentToken);
        }
    }
}