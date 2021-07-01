using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class SyntaxParser
    {
        /*
         * const : int_const
			| char_const
			| float_const
			| enumeration_const
         */

        private IParseResult ParseConst()
        {
            Token token = null;
            if (Accept(new [] { TokenType.INT, TokenType.CHAR, TokenType.FLOAT }, ref token))
                return new SuccessParseResult(new Const(token));

            return new SuccessParseResult(new NullStat());
        }

        /*
         *  identifier : id
             ;
         */
        
        private IParseResult ParseId()
        {
            Token token = null;
            if (Accept(TokenType.IDENTIFIER, ref token))
                return new SuccessParseResult(new Id((string)token.Value));
            
            return new SuccessParseResult(new NullStat());
        }
        
        /*
         * primary_exp : identifier
			| const
			| string
			| '(' exp ')'
         */

        private IParseResult ParsePrimaryExp()
        {
            if (Accept(OperatorType.LRBRACKET))
            {
                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;
                if (exp.IsNullStat())
                    return ExpectedExpressionFailure();
                Expect(OperatorType.RRBRACKET);
                return exp;
            }
            
            var id = ParseId();
            if (!id.IsNullStat())
                return id;

            Token token = null;
            if (Accept(TokenType.STRING, ref token))
                return new SuccessParseResult(new String(token.Source));

            var @const = ParseConst();
            return @const;
        }

        /*
         * postfix_exp : primary_exp
        	| postfix_exp '[' exp ']'
        	| postfix_exp '(' argument_exp_list ')'
        	| postfix_exp '(' ')'
        	| postfix_exp '.' id
        	| postfix_exp '->' id
        	| postfix_exp '++'
        	| postfix_exp '--'
         */

        private IParseResult ParsePostfixExp()
        {
            var result = ParsePrimaryExp();
            if (!result.IsSuccess || result.IsNullStat())
                return result;

            while (true)
            {
                if (Accept(OperatorType.LSBRACKET))
                {
                    var exp = ParseExp();
                    if (!exp.IsSuccess)
                        return exp;
                    if (exp.IsNullStat())
                        return ExpectedExpressionFailure();
                    Expect(OperatorType.RSBRACKET);
                    result = new SuccessParseResult(new AccessingArrayElement(result.ResultNode as ExpNode,
                        exp.ResultNode as ExpNode));
                    continue;
                }

                if (Accept(OperatorType.LRBRACKET))
                {
                    if (Accept(OperatorType.RRBRACKET))
                    {
                        result = new SuccessParseResult(new FuncCall(result.ResultNode as ExpNode, new NullStat()));
                        continue;
                    }
                    
                    var exp = ParseList<ExpList>(ParseAssignmentExp, OperatorType.COMMA);
                    if (!exp.IsSuccess)
                        return exp;
                    
                    Expect(OperatorType.RRBRACKET);
                    result = new SuccessParseResult(new FuncCall(result.ResultNode as ExpNode, exp.ResultNode));
                    continue;
                }

                OperatorToken op = null;
                if (Accept(new [] {OperatorType.DOT, OperatorType.RARROW}, ref op))
                {
                    var id = ParseId();
                    if (id.IsNullStat())
                        return new FailedParseResult("expected identifier", _currentToken);

                    result = new SuccessParseResult(
                        new MemberCall(result.ResultNode as ExpNode, id.ResultNode as ExpNode,
                            (op.Type == OperatorType.DOT ? MemberCall.CallType.VALUE : MemberCall.CallType.POINTER)));
                    continue;
                }

                if (Accept(new [] {OperatorType.INC, OperatorType.DEC}, ref op))
                {
                    result = new SuccessParseResult(new PostfixIncDec(result.ResultNode as ExpNode,
                        (op.Type == OperatorType.INC ? PostfixIncDec.OpType.INC : PostfixIncDec.OpType.DEC)));
                    continue;
                }

                break;
            }

            return result;
        }

        /*
         * unary_operator : '&' | '*' | '+' | '-' | '~' | '!'
         */

        private IParseResult ParseUnaryOperator()
        {
            OperatorToken token = null;
            if (Accept(new []
            {
                OperatorType.BITAND, OperatorType.MULT, OperatorType.ADD, 
                OperatorType.SUB, OperatorType.TILDE, OperatorType.NOT
            }, ref token))
                return new SuccessParseResult(new UnaryOperator(token));

            return new SuccessParseResult(new NullStat());
        }

        /*
         * unary_exp : postfix_exp
        	| '++' unary_exp
        	| '--' unary_exp
        	| unary_operator unary_exp
         */

        private IParseResult ParseUnaryExp()
        {
            OperatorToken op = null;
            if (Accept(new [] {OperatorType.INC, OperatorType.DEC}, ref op))
            {
                var postfixUnaryExp = ParseUnaryExp();
                if (!postfixUnaryExp.IsSuccess)
                    return postfixUnaryExp;
                if (postfixUnaryExp.IsNullStat())
                    return ExpectedExpressionFailure();
                
                return new SuccessParseResult(new PrefixIncDec(postfixUnaryExp.ResultNode as ExpNode,
                    (op.Type == OperatorType.INC ? PrefixIncDec.OpType.INC : PrefixIncDec.OpType.DEC)));
            }

            var unaryOperator = ParseUnaryOperator();
            if (!unaryOperator.IsSuccess || unaryOperator.IsNullStat())
                return ParsePostfixExp();
            
            var unaryExp = ParseUnaryExp();
            if (unaryExp.IsSuccess)
                return new SuccessParseResult(new UnaryExp(unaryExp.ResultNode as ExpNode,
                    unaryOperator.ResultNode as UnaryOperator));

            return unaryExp;
        }
        
        /*
         * mult_exp	: cast_exp
			| mult_exp '*' cast_exp
			| mult_exp '/' cast_exp
			| mult_exp '%' cast_exp
         */

        private IParseResult ParseMultExp()
        {
            return ParseBinaryExp<MultExp>(ParseUnaryExp,
                new [] {OperatorType.MULT, OperatorType.DIV, OperatorType.MOD});
        }

        /*
         * additive_exp	: mult_exp
        	| additive_exp '+' mult_exp
        	| additive_exp '-' mult_exp
         */

        private IParseResult ParseAdditiveExp()
        {
            return ParseBinaryExp<AdditiveExp>(ParseMultExp,
                new[] {OperatorType.ADD, OperatorType.SUB});
        }

        /*
         * shift_expression	: additive_exp
            | shift_expression '<<' additive_exp
            | shift_expression '>>' additive_exp
            ;
         */

        private IParseResult ParseShiftExp()
        {
            return ParseBinaryExp<ShiftExp>(ParseAdditiveExp,
                new [] {OperatorType.LSHIFT, OperatorType.RSHIFT});
        }

        /*
         * relational_exp : shift_expression
            | relational_exp '<' shift_expression
            | relational_exp '>' shift_expression
            | relational_exp '<=' shift_expression
            | relational_exp '>=' shift_expression
            ;
         */

        private IParseResult ParseRelationalExp()
        {
            return ParseBinaryExp<RelationalExp>(ParseShiftExp,
                new []
                    {OperatorType.MORE, OperatorType.MOREEQ, OperatorType.LESS, OperatorType.LESSEQ});
        }

        /*
         * equality_exp	: relational_exp
            | equality_exp '==' relational_exp
            | equality_exp '!=' relational_exp
            ;
         */

        private IParseResult ParseEqualityExp()
        {
            return ParseBinaryExp<EqualityExp>(ParseRelationalExp,
                new [] {OperatorType.EQ, OperatorType.NEQ});
        }
        
        /*
         * and_exp : equality_exp
            | and_exp '&' equality_exp
            ;
         */
        
        private IParseResult ParseAndExp()
        {
            return ParseBinaryExp<AndExp>(ParseEqualityExp,new [] {OperatorType.BITAND});
        }
        
        /*
         * exclusive_or_exp	: and_exp
            | exclusive_or_exp '^' and_exp
            ;
         */
        
        private IParseResult ParseExclusiveOrExp()
        {
            return ParseBinaryExp<ExclusiveOrExp>(ParseAndExp, new [] {OperatorType.XOR});
        }
        
        /*
         * inclusive_or_exp	: exclusive_or_exp
            | inclusive_or_exp '|' exclusive_or_exp
            ;
         */
        
        private IParseResult ParseInclusiveOrExp()
        {
            return ParseBinaryExp<InclusiveOrExp>(ParseExclusiveOrExp, new [] {OperatorType.BITOR});
        }
        
        /*
         * logical_and_exp : inclusive_or_exp
            | logical_and_exp '&&' inclusive_or_exp
            ;
         */
        
        private IParseResult ParseLogicalAndExp()
        {
            return ParseBinaryExp<LogicalAndExp>(ParseInclusiveOrExp, new [] {OperatorType.AND});
        }
        
        /*
         * logical_or_exp : logical_and_exp
            | logical_or_exp '||' logical_and_exp
            ;
         */
        
        private IParseResult ParseLogicalOrExp()
        {
            return ParseBinaryExp<LogicalOrExp>(ParseLogicalAndExp, new [] {OperatorType.OR});
        }
        
        /*
         * conditional_exp : logical_or_exp
            | logical_or_exp '?' exp ':' conditional_exp
            ;
         */
        
        private IParseResult ParseConditionalExp()
        {
            var left = ParseLogicalOrExp();
            if (!left.IsSuccess || left.IsNullStat())
                return left;

            if (Accept(OperatorType.QUESTION))
            {
                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;
                if (exp.IsNullStat())
                    return ExpectedExpressionFailure();

                Expect(OperatorType.COLON);
                var conditionalExp = ParseConditionalExp();
                if (!conditionalExp.IsSuccess)
                    return conditionalExp;
                if (conditionalExp.IsNullStat())
                    return ExpectedExpressionFailure();

                return new SuccessParseResult(new ConditionalExp(left.ResultNode as ExpNode, exp.ResultNode as ExpNode,
                    conditionalExp.ResultNode as ExpNode));
            }

            return left;
        }
        
        /*
         * assignment_exp : conditional_exp
            | unary_exp assignment_operator assignment_exp
            ;
            
           assignment_operator	: '=' | '*=' | '/=' | '%=' | '+=' | '-=' | '<<='
            | '>>=' | '&=' | '^=' | '|='
            ;
         */
        
        private IParseResult ParseAssignmentExp()
        {
            var result = ParseConditionalExp();
            if (!result.IsSuccess || result.IsNullStat())
                return result;

            if (result.ResultNode.Type == NodeType.UNARYEXP || result.ResultNode.Type == NodeType.POSTFIXEXP ||
                result.ResultNode.Type == NodeType.PRIMARYEXP || result.ResultNode.Type == NodeType.CONST)
            {
                var available = new []
                {
                    OperatorType.ASSIGN, OperatorType.MULTASSIGN, OperatorType.DIVASSIGN, OperatorType.MODASSIGN,
                    OperatorType.ADDASSIGN, OperatorType.SUBASSIGN, OperatorType.LSHIFTASSIGN,
                    OperatorType.RSHIFTASSIGN, OperatorType.ANDASSIGN, OperatorType.ORASSIGN, OperatorType.XORASSIGN
                };
                OperatorToken op = null;
                
                if (Accept(available, ref op))
                {
                    var assignmentExp = ParseAssignmentExp();
                    if (!assignmentExp.IsSuccess)
                        return assignmentExp;
                    if (assignmentExp.IsNullStat())
                        return ExpectedExpressionFailure();

                    return new SuccessParseResult(new AssignmentExp(op, result.ResultNode as ExpNode,
                        assignmentExp.ResultNode as ExpNode));
                }
            }

            return result;
        }
        
        /*
         * exp : assignment_exp
            | exp ',' assignment_exp
            ;
         */

        private IParseResult ParseExp()
        {
            return ParseBinaryExp<Exp>(ParseAssignmentExp, new [] {OperatorType.COMMA});
        }
    }
}