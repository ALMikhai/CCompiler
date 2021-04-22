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

        private IParseResult ParseCastExp()
        {
            var unaryExp = ParseUnaryExp();
            if (unaryExp.IsSuccess)
            {
                return unaryExp;
            }

            return new FailedParseResult("failed parse cast exp", _currentToken);
        }

        private IParseResult ParseUnaryExp()
        {
            var postfixExp = ParsePostfixExp();
            if (postfixExp.IsSuccess)
            {
                return postfixExp;
            }

            return new FailedParseResult("failed parse unary exp", _currentToken);
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
            if (primaryExp.IsSuccess)
            {
                return primaryExp;
            }

            return new FailedParseResult("failed parse postfix exp", _currentToken);
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