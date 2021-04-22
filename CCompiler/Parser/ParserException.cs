using System;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public class ParserException : Exception
    {
        private Token _token;

        public ParserException(Token token, string message) : base(message)
        {
            _token = token;
        }

        public override string ToString()
        {
            return $"{_token.Position}: syntax error: {Message}";
        }
    }
}