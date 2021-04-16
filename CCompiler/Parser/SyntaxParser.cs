using System;
using System.Collections.Generic;
using System.Linq;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public struct ParseResult
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public Node ResultNode { get; }

        public ParseResult(bool isSuccess, Node node = null, string errorMessage = "")
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ResultNode = node;
        }
    }
    
    public class SyntaxParser
    {
        private Tokenizer.Tokenizer _tokenizer;
        private Token _currentToken;
        private List<Token> _tokenBuffer;
        
        public SyntaxParser(Tokenizer.Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            _tokenBuffer = new List<Token>();
            NextToken();
            var additiveExp = ParseAdditiveExp();
            Console.WriteLine(additiveExp.IsSuccess ? additiveExp.ResultNode.ToString() : "Error");
        }

        public SyntaxParser(string filePath) : this(new Tokenizer.Tokenizer(filePath)) { }

        private void NextToken()
        {
            _currentToken = _tokenizer.Get();
        }

        private bool AcceptOp(OperatorToken.OperatorType type)
        {
            if (_currentToken is OperatorToken token &&
                token.Type == type)
            {
                _tokenBuffer.Add(_currentToken);
                NextToken();
                return true;
            }
            return false;
        }

        private bool ExceptOp(OperatorToken.OperatorType type)
        {
            if (AcceptOp(type))
                return true;

            throw new ParserException(_currentToken);
        }
        
        private bool Accept(TokenType type)
        {
            if (_currentToken.TokenType == type)
            {
                _tokenBuffer.Add(_currentToken);
                NextToken();
                return true;
            }
            return false;
        }

        private bool Except(TokenType type)
        {
            if (Accept(type))
                return true;

            throw new ParserException(_currentToken);
        }
        
        private ParseResult ParseAdditiveExp()
        {
            var left = ParseMultExp();
            if (!left.IsSuccess)
                return new ParseResult(false);

            if (AcceptOp(OperatorToken.OperatorType.ADD) || AcceptOp(OperatorToken.OperatorType.SUB))
            {
                var operation = _tokenBuffer.Last();
                var right = ParseMultExp();
                if (!right.IsSuccess)
                    return new ParseResult(false);

                return new ParseResult(true, new AdditiveExp(operation, left.ResultNode, right.ResultNode));
            }

            return left;
        }

        private ParseResult ParseMultExp()
        {
            var left = ParseCastExp();
            if (!left.IsSuccess)
                return new ParseResult(false);

            if (AcceptOp(OperatorToken.OperatorType.MULT) || AcceptOp(OperatorToken.OperatorType.DIV) ||
                AcceptOp(OperatorToken.OperatorType.MOD))
            {
                var operation = _tokenBuffer.Last();
                var right = ParseCastExp();
                if (!right.IsSuccess)
                    return new ParseResult(false);

                return new ParseResult(true, new MultExp(operation, left.ResultNode, right.ResultNode));
            }

            return left;
        }

        private ParseResult ParseCastExp()
        {
            var unaryExp = ParseUnaryExp();
            if (unaryExp.IsSuccess)
            {
                return unaryExp;
            }
            
            return new ParseResult(false);
        }

        private ParseResult ParseUnaryExp()
        {
            var postfixExp = ParsePostfixExp();
            if (postfixExp.IsSuccess)
            {
                return postfixExp;
            }
            
            return new ParseResult(false);
        }

        private ParseResult ParseUnaryOperator()
        {
            if (AcceptOp(OperatorToken.OperatorType.AND) ||
                AcceptOp(OperatorToken.OperatorType.MULT) ||
                AcceptOp(OperatorToken.OperatorType.ADD) ||
                AcceptOp(OperatorToken.OperatorType.SUB) ||
                AcceptOp(OperatorToken.OperatorType.TILDE) ||
                AcceptOp(OperatorToken.OperatorType.NOT))
            {
                return new ParseResult(true,
                    new UnaryOperator(_tokenBuffer.Last()));
            }

            return new ParseResult(false);
        }

        private ParseResult ParsePostfixExp()
        {
            var primaryExp = ParsePrimaryExp();
            if (primaryExp.IsSuccess)
            {
                return primaryExp;
            }

            return new ParseResult(false);
        }
        
        private ParseResult ParsePrimaryExp()
        {
            if (Accept(TokenType.IDENTIFIER) || Accept(TokenType.STRING))
            {
                return new ParseResult(true, new PrimaryExp(_tokenBuffer.Last()));
            }

            var @const = ParseConst();
            if (@const.IsSuccess)
            {
                return @const;
            }

            if (AcceptOp(OperatorToken.OperatorType.LRBRACKET))
            {
                // var exp = ParseExp(); TODO ...
                ExceptOp(OperatorToken.OperatorType.RRBRACKET);
                return new ParseResult(true, new Const(new IntToken(TokenType.INT, "1", 1, IntToken.IntType.INT))); // TODO Remove.
            }

            return new ParseResult(false);
        }
        
        private ParseResult ParseConst()
        {
            if (Accept(TokenType.INT) || Accept(TokenType.CHAR) || Accept(TokenType.FLOAT))
            {
                return new ParseResult(true, new Const(_tokenBuffer.Last()));
            }

            return new ParseResult(false);
        }
    }
}
