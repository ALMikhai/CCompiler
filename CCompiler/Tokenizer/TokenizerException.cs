using System;

namespace CCompiler.Tokenizer
{
    public class TokenizerException : Exception
    {
        private string _message;
        private Position _position;

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