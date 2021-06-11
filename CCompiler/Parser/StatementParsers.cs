using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class SyntaxParser
    {
        /*
         * stat	: exp_stat
            | compound_stat
            | selection_stat
            | iteration_stat
            | jump_stat
            ;
         */

        private IParseResult ParseStat()
        {
            var parsers = new Parser[]
                {ParseExpStat, ParseCompoundStat, ParseSelectionStat, ParseIterationStat, ParseJumpStat};
            foreach (var parser in parsers)
            {
                var stat = parser();
                if (!stat.IsSuccess)
                    return stat;
                if (!stat.IsNullStat())
                    return stat;
            }

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
                if (exp.IsNullStat())
                    return ExpectedExpressionFailure();

                Expect(OperatorType.RRBRACKET);
                var stat = ParseStat();
                if (!stat.IsSuccess)
                    return stat;
                if (stat.IsNullStat())
                    return ExpectedExpressionFailure();
                    
                if (Accept(KeywordType.ELSE))
                {
                    var stat2 = ParseStat();
                    if (!stat2.IsSuccess)
                        return stat2;
                    if (stat2.IsNullStat())
                        return ExpectedStatFailure();

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
                if (exp.IsNullStat())
                    return ExpectedExpressionFailure();

                Expect(OperatorType.RRBRACKET);
                var stat = ParseStat();
                if (!stat.IsSuccess)
                    return stat;
                if (stat.IsNullStat())
                    return ExpectedStatFailure();
                
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
            if (!exp.IsSuccess || exp.IsNullStat())
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
                if (exp.IsNullStat())
                    return ExpectedExpressionFailure();

                Expect(OperatorType.RRBRACKET);
                var stat = ParseStat();
                if (!stat.IsSuccess)
                    return stat;
                if (stat.IsNullStat())
                    return ExpectedStatFailure();

                return new SuccessParseResult(new WhileStat(exp.ResultNode, stat.ResultNode, WhileType.WHILE));
            }
            
            if (Accept(KeywordType.DO))
            {
                var stat = ParseStat();
                if (!stat.IsSuccess)
                    return stat;
                if (stat.IsNullStat())
                    return ExpectedStatFailure();

                Expect(KeywordType.WHILE);
                Expect(OperatorType.LRBRACKET);
                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;
                if (exp.IsNullStat())
                    return ExpectedExpressionFailure();

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
                    if (stat1.IsNullStat())
                        return ExpectedStatFailure();
                    
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
                if (stat2.IsNullStat())
                    return ExpectedStatFailure();

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
        
        private IParseResult ParseDeclList() => ParseList<DeclList>(ParseDecl);
        
        /*
         * stat_list : stat
			| stat_list stat
			;
         */
        
        private IParseResult ParseStatList() => ParseList<StatList>(ParseStat);


        /*
         * decl	: decl_specs init_declarator_list ';'
            | decl_specs ';'
            ;
         */
        
        private IParseResult ParseDecl()
        {
            var declSpecs = ParseDeclSpecs();
            if (!declSpecs.IsSuccess)
                return declSpecs;
            
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
                if (!declSpecs.IsSuccess)
                    return declSpecs;
                return new SuccessParseResult(new DeclSpecs(storageClassSpec.ResultNode, declSpecs.ResultNode));
            }
            
            var typeSpec = ParseTypeSpec();
            if (!typeSpec.IsSuccess)
                return typeSpec;
            
            if (!typeSpec.IsNullStat())
            {
                var declSpecs = ParseDeclSpecs();
                if (!declSpecs.IsSuccess)
                    return declSpecs;
                return new SuccessParseResult(new DeclSpecs(typeSpec.ResultNode, declSpecs.ResultNode));
            }
            
            var typeQualifier = ParseTypeQualifier();
            if (!typeQualifier.IsNullStat())
            {
                var declSpecs = ParseDeclSpecs();
                if (!declSpecs.IsSuccess)
                    return declSpecs;
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

            var structSpec = ParseStructSpec();
            if (!structSpec.IsSuccess || !structSpec.IsNullStat())
                return structSpec;

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
            ParseList<InitDeclaratorList>(ParseInitDeclarator, OperatorType.COMMA);
        
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
            if (declarator.IsNullStat())
                return declarator;

            if (Accept(OperatorType.ASSIGN))
            {
                var initializer = ParseInitializer();
                if (!initializer.IsSuccess)
                    return initializer;
                if (initializer.IsNullStat())
                    return new FailedParseResult("expected initializer", _currentToken);

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

        private IParseResult ParseTypeQualifierList() => ParseList<TypeQualifierList>(ParseTypeQualifier);

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
                if (declarator.IsNullStat())
                    return new FailedParseResult("expected declarator", _currentToken);
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
                    if (conditionalExp.IsNullStat())
                        return ExpectedExpressionFailure();

                    Expect(OperatorType.RSBRACKET);
                    left = new SuccessParseResult(new ArrayDecl(left.ResultNode, conditionalExp.ResultNode));
                    continue;
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
                    if (idList.IsNullStat())
                        return new FailedParseResult("expected parameter list", _currentToken);
                    
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
        
        private IParseResult ParseParamList() => ParseList<ParamList>(ParseParamDecl, OperatorType.COMMA);

        /*
         * param_decl : decl_specs declarator
            ;
         */
        
        private IParseResult ParseParamDecl()
        {
            var declSpecs = ParseDeclSpecs();
            if (!declSpecs.IsSuccess || declSpecs.IsNullStat())
                return declSpecs;
            
            var declarator = ParseDeclarator();
            if (!declarator.IsSuccess)
                return declarator;
            if (declarator.IsNullStat())
                return new FailedParseResult("after type specialization expected declarator", _currentToken);
            
            return new SuccessParseResult(new ParamDecl(declSpecs.ResultNode, declarator.ResultNode));
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
                if (initializerList.IsNullStat())
                    return new FailedParseResult("expected initializer list", _currentToken);

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
        
        private IParseResult ParseInitializerList() => ParseList<InitializerList>(ParseInitializer, OperatorType.COMMA);

        /*
         * id_list : id
            | id_list ',' id
         */
        
        private IParseResult ParseIdList() => ParseList<IdList>(ParseId, OperatorType.COMMA);

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

        private IParseResult ParseExternalDecl()
        {
            var declSpecs = ParseDeclSpecs();
            if (!declSpecs.IsSuccess)
                return declSpecs;

            if (!declSpecs.IsNullStat() && Accept(OperatorType.SEMICOLON))
                return new SuccessParseResult(new Decl(declSpecs.ResultNode, new EmptyExp()));
            
            var initDeclarator = ParseInitDeclarator();
            if (!initDeclarator.IsSuccess)
                return initDeclarator;

            if (!declSpecs.IsNullStat() && Accept(OperatorType.SEMICOLON))
                return new SuccessParseResult(new Decl(declSpecs.ResultNode, initDeclarator.ResultNode));

            var declList = ParseDeclList();
            if (!declList.IsSuccess)
                return declList;

            var compoundStat = ParseCompoundStat();
            if (!compoundStat.IsSuccess)
                return compoundStat;

            if (initDeclarator.ResultNode is Declarator && !compoundStat.IsNullStat())
                return new SuccessParseResult(new FuncDef(declSpecs.ResultNode, initDeclarator.ResultNode,
                    declList.ResultNode, compoundStat.ResultNode));

            if (initDeclarator.IsNullStat() && declSpecs.IsNullStat() && declList.IsNullStat())
                return new SuccessParseResult(new NullStat());
            
            return new FailedParseResult("the external declaration is incorrectly", _currentToken);
        }

        /*
         * translation_unit	: external_decl
            | translation_unit external_decl
            ;
         */

        private IParseResult ParseTranslationUnit() => ParseList<TranslationUnit>(ParseExternalDecl);

        /*
         * struct_spec	: 'struct' identifier '{' struct_decl_list '}'
            | 'struct' '{' struct_decl_list '}'
            | 'struct' identifier
            ;
         */
        
        private IParseResult ParseStructSpec()
        {
            if (Accept(KeywordType.STRUCT))
            {
                var id = ParseId();

                if (Accept(OperatorType.LFBRACKET))
                {
                    var structDeclList = ParseStructDeclList();
                    if (!structDeclList.IsSuccess)
                        return structDeclList;
                    Expect(OperatorType.RFBRACKET);

                    return new SuccessParseResult(new StructSpec(id.ResultNode, structDeclList.ResultNode));
                }

                if (!id.IsNullStat())
                    return new SuccessParseResult(new StructSpec(id.ResultNode, new EmptyExp()));

                return new FailedParseResult("expected struct declaration", _currentToken);
            }

            return new SuccessParseResult(new NullStat());
        }
        
        /*
         * struct_decl_list : struct_decl
            | struct_decl_list struct_decl
            ;
         */

        private IParseResult ParseStructDeclList() => ParseList<StructDeclList>(ParseStructDecl);

        /*
         * struct_decl : spec_qualifier_list struct_declarator_list ';'
            ;
         */

        private IParseResult ParseStructDecl()
        {
            var specQualifierList = ParseSpecQualifierList();
            if (!specQualifierList.IsSuccess)
                return specQualifierList;
            if (!specQualifierList.IsNullStat())
            {
                var structDeclaratorList = ParseStructDeclaratorList();
                if (!structDeclaratorList.IsSuccess)
                    return structDeclaratorList;
                Expect(OperatorType.SEMICOLON);
                return new SuccessParseResult(new StructDecl(specQualifierList.ResultNode,
                    structDeclaratorList.ResultNode));
            }

            return new SuccessParseResult(new NullStat());
        }
        
        /*
         * spec_qualifier_list : type_spec spec_qualifier_list
            | type_spec
            | type_qualifier spec_qualifier_list
            | type_qualifier
            ;
         */

        private IParseResult ParseSpecQualifierList()
        {
            var typeSpec = ParseTypeSpec();
            if (!typeSpec.IsSuccess)
                return typeSpec;
            if (!typeSpec.IsNullStat())
            {
                var specQualifierList = ParseSpecQualifierList();
                return new SuccessParseResult(new DeclSpecs(typeSpec.ResultNode, specQualifierList.ResultNode));
            }
            
            var typeQualifier = ParseTypeQualifier();
            if (!typeQualifier.IsNullStat())
            {
                var specQualifierList = ParseSpecQualifierList();
                return new SuccessParseResult(new DeclSpecs(typeSpec.ResultNode, specQualifierList.ResultNode));   
            }

            return new SuccessParseResult(new NullStat());
        }
        
        /*
         * struct_declarator_list : declarator
            | struct_declarator_list ',' declarator
            ;
         */

        private IParseResult ParseStructDeclaratorList() =>
            ParseList<StructDeclaratorList>(ParseDeclarator, OperatorType.COMMA);
    }
}