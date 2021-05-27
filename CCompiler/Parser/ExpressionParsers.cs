﻿using System.Collections.Generic;
using System.Linq;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class SyntaxParser
    {
        /*
         * const : int_const +
			| char_const +
			| float_const +
			| enumeration_const
         */

        private IParseResult ParseConst()
        {
            if (Accept(TokenType.INT) || Accept(TokenType.CHAR) || Accept(TokenType.FLOAT))
                return new SuccessParseResult(new Const(_acceptedToken));

            return new FailedParseResult("expected expression", _currentToken);
        }

        /*
         * primary_exp : id +
			| const +
			| string +
			| '(' exp ')' - This part go to CastExp.
         */

        private IParseResult ParsePrimaryExp()
        {
            if (Accept(TokenType.IDENTIFIER) || Accept(TokenType.STRING))
                return new SuccessParseResult(new PrimaryExp(_acceptedToken));

            var @const = ParseConst();
            return @const;
        }

        /*
         * postfix_exp : primary_exp +
        	| postfix_exp '[' exp ']'
        	| postfix_exp '(' argument_exp_list ')'
        	| postfix_exp '(' ')' +
        	| postfix_exp '.' id +
        	| postfix_exp '->' id +
        	| postfix_exp '++' +
        	| postfix_exp '--' +
         */

        private IParseResult ParsePostfixExp()
        {
            var result = ParsePrimaryExp();
            if (!result.IsSuccess)
                return result;

            while (true)
            {
                if (AcceptOp(OperatorType.LSBRACKET))
                {
                    var op = _acceptedToken;
                    var exp = ParseExp();
                    if (!exp.IsSuccess)
                        return exp;
                    ExceptOp(OperatorType.RSBRACKET);
                    result = new SuccessParseResult(new PostfixExp(result.ResultNode, op as OperatorToken,
                        exp.ResultNode));
                    continue;
                }

                if (AcceptOp(OperatorType.LRBRACKET))
                {
                    var op = _acceptedToken;
                    if (AcceptOp(OperatorType.RRBRACKET))
                    {
                        result = new SuccessParseResult(new PostfixExp(result.ResultNode, op as OperatorToken,
                            (Node) null));
                        continue;
                    }

                    var exp = ParseExp(); // TODO Replace Exp to ArgumentExpList.
                    if (!exp.IsSuccess)
                        return exp;
                    ExceptOp(OperatorType.RRBRACKET);
                    result = new SuccessParseResult(new PostfixExp(result.ResultNode, op as OperatorToken,
                        exp.ResultNode));
                    continue;
                }

                if (AcceptOp(OperatorType.DOT) || AcceptOp(OperatorType.RARROW))
                {
                    var op = _acceptedToken;
                    Except(TokenType.IDENTIFIER);
                    result = new SuccessParseResult(
                        new PostfixExp(result.ResultNode, op as OperatorToken, _acceptedToken));
                    continue;
                }

                if (AcceptOp(OperatorType.INC) || AcceptOp(OperatorType.DEC))
                {
                    result = new SuccessParseResult(new PostfixExp(result.ResultNode, _acceptedToken as OperatorToken,
                        (Node) null));
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
            if (AcceptOp(OperatorType.AND) ||
                AcceptOp(OperatorType.MULT) ||
                AcceptOp(OperatorType.ADD) ||
                AcceptOp(OperatorType.SUB) ||
                AcceptOp(OperatorType.TILDE) ||
                AcceptOp(OperatorType.NOT))
                return new SuccessParseResult(new UnaryOperator(_acceptedToken as OperatorToken));

            return new FailedParseResult("expected unary operator", _currentToken);
        }

        /*
         * unary_exp : postfix_exp +
        	| '++' unary_exp +
        	| '--' unary_exp +
        	| unary_operator cast_exp +
        	| 'sizeof' unary_exp
        	| 'sizeof' '(' type_name ')'
         */

        private IParseResult ParseUnaryExp()
        {
            if (AcceptOp(OperatorType.INC) || AcceptOp(OperatorType.DEC))
            {
                var op = _acceptedToken;
                var unaryExp = ParseUnaryExp();
                if (unaryExp.IsSuccess)
                    return new SuccessParseResult(new UnaryExp(unaryExp.ResultNode, op as OperatorToken));

                return unaryExp;
            }

            var unaryOperator = ParseUnaryOperator();
            if (unaryOperator.IsSuccess)
            {
                var castExp = ParseCastExp();
                if (castExp.IsSuccess)
                    return new SuccessParseResult(new UnaryExp(castExp.ResultNode,
                        unaryOperator.ResultNode as UnaryOperator));

                return castExp;
            }

            // TODO Sizeof exp parse.

            return ParsePostfixExp();
        }

        /*
         * type_name : spec_qualifier_list abstract_declarator
        	| spec_qualifier_list +
         */

        private IParseResult ParseTypename()
        {
            var typenameParts = new List<KeywordType>();
            var lastTypename = Typename.TypenameTypes.NONE;
            while (Accept(TokenType.KEYWORD))
            {
                typenameParts.Add(((KeywordToken) _acceptedToken).Type);
                foreach (var pair in Typename.Nodes2Type)
                    if (typenameParts.Count == pair.Value.Count)
                    {
                        var isValid = true;
                        for (var i = 0; i < typenameParts.Count; i++)
                            if (typenameParts[i] != pair.Value[i])
                                isValid = false;

                        if (isValid)
                            lastTypename = pair.Key;
                    }
            }

            if (lastTypename == Typename.TypenameTypes.NONE)
                return new FailedParseResult("failed parse typename", _currentToken);

            return new SuccessParseResult(new Typename(lastTypename));
        }

        /*
         * cast_exp	: unary_exp +
			| '(' type_name ')' cast_exp +
         */

        private IParseResult ParseCastExp()
        {
            var unaryExp = ParseUnaryExp();
            if (unaryExp.IsSuccess)
            {
                return unaryExp;
            }

            if (AcceptOp(OperatorType.LRBRACKET))
            {
                var typename = ParseTypename();
                if (typename.IsSuccess)
                {
                    ExceptOp(OperatorType.RRBRACKET);
                    var castExp = ParseCastExp();
                    if (castExp.IsSuccess)
                        return new SuccessParseResult(new CastExp((Typename) typename.ResultNode, castExp.ResultNode));

                    return castExp;
                }

                // Part from PrimaryExp.
                var exp = ParseExp();
                if (exp.IsSuccess)
                {
                    ExceptOp(OperatorType.RRBRACKET);
                    return exp;
                }

                return typename;
            }

            return unaryExp;
        }

        delegate IParseResult Parser();

        delegate Node ExpCtor(OperatorToken token, Node left, Node right);

        private IParseResult ParseBinaryExp(Parser parser, ExpCtor ctor, List<OperatorType> availableOperators)
        {
            var left = parser();
            if (!left.IsSuccess)
                return left;

            while (availableOperators.FirstOrDefault(AcceptOp) != OperatorType.NONE)
            {
                var operation = _acceptedToken;
                var right = parser();
                if (!right.IsSuccess)
                    return right;

                left = new SuccessParseResult(ctor(operation as OperatorToken, left.ResultNode,
                    right.ResultNode));
            }

            return left;
        }
        
        /*
         * mult_exp	: cast_exp +
			| mult_exp '*' cast_exp +
			| mult_exp '/' cast_exp +
			| mult_exp '%' cast_exp +
         */

        private IParseResult ParseMultExp()
        {
            return ParseBinaryExp(ParseCastExp, MultExp.Instance,
                new List<OperatorType>() {OperatorType.MULT, OperatorType.DIV, OperatorType.MOD});
        }

        /*
         * additive_exp	: mult_exp
        	| additive_exp '+' mult_exp
        	| additive_exp '-' mult_exp
         */

        private IParseResult ParseAdditiveExp()
        {
            return ParseBinaryExp(ParseMultExp, AdditiveExp.Instance,
                new List<OperatorType>() {OperatorType.ADD, OperatorType.SUB});
        }

        /*
         * shift_expression	: additive_exp
            | shift_expression '<<' additive_exp
            | shift_expression '>>' additive_exp
            ;
         */

        private IParseResult ParseShiftExp()
        {
            return ParseBinaryExp(ParseAdditiveExp, ShiftExp.Instance,
                new List<OperatorType>() {OperatorType.LSHIFT, OperatorType.RSHIFT});
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
            return ParseBinaryExp(ParseShiftExp, RelationalExp.Instance,
                new List<OperatorType>()
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
            return ParseBinaryExp(ParseRelationalExp, EqualityExp.Instance,
                new List<OperatorType>() {OperatorType.EQ, OperatorType.NEQ});
        }
        
        /*
         * and_exp : equality_exp
            | and_exp '&' equality_exp
            ;
         */
        
        private IParseResult ParseAndExp()
        {
            return ParseBinaryExp(ParseEqualityExp, AndExp.Instance, new List<OperatorType>() {OperatorType.BITAND});
        }
        
        /*
         * exclusive_or_exp	: and_exp
            | exclusive_or_exp '^' and_exp
            ;
         */
        
        private IParseResult ParseExclusiveOrExp()
        {
            return ParseBinaryExp(ParseAndExp, ExclusiveOrExp.Instance, new List<OperatorType>() {OperatorType.XOR});
        }
        
        /*
         * inclusive_or_exp	: exclusive_or_exp
            | inclusive_or_exp '|' exclusive_or_exp
            ;
         */
        
        private IParseResult ParseInclusiveOrExp()
        {
            return ParseBinaryExp(ParseExclusiveOrExp, InclusiveOrExp.Instance, new List<OperatorType>() {OperatorType.BITOR});
        }
        
        /*
         * logical_and_exp : inclusive_or_exp
            | logical_and_exp '&&' inclusive_or_exp
            ;
         */
        
        private IParseResult ParseLogicalAndExp()
        {
            return ParseBinaryExp(ParseInclusiveOrExp, LogicalAndExp.Instance, new List<OperatorType>() {OperatorType.AND});
        }
        
        /*
         * logical_or_exp : logical_and_exp
            | logical_or_exp '||' logical_and_exp
            ;
         */
        
        private IParseResult ParseLogicalOrExp()
        {
            return ParseBinaryExp(ParseLogicalAndExp, LogicalOrExp.Instance, new List<OperatorType>() {OperatorType.OR});
        }
        
        /*
         * conditional_exp : logical_or_exp
            | logical_or_exp '?' exp ':' conditional_exp
            ;
         */
        
        private IParseResult ParseConditionalExp()
        {
            var left = ParseLogicalOrExp();
            if (!left.IsSuccess)
                return left;

            if (AcceptOp(OperatorType.QUESTION))
            {
                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;

                if (ExceptOp(OperatorType.COLON))
                {
                    var conditionalExp = ParseConditionalExp();
                    if (!conditionalExp.IsSuccess)
                        return conditionalExp;

                    return new SuccessParseResult(new ConditionalExp(left.ResultNode, exp.ResultNode, conditionalExp.ResultNode));
                }
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
            if (!result.IsSuccess)
                return result;

            if (result.ResultNode.Type == NodeType.UNARYEXP || result.ResultNode.Type == NodeType.POSTFIXEXP ||
                result.ResultNode.Type == NodeType.PRIMARYEXP || result.ResultNode.Type == NodeType.CONST)
            {
                var available = new List<OperatorType>()
                {
                    OperatorType.ASSIGN, OperatorType.MULTASSIGN, OperatorType.DIVASSIGN, OperatorType.MODASSIGN,
                    OperatorType.ADDASSIGN, OperatorType.SUBASSIGN, OperatorType.LSHIFTASSIGN,
                    OperatorType.RSHIFTASSIGN, OperatorType.ANDASSIGN, OperatorType.ORASSIGN, OperatorType.XORASSIGN
                };
                
                if (available.FirstOrDefault(AcceptOp) != OperatorType.NONE)
                {
                    var opToken = _acceptedToken;
                    var assignmentExp = ParseAssignmentExp();
                    if (!assignmentExp.IsSuccess)
                        return assignmentExp;

                    return new SuccessParseResult(new AssignmentExp(opToken as OperatorToken, result.ResultNode,
                        assignmentExp.ResultNode));
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
            return ParseBinaryExp(ParseAssignmentExp, Exp.Instance, new List<OperatorType>() {OperatorType.COMMA});
        }
    }
}