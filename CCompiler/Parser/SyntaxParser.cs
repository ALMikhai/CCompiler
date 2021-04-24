using System;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class SyntaxParser
    {
        private readonly Tokenizer.Tokenizer _tokenizer;
        private Token _acceptedToken;
        private Token _currentToken;

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
    }
}