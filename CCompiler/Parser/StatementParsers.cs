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

        private IParseResult ParseStat() // TODO Remake this block.
        {
            var expStat = ParseExpStat();
            if (expStat.IsSuccess)
                return expStat;

            var compoundStat = ParseCompoundStat();
            if (!compoundStat.IsSuccess)
                return compoundStat;
            if (!(compoundStat.ResultNode is NullStat))
                return compoundStat;

            var selectionStat = ParseSelectionStat();
            if (!selectionStat.IsSuccess)
                return selectionStat;
            if (!(selectionStat.ResultNode is NullStat))
                return selectionStat;

            var iterationStat = ParseIterationStat();
            if (!iterationStat.IsSuccess)
                return iterationStat;
            if (!(iterationStat.ResultNode is NullStat))
                return iterationStat;

            var jumpStat = ParseJumpStat();
            if (!jumpStat.IsSuccess)
                return jumpStat;
            if (!(jumpStat.ResultNode is NullStat))
                return jumpStat;

            return new SuccessParseResult(new NullStat());
        }
        
        /*
         *jump_stat	: 'continue' ';'
            | 'break' ';'
            | 'return' exp ';'
            | 'return'	';'
            ;
         */

        private IParseResult ParseJumpStat()
        {
            if (Accept(KeywordType.CONTINUE))
            {
                Expect(OperatorType.SEMICOLON);
                return new SuccessParseResult(new JumpStat(KeywordType.CONTINUE));
            }

            if (Accept(KeywordType.BREAK))
            {
                Expect(OperatorType.SEMICOLON);
                return new SuccessParseResult(new JumpStat(KeywordType.BREAK));
            }
            
            if (Accept(KeywordType.RETURN))
            {
                if (Accept(OperatorType.SEMICOLON))
                    return new SuccessParseResult(new ReturnStat(new NullStat()));

                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;
                
                Expect(OperatorType.SEMICOLON);
                return new SuccessParseResult(new ReturnStat(exp.ResultNode));
            }

            return new SuccessParseResult(new NullStat());
        }
        
        /*
         * selection_stat : 'if' '(' exp ')' stat
            | 'if' '(' exp ')' stat 'else' stat
            | 'switch' '(' exp ')' stat
            ;
         */

        private IParseResult ParseSelectionStat()
        {
            if (Accept(KeywordType.IF))
            {
                Expect(OperatorType.LRBRACKET);
                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;

                Expect(OperatorType.RRBRACKET);
                var stat = ParseStat();
                if (!stat.IsSuccess)
                    return stat;

                if (stat.ResultNode is NullStat)
                    return new FailedParseResult("expected statement", _currentToken);
                    
                if (Accept(KeywordType.ELSE))
                {
                    var stat2 = ParseStat();
                    if (!stat2.IsSuccess)
                        return stat2;

                    if (stat2.ResultNode is NullStat)
                        return new FailedParseResult("expected statement", _currentToken);

                    return new SuccessParseResult(new IfStat(exp.ResultNode, stat.ResultNode,
                        stat2.ResultNode));
                }

                return new SuccessParseResult(new IfStat(exp.ResultNode, stat.ResultNode, new EmptyExp()));
            }

            if (Accept(KeywordType.SWITCH))
            {
                Expect(OperatorType.LRBRACKET);
                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;

                Expect(OperatorType.RRBRACKET);
                var stat = ParseStat();
                if (!stat.IsSuccess)
                    return stat;

                if (stat.ResultNode is NullStat)
                    return new FailedParseResult("expected statement", _currentToken);
                
                return new SuccessParseResult(new SwitchStat(exp.ResultNode, stat.ResultNode));
            }
            
            return new SuccessParseResult(new NullStat());
        }
        
        /*
         * exp_stat	: exp ';'
			|	';'
			;
         */

        private IParseResult ParseExpStat()
        {
            if (Accept(OperatorType.SEMICOLON))
                return new SuccessParseResult(new EmptyExp());

            var exp = ParseExp();
            if (!exp.IsSuccess)
                return exp;

            Expect(OperatorType.SEMICOLON); 
            return exp;
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
            if (Accept(KeywordType.WHILE))
            {
                Expect(OperatorType.LRBRACKET);
                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;

                Expect(OperatorType.RRBRACKET);
                var stat = ParseStat();
                if (!stat.IsSuccess)
                    return stat;
                
                if (stat.ResultNode is NullStat)
                    return new FailedParseResult("expected statement", _currentToken);

                return new SuccessParseResult(new WhileStat(exp.ResultNode, stat.ResultNode, WhileType.WHILE));
            }
            
            if (Accept(KeywordType.DO))
            {
                var stat = ParseStat();
                if (!stat.IsSuccess)
                    return stat;
                
                if (stat.ResultNode is NullStat)
                    return new FailedParseResult("expected statement", _currentToken);

                Expect(KeywordType.WHILE);
                Expect(OperatorType.LRBRACKET);
                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;

                Expect(OperatorType.RRBRACKET);
                Expect(OperatorType.SEMICOLON);
                return new SuccessParseResult(new WhileStat(exp.ResultNode, stat.ResultNode, WhileType.DOWHILE));
            }

            if (Accept(KeywordType.FOR))
            {
                Expect(OperatorType.LRBRACKET);
                var exp1 = ParseExpStat();
                if (!exp1.IsSuccess)
                    return exp1;

                var exp2 = ParseExpStat();
                if (!exp2.IsSuccess)
                    return exp2;

                if (Accept(OperatorType.RRBRACKET))
                {
                    var stat1 = ParseStat();
                    if (!stat1.IsSuccess)
                        return stat1;
                    
                    if (stat1.ResultNode is NullStat)
                        return new FailedParseResult("expected statement", _currentToken);
                    
                    return new SuccessParseResult(new ForStat(exp1.ResultNode, exp2.ResultNode, new EmptyExp(),
                        stat1.ResultNode));
                }
                
                var exp3 = ParseExp();
                if (!exp3.IsSuccess)
                    return exp3;

                Expect(OperatorType.RRBRACKET);
                var stat2 = ParseStat();
                if (!stat2.IsSuccess)
                    return stat2;
                
                if (stat2.ResultNode is NullStat)
                    return new FailedParseResult("expected statement", _currentToken);

                return new SuccessParseResult(new ForStat(exp1.ResultNode, exp2.ResultNode, exp3.ResultNode,
                    stat2.ResultNode));
            }
            
            return new SuccessParseResult(new NullStat());
        }
        
        /*
         * compound_stat : '{' decl_list stat_list '}'
            | '{' stat_list '}t	'
            | '{' decl_lis '}'
            | '{' '}'
            ;
         */

        private IParseResult ParseCompoundStat()
        {
            if (Accept(OperatorType.LFBRACKET))
            {
                var declList = ParseDeclList();
                if (!declList.IsSuccess)
                    return declList;

                var statList = ParseStatList();
                if (!statList.IsSuccess)
                    return statList;

                Expect(OperatorType.RFBRACKET);
                return new SuccessParseResult(new CompoundStat(declList.ResultNode,
                        statList.ResultNode));
            }

            return new SuccessParseResult(new NullStat());
        }

        /*
         * decl_list : decl
			| decl_list decl
			;
         */
        
        private IParseResult ParseDeclList() => ParseList(ParseDecl, DeclList.Instance);
        
        /*
         * stat_list : stat
			| stat_list stat
			;
         */
        
        private IParseResult ParseStatList() => ParseList(ParseStat, StatList.Instance);


        /*
         * decl	: decl_specs init_declarator_list ';'
            | decl_specs ';'
            ;
         */
        
        private IParseResult ParseDecl()
        {
            var declSpecs = ParseDeclSpecs();
            if (!declSpecs.IsNullStat())
            {
                var initDeclaratorList = ParseInitDeclaratorList();
                if (!initDeclaratorList.IsSuccess)
                    return initDeclaratorList;
                
                Expect(OperatorType.SEMICOLON);
                return new SuccessParseResult(new Decl(declSpecs.ResultNode, initDeclaratorList.ResultNode));
            }

            return declSpecs;
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
            if (!storageClassSpec.IsNullStat())
            {
                var declSpecs = ParseDeclSpecs();
                return new SuccessParseResult(new DeclSpecs(storageClassSpec.ResultNode, declSpecs.ResultNode));
            }
            
            var typeSpec = ParseTypeSpec();
            if (!typeSpec.IsNullStat())
            {
                var declSpecs = ParseDeclSpecs();
                return new SuccessParseResult(new DeclSpecs(typeSpec.ResultNode, declSpecs.ResultNode));
            }
            
            var typeQualifier = ParseTypeQualifier();
            if (!typeQualifier.IsNullStat())
            {
                var declSpecs = ParseDeclSpecs();
                return new SuccessParseResult(new DeclSpecs(typeQualifier.ResultNode, declSpecs.ResultNode));
            }
            
            return new SuccessParseResult(new NullStat());
        }
        
        /*
         * storage_class_spec : 'auto' | 'register' | 'static' | 'extern' | 'typedef'
			;
         */

        private IParseResult ParseStorageClassSpec()
        {
            KeywordToken token = null;
            if (Accept(new[] 
            {
                KeywordType.AUTO, KeywordType.REGISTER, KeywordType.STATIC, 
                KeywordType.EXTERN, KeywordType.TYPEDEF
            }, ref token))
            {
                return new SuccessParseResult(new StorageClassSpec(token));
            }
            
            return new SuccessParseResult(new NullStat());
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
            KeywordToken token = null;
            if (Accept(new[]
            {
                KeywordType.VOID, KeywordType.CHAR, KeywordType.UNSIGNED, KeywordType.LONG, KeywordType.INT,
                KeywordType.SIGNED, KeywordType.DOUBLE, KeywordType.SHORT, KeywordType.FLOAT
            }, ref token))
            {
                return new SuccessParseResult(new TypeSpec(token));
            }
            
            // TODO struct_or_union_spec
            // TODO enum_spec
            // TODO typedef_name
            
            return new SuccessParseResult(new NullStat());
        }
        
        /*
         * type_qualifier : 'const' | 'volatile'
            ;
         */

        private IParseResult ParseTypeQualifier()
        {
            KeywordToken token = null;
            if (Accept(new [] {KeywordType.CONST, KeywordType.VOLATILE}, ref token))
                return new SuccessParseResult(new TypeQualifier(token));
            
            return new SuccessParseResult(new NullStat());
        }

        /*
         * init_declarator_list	: init_declarator
            | init_declarator_list ',' init_declarator
            ;
         */
        
        private IParseResult ParseInitDeclaratorList() => 
            ParseList(ParseInitDeclarator, InitDeclaratorList.Instance, OperatorType.COMMA);
        
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

            if (Accept(OperatorType.ASSIGN))
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
            if (!pointer.IsSuccess)
                return pointer;
            
            var directDeclarator = ParseDirectDeclarator();
            if (!directDeclarator.IsSuccess)
                return directDeclarator;

            if (!directDeclarator.IsNullStat())
                return new SuccessParseResult(new Declarator(pointer.ResultNode, directDeclarator.ResultNode));
            
            if (!pointer.IsNullStat())
                return new FailedParseResult("after pointer expected declarator", _currentToken);

            return new SuccessParseResult(new NullStat());
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
            if (!Accept(OperatorType.MULT))
                return new SuccessParseResult(new NullStat());

            var typeQualifierList = ParseTypeQualifierList();
            if (!typeQualifierList.IsSuccess)
                return typeQualifierList;
            
            var pointer = ParsePointer();
            if (!pointer.IsSuccess)
                return pointer;

            return new SuccessParseResult(new Pointer(pointer.ResultNode, typeQualifierList.ResultNode));
        }
        
        /*
         * type_qualifier_list : type_qualifier
            | type_qualifier_list type_qualifier
            ;
         */

        private IParseResult ParseTypeQualifierList() => ParseList(ParseTypeQualifier, TypeQualifierList.Instance);

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
            IParseResult left;

            var id = ParseId();
            if (!id.IsNullStat())
            {
                left = id;
            }
            else if (Accept(OperatorType.LRBRACKET))
            {
                var declarator = ParseDeclarator();
                if (!declarator.IsSuccess)
                    return declarator;
                Expect(OperatorType.RRBRACKET);
                left = declarator;
            }
            else
            {
                return new SuccessParseResult(new NullStat());
            }

            OperatorToken op = null;
            while (Accept(new [] {OperatorType.LSBRACKET, OperatorType.LRBRACKET}, ref op))
            {
                if (op.Type == OperatorType.LSBRACKET)
                {
                    if (Accept(OperatorType.RSBRACKET))
                    {
                        left = new SuccessParseResult(new ArrayDecl(left.ResultNode, new EmptyExp()));
                        continue;
                    }

                    var conditionalExp = ParseConditionalExp();
                    if (!conditionalExp.IsSuccess)
                        return conditionalExp;

                    if (Expect(OperatorType.RSBRACKET))
                    {
                        left = new SuccessParseResult(new ArrayDecl(left.ResultNode, conditionalExp.ResultNode));
                        continue;
                    }
                }
                
                if (op.Type == OperatorType.LRBRACKET)
                {
                    if (Accept(OperatorType.RRBRACKET))
                    {
                        left = new SuccessParseResult(new FuncDecl(left.ResultNode, new EmptyExp()));
                        continue;
                    }

                    var paramList = ParseParamList();
                    if (!paramList.IsSuccess)
                        return paramList;
                    
                    if (!paramList.IsNullStat())
                    {
                        Expect(OperatorType.RRBRACKET);
                        left = new SuccessParseResult(new FuncDecl(left.ResultNode, paramList.ResultNode));
                        continue;
                    }

                    var idList = ParseIdList();
                    if (!idList.IsSuccess)
                        return idList;
                    
                    Expect(OperatorType.RRBRACKET);
                    left = new SuccessParseResult(new FuncDecl(left.ResultNode, idList.ResultNode));
                    continue;
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
        
        private IParseResult ParseParamList() => ParseList(ParseParamDecl, ParamList.Instance, OperatorType.COMMA);

        /*
         * param_decl : decl_specs declarator
            | decl_specs
            ;
         */
        
        private IParseResult ParseParamDecl()
        {
            var declSpecs = ParseDeclSpecs();
            if (!declSpecs.IsSuccess)
                return declSpecs;

            if (declSpecs.IsNullStat())
                return new SuccessParseResult(new NullStat());
            
            var declarator = ParseDeclarator();
            if (!declarator.IsSuccess)
                return declarator;

            if (!declarator.IsNullStat())
                return new SuccessParseResult(new ParamDecl(declSpecs.ResultNode, declarator.ResultNode));

            return new FailedParseResult("after type specialization expected declarator", _currentToken);
        }
        
        /*
         * initializer : assignment_exp
            | '{' initializer_list '}'
            | '{' initializer_list ',' '}'
            ;
         */
        
        private IParseResult ParseInitializer()
        {
            if (Accept(OperatorType.LFBRACKET))
            {
                var initializerList = ParseInitializerList();
                if (!initializerList.IsSuccess)
                    return initializerList;

                Expect(OperatorType.RFBRACKET);
                return new SuccessParseResult(new Initializer(initializerList.ResultNode));
            }

            return ParseAssignmentExp();
        }
        
        /*
         * initializer_list	: initializer
            | initializer_list ',' initializer
            ;
         */
        
        private IParseResult ParseInitializerList() => ParseList(ParseInitializer, InitializerList.Instance, OperatorType.COMMA);

        /*
         * id_list : id
            | id_list ',' id
         */
        
        private IParseResult ParseIdList() => ParseList(ParseId, IdList.Instance, OperatorType.COMMA);

        /*
         * function_definition : decl_specs declarator decl_list compound_stat
            | declarator decl_list compound_stat
            | decl_specs declarator	compound_stat
            | declarator compound_stat
            ;
            
           external_decl : function_definition
            | decl
            ;
         */

        private IParseResult ParseFuncDef()
        {
            var declSpecs = ParseDeclSpecs();
            if (!declSpecs.IsSuccess)
                return declSpecs;

            if (!declSpecs.IsNullStat() && Accept(OperatorType.SEMICOLON))
                return new SuccessParseResult(new Decl(declSpecs.ResultNode, new EmptyExp()));
            
            var initDeclarator = ParseInitDeclarator();
            if (!initDeclarator.IsSuccess)
                return initDeclarator;
            
            if (!initDeclarator.IsNullStat() && Accept(OperatorType.SEMICOLON))
                if (!declSpecs.IsNullStat())
                    return new SuccessParseResult(new Decl(declSpecs.ResultNode, initDeclarator.ResultNode));
                else
                    return new FailedParseResult("the function is declared incorrectly", _currentToken);
            
            var declList = ParseDeclList();
            if (!declList.IsSuccess)
                return declList;

            var compoundStat = ParseCompoundStat();
            if (!compoundStat.IsSuccess)
                return compoundStat;

            if (initDeclarator.ResultNode is Declarator && !initDeclarator.IsNullStat() && !compoundStat.IsNullStat())
                return new SuccessParseResult(new FuncDef(declSpecs.ResultNode, initDeclarator.ResultNode,
                    declList.ResultNode, compoundStat.ResultNode));

            if (initDeclarator.IsNullStat() && declSpecs.IsNullStat() && declList.IsNullStat())
                return new SuccessParseResult(new NullStat());
            
            return new FailedParseResult("the function is declared incorrectly", _currentToken);
        }

        /*
         * translation_unit	: external_decl
            | translation_unit external_decl
            ;
         */

        private IParseResult ParseTranslationUnit() => ParseList(ParseFuncDef, TranslationUnit.Instance);
    }
}