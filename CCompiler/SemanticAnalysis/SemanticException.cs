using System;
using CCompiler.Tokenizer;

namespace CCompiler.SemanticAnalysis
{
    public class SemanticException : Exception
    {
        private readonly Token _token;

        public SemanticException(Token token, string message) : base(message)
        {
            _token = token;
        }

        public override string ToString()
        {
            return $"{_token.Position}: semantic error: {Message}";
        }
    }
}