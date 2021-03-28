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

        public TokenizerException AddMessage(string message)
        {
            _message = message;
            return this;
        }

        public TokenizerException AddPosition(Position position)
        {
            _position = position;
            return this;
        }

        public override string ToString()
        {
            return $"{_position}: error: {_message}";
        }
    }
}
