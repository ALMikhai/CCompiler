using System;
using System.Collections.Generic;
using System.Linq;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public class SyntaxParser
    {
        private Tokenizer.Tokenizer _tokenizer;
        private Token _currentToken;
        private Token _acceptedToken;

        public SyntaxParser(Tokenizer.Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;

            NextToken();
            var additiveExp = ParseAdditiveExp();
            Console.WriteLine(additiveExp.IsSuccess
                ? additiveExp.ResultNode.ToString()
                : throw new ParserException(_currentToken, additiveExp.ErrorMessage));
        }

        public SyntaxParser(string filePath) : this(new Tokenizer.Tokenizer(filePath))
        {
        }

        private void NextToken()
        {
            _currentToken = _tokenizer.Get();
        }

        private bool AcceptOp(OperatorType type)
        {
            if (_currentToken is OperatorToken token &&
                token.Type == type)
            {
                _acceptedToken = _currentToken;
                NextToken();
                return true;
            }

            return false;
        }

        private bool ExceptOp(OperatorType type)
        {
            if (AcceptOp(type))
                return true;

            throw new ParserException(_currentToken,
                $"expected operator {type}, received {(_currentToken as OperatorToken)?.Type}");
        }

        private bool AcceptKeyword(KeywordType type)
        {
            if (_currentToken is KeywordToken token &&
                token.Type == type)
            {
                _acceptedToken = _currentToken;
                NextToken();
                return true;
            }

            return false;
        }

        private bool ExceptKeyword(KeywordType type)
        {
            if (AcceptKeyword(type))
                return true;

            throw new ParserException(_currentToken,
                $"expected keyword {type}, received {(_currentToken as OperatorToken)?.Type}");
        }

        private bool Accept(TokenType type)
        {
            if (_currentToken.TokenType == type)
            {
                _acceptedToken = _currentToken;
                NextToken();
                return true;
            }

            return false;
        }

        private bool Except(TokenType type)
        {
            if (Accept(type))
                return true;

            throw new ParserException(_currentToken, $"expected token {type}, received {_currentToken.TokenType}");
        }

        private IParseResult ParseAdditiveExp()
        {
            var left = ParseMultExp();
            if (!left.IsSuccess)
                return left;

            if (AcceptOp(OperatorType.ADD) || AcceptOp(OperatorType.SUB))
            {
                var operation = _acceptedToken;
                var right = ParseMultExp();
                if (!right.IsSuccess)
                    return right;

                return new SuccessParseResult(new AdditiveExp(operation as OperatorToken, left.ResultNode,
                    right.ResultNode));
            }

            return left;
        }

        private IParseResult ParseMultExp()
        {
            var left = ParseCastExp();
            if (!left.IsSuccess)
                return left;

            if (AcceptOp(OperatorType.MULT) || AcceptOp(OperatorType.DIV) ||
                AcceptOp(OperatorType.MOD))
            {
                var operation = _acceptedToken;
                var right = ParseCastExp();
                if (!right.IsSuccess)
                    return right;

                return new SuccessParseResult(
                    new MultExp(operation as OperatorToken, left.ResultNode, right.ResultNode));
            }

            return left;
        }

        /*
         * cast_exp	: unary_exp +
			| '(' type_name ')' cast_exp +
         */

        private IParseResult ParseCastExp()
        {
            if (AcceptOp(OperatorType.LRBRACKET))
            {
                var typename = ParseTypename();
                if (typename.IsSuccess)
                {
                    ExceptOp(OperatorType.RRBRACKET);
                    var castExp = ParseCastExp();
                    if (castExp.IsSuccess)
                    {
                        return new SuccessParseResult(new CastExp((Typename) typename.ResultNode, castExp.ResultNode));
                    }

                    return castExp;
                }

                return typename;
            }

            var unaryExp = ParseUnaryExp();
            return unaryExp;
        }

        private IParseResult ParseTypename()
        {
            var typenameParts = new List<KeywordType>();
            Typename.TypenameTypes lastTypename = Typename.TypenameTypes.NONE;
            while (Accept(TokenType.KEYWORD))
            {
                typenameParts.Add(((KeywordToken) _acceptedToken).Type);
                foreach (var pair in Typename.Nodes2Type)
                {
                    if (typenameParts.Count == pair.Value.Count)
                    {
                        var isValid = true;
                        for (int i = 0; i < typenameParts.Count; i++)
                        {
                            if (typenameParts[i] != pair.Value[i])
                                isValid = false;
                        }

                        if (isValid)
                            lastTypename = pair.Key;
                    }
                }
            }

            if (lastTypename == Typename.TypenameTypes.NONE)
                return new FailedParseResult("failed parse typename", _currentToken);

            return new SuccessParseResult(new Typename(lastTypename));
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
                var postfixExp = ParsePostfixExp();
                if (postfixExp.IsSuccess)
                {
                    return new SuccessParseResult(new UnaryExp(postfixExp.ResultNode, op as OperatorToken, null));
                }
            }

            var unaryOperator = ParseUnaryOperator();
            if (unaryOperator.IsSuccess)
            {
                var castExp = ParseCastExp();
                if (castExp.IsSuccess)
                {
                    return new SuccessParseResult(new UnaryExp(castExp.ResultNode, null,
                        unaryOperator.ResultNode as UnaryOperator));
                }
            }

            // TODO Sizeof exp parse.

            return ParsePostfixExp();
        }

        private IParseResult ParseUnaryOperator()
        {
            if (AcceptOp(OperatorType.AND) ||
                AcceptOp(OperatorType.MULT) ||
                AcceptOp(OperatorType.ADD) ||
                AcceptOp(OperatorType.SUB) ||
                AcceptOp(OperatorType.TILDE) ||
                AcceptOp(OperatorType.NOT))
            {
                return new SuccessParseResult(new UnaryOperator(_acceptedToken as OperatorToken));
            }

            return new FailedParseResult("failed parse unary operator", _currentToken);
        }

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
                {
                    return new SuccessParseResult(new PostfixExp(primaryExp.ResultNode, _acceptedToken as OperatorToken,
                        (Node) null));
                }

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
            {
                return new SuccessParseResult(new PostfixExp(primaryExp.ResultNode, _acceptedToken as OperatorToken,
                    (Node) null));
            }

            return primaryExp;
        }

        private IParseResult ParsePrimaryExp()
        {
            if (Accept(TokenType.IDENTIFIER) || Accept(TokenType.STRING))
            {
                return new SuccessParseResult(new PrimaryExp(_acceptedToken));
            }

            var @const = ParseConst();
            if (@const.IsSuccess)
            {
                return @const;
            }

            if (AcceptOp(OperatorType.LRBRACKET))
            {
                // var exp = ParseExp(); TODO ...
                ExceptOp(OperatorType.RRBRACKET);
                return new SuccessParseResult(new Const(new IntToken(TokenType.INT, "1", 1,
                    IntToken.IntType.INT))); // TODO Remove.
            }

            return new FailedParseResult("failed parse primary exp", _currentToken);
        }

        private IParseResult ParseConst()
        {
            if (Accept(TokenType.INT) || Accept(TokenType.CHAR) || Accept(TokenType.FLOAT))
            {
                return new SuccessParseResult(new Const(_acceptedToken));
            }

            return new FailedParseResult("failed parse const token", _currentToken);
        }
    }
}