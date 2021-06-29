using System;
using CCompiler.Tokenizer;

namespace CCompiler.SemanticAnalysis
{
    public class SemanticException : Exception
    {
        private readonly Token _token;

        public SemanticException(string message, Token token) : base(message)
        {
            _token = token;
        }

        public SemanticException(string message) : this(message,
            new Token(TokenType.EOF, "", "").AddPosition(new Position(0, 0))) {}

        public override string ToString()
        {
            return $"{_token.Position}: semantic error: {Message}";
        }
    }
}