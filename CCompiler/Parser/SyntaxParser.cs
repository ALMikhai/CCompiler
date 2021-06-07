using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public enum SyntaxParserType
    {
        EXP,
        STAT,
        UNIT
    }
    
    public partial class SyntaxParser
    {
        private readonly Tokenizer.Tokenizer _tokenizer;
        private Token _currentToken;

        public SyntaxParser(Tokenizer.Tokenizer tokenizer, SyntaxParserType parserType)
        {
            _tokenizer = tokenizer;

            NextToken();
            IParseResult result;
            switch (parserType)
            {
                case SyntaxParserType.EXP:
                    result = ParseExp();
                    break;
                case SyntaxParserType.STAT:
                    result = ParseStat();
                    break;
                case SyntaxParserType.UNIT:
                    result = ParseTranslationUnit();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parserType), parserType, null);
            }
            
            Console.WriteLine(result.IsSuccess
                ? result.ResultNode.ToString()
                : throw new ParserException(_currentToken, result.ErrorMessage));
        }

        public SyntaxParser(string filePath, SyntaxParserType parserType) : this(new Tokenizer.Tokenizer(filePath), parserType)
        {
        }

        private void NextToken()
        {
            _currentToken = _tokenizer.Get();
        }

        private bool Accept<T>(T type)
            where T : Enum
        {
            Token token = null;
            return Accept(type, ref token);
        }

        private bool Accept<T, T2>(T type, ref T2 result)
            where T : Enum 
            where T2 : Token
        {
            var table = new Dictionary<Type, TokenType>()
            {
                {typeof(OperatorType), TokenType.OPERATOR},
                {typeof(KeywordType), TokenType.KEYWORD}
            };

            if (typeof(T) != typeof(TokenType) && !table.ContainsKey(typeof(T)))
                throw new ArgumentException($"{typeof(T)} not provided for this method");

            var tokenType = typeof(T) != typeof(TokenType) ? table[typeof(T)] : (TokenType)(object)type;

            if (_currentToken.TokenType == tokenType && _currentToken.GetSpecificType().Equals(type))
            {
                result = _currentToken as T2;
                NextToken();
                return true;
            }

            return false;
        }

        private bool Accept<T, T2>(IEnumerable<T> types, ref T2 result)
            where T : Enum
            where T2 : Token
        {
            foreach (var type in types)
            {
                if (Accept(type, ref result))
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool Expect<T>(T type) 
            where T : Enum
        {
            Token token = null;
            if (Accept(type, ref token))
                return true;

            throw new ParserException(_currentToken,
                $"expected {type}, received {(_currentToken as OperatorToken)?.Type}");
        }
        
        private FailedParseResult ExpectedExpressionFailure() => new FailedParseResult("expected expression", _currentToken);
        private FailedParseResult ExpectedStatFailure() => new FailedParseResult("expected statement", _currentToken);
    }
}