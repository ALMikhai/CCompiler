using System;
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

        public IParseResult ParseStat()
        {
            var expStat = ParseExpStat();
            if (expStat.IsSuccess)
                return expStat;
            
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

        public IParseResult ParseJumpStat()
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

        public IParseResult ParseSelectionStat()
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

        public IParseResult ParseExpStat()
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

        public IParseResult ParseIterationStat()
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
            | '{'		stat_list '}'
            | '{' decl_list		'}'
            | '{'			'}'
            ;
         */

        public IParseResult ParseCompoundStat()
        {
            throw new NotImplementedException();
        }

        /*
         * decl_list : decl
			| decl_list decl
			;
         */
        
        public IParseResult ParseDeclList()
        {
            throw new NotImplementedException();
        }
        
        /*
         * stat_list : stat
			| stat_list stat
			;
         */
        
        public IParseResult ParseStatList()
        {
            throw new NotImplementedException();
        }
        
        
        /*
         * decl	: decl_specs init_declarator_list ';'
            | decl_specs			';'
            ;
         */
        
        public IParseResult ParseDecl()
        {
            throw new NotImplementedException();
        }
        
        /*
         * decl_specs : storage_class_spec decl_specs
            | storage_class_spec
            | type_spec decl_specs
            | type_spec
            | type_qualifier decl_specs
            | type_qualifier
            ;
            
           storage_class_spec : 'auto' | 'register' | 'static' | 'extern' | 'typedef'
			;
			
		   type_spec : 'void' | 'char' | 'short' | 'int' | 'long' | 'float'
			| 'double' | 'signed' | 'unsigned'
			| struct_or_union_spec
			| enum_spec
			| typedef_name
			;
			
		   type_qualifier : 'const' | 'volatile'
            ;
         */
        
        public IParseResult ParseDeclSpecs()
        {
            throw new NotImplementedException();
        }

        /*
         * init_declarator_list	: init_declarator
            | init_declarator_list ',' init_declarator
            ;
         */
        
        public IParseResult ParseInitDeclaratorList()
        {
            throw new NotImplementedException();
        }
        
        /*
         * init_declarator : declarator
            | declarator '=' initializer
            ;
         */
        
        public IParseResult ParseInitDeclarator()
        {
            throw new NotImplementedException();
        }
        
        /*
         * declarator : pointer direct_declarator
            |	direct_declarator
            ;
         */
        
        public IParseResult ParseDeclarator()
        {
            throw new NotImplementedException();
        }
        
        /*
         * pointer : '*' type_qualifier_list
            | '*'
            | '*' type_qualifier_list pointer
            | '*'			pointer
            ;
         */
        
        public IParseResult ParsePointer()
        {
            throw new NotImplementedException();
        }
        
        /*
         * type_qualifier_list : type_qualifier
            | type_qualifier_list type_qualifier
            ;
         */

        public IParseResult ParseTypeQualifierList()
        {
            throw new NotImplementedException();
        }
        
        /*
         * direct_declarator : id
            | '(' declarator ')'
            | direct_declarator '[' const_exp ']'
            | direct_declarator '['		']'
            | direct_declarator '(' param_type_list ')'
            | direct_declarator '(' id_list ')'
            | direct_declarator '('		')'
            ;
         */
        
        public IParseResult ParseDirectDeclarator()
        {
            throw new NotImplementedException();
        }
        
        /*
         * param_type_list : param_list
            | param_list ',' '...'
            ;
         */
        
        public IParseResult ParseParamTypeList()
        {
            throw new NotImplementedException();
        }
        
        /*
         * param_list : param_decl
            | param_list ',' param_decl
            ;
         */
        
        public IParseResult ParseParamList()
        {
            throw new NotImplementedException();
        }
        
        /*
         * param_decl : decl_specs declarator
            | decl_specs abstract_declarator
            | decl_specs
            ;
         */
        
        public IParseResult ParseParamDecl()
        {
            throw new NotImplementedException();
        }
        
        /*
         * abstract_declarator : pointer
            | pointer direct_abstract_declarator
            |	direct_abstract_declarator
            ;
         */
        
        public IParseResult ParseAbstractDeclarator()
        {
            throw new NotImplementedException();
        }
        
        /*
         * direct_abstract_declarator : '(' abstract_declarator ')'
            | direct_abstract_declarator '[' const_exp ']'
            |				'[' const_exp ']'
            | direct_abstract_declarator '['	']'
            |				'['	']'
            | direct_abstract_declarator '(' param_type_list ')'
            |				'(' param_type_list ')'
            | direct_abstract_declarator '('		')'
            |				'('		')'
            ;
         */
        
        public IParseResult ParseDirectAbstractDeclarator()
        {
            throw new NotImplementedException();
        }
        
        /*
         * initializer : assignment_exp
            | '{' initializer_list '}'
            | '{' initializer_list ',' '}'
            ;
         */
        
        public IParseResult ParseInitializer()
        {
            throw new NotImplementedException();
        }
        
        /*
         * initializer_list	: initializer
            | initializer_list ',' initializer
            ;
         */
        
        public IParseResult ParseInitializerList()
        {
            throw new NotImplementedException();
        }
    }
}