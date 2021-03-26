using System;
using System.Collections.Generic;
using System.Text;

namespace CCompiler.Tokenizer
{
    public class TokenizerException : Exception
    {
        private Position _position;
        private string _message;

        public TokenizerException(Position position, string message)
        {
            _position = position;
            _message = message;
        }

        public override string ToString()
        {
            return $"{_position}: error: {_message}";
        }
    }
}
