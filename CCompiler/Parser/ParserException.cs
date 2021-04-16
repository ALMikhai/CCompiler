using System;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public class ParserException : Exception
    {
        private Token _token;
        
        public ParserException(Token token)
        {
            _token = token;
        }
        
        public override string ToString()
        {
            return $"{_token.Position}: error: syntax error";
        }
    }
}
