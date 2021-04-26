using System.Collections.Generic;
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

            return new FailedParseResult("failed parse const token", _currentToken);
        }

        /*
         * primary_exp : id +
			| const +
			| string +
			| '(' exp ')'
         */

        private IParseResult ParsePrimaryExp()
        {
            if (Accept(TokenType.IDENTIFIER) || Accept(TokenType.STRING))
                return new SuccessParseResult(new PrimaryExp(_acceptedToken));

            var @const = ParseConst();
            if (@const.IsSuccess) return @const;

            // This part go to CastExp.
            // if (AcceptOp(OperatorType.LRBRACKET))
            // {
            //     var additiveExp = ParseAdditiveExp();
            //     if (!additiveExp.IsSuccess)
            //         return additiveExp;
            //     
            //     ExceptOp(OperatorType.RRBRACKET);
            //     return additiveExp;
            // }

            return new FailedParseResult("failed parse primary exp", _currentToken);
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
            var primaryExp = ParsePrimaryExp();
            if (!primaryExp.IsSuccess)
                return primaryExp;

            if (AcceptOp(OperatorType.LSBRACKET))
            {
                // var exp = ParseExp(); TODO ...
                ExceptOp(OperatorType.RSBRACKET);
                return new SuccessParseResult(new Const(new IntToken(TokenType.INT, "1", 1,
                    IntToken.IntType.INT))); // TODO Remove.
            }

            if (AcceptOp(OperatorType.LRBRACKET))
            {
                if (AcceptOp(OperatorType.RRBRACKET))
                    return new SuccessParseResult(new PostfixExp(primaryExp.ResultNode, _acceptedToken as OperatorToken,
                        (Node) null));

                // var args = ParseArgs(); TODO ...
                ExceptOp(OperatorType.RRBRACKET);
                return new SuccessParseResult(new Const(new IntToken(TokenType.INT, "1", 1,
                    IntToken.IntType.INT))); // TODO Remove.
            }

            if (AcceptOp(OperatorType.DOT) || AcceptOp(OperatorType.RARROW))
            {
                var op = _acceptedToken;
                Except(TokenType.IDENTIFIER);
                return new SuccessParseResult(
                    new PostfixExp(primaryExp.ResultNode, op as OperatorToken, _acceptedToken));
            }

            if (AcceptOp(OperatorType.INC) || AcceptOp(OperatorType.DEC))
                return new SuccessParseResult(new PostfixExp(primaryExp.ResultNode, _acceptedToken as OperatorToken,
                    (Node) null));

            return primaryExp;
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

            return new FailedParseResult("failed parse unary operator", _currentToken);
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

                // TODO Replace AdditiveExp to Exp.
                var additiveExp = ParseAdditiveExp(); // Part from PrimaryExp.
                if (additiveExp.IsSuccess)
                {
                    ExceptOp(OperatorType.RRBRACKET);
                    return additiveExp;
                }

                return typename;
            }

            return unaryExp;
        }

        /*
         * mult_exp	: cast_exp +
			| mult_exp '*' cast_exp +
			| mult_exp '/' cast_exp +
			| mult_exp '%' cast_exp +
         */

        private IParseResult ParseMultExp()
        {
            var left = ParseCastExp();
            if (!left.IsSuccess)
                return left;

            if (AcceptOp(OperatorType.MULT) || AcceptOp(OperatorType.DIV) ||
                AcceptOp(OperatorType.MOD))
            {
                var operation = _acceptedToken;
                var right = ParseMultExp();
                if (!right.IsSuccess)
                    return right;

                return new SuccessParseResult(
                    new MultExp(operation as OperatorToken, left.ResultNode, right.ResultNode));
            }

            return left;
        }

        /*
         * additive_exp	: mult_exp
        	| additive_exp '+' mult_exp
        	| additive_exp '-' mult_exp
         */

        private IParseResult ParseAdditiveExp()
        {
            var left = ParseMultExp();
            if (!left.IsSuccess)
                return left;

            if (AcceptOp(OperatorType.ADD) || AcceptOp(OperatorType.SUB))
            {
                var operation = _acceptedToken;
                var right = ParseAdditiveExp();
                if (!right.IsSuccess)
                    return right;

                return new SuccessParseResult(new AdditiveExp(operation as OperatorToken, left.ResultNode,
                    right.ResultNode));
            }

            return left;
        }
    }
}