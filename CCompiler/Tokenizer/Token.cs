using System;
using System.Collections.Generic;
using System.Text;

namespace CCompiler.Tokenizer
{
    public struct Position
    {
        public int String { get; set; }
        public int Column { get; set; }

        public Position(int @string, int column)
        {
            String = @string;
            Column = column;
        }

        public override string ToString()
        {
            return $"({String}:{Column})";
        }
    }

    public enum TokenType
    {
        NONE,
        FLOAT,
        INT,
        CHAR,
        STRING,
        IDENTIFIER,
        KEYWORD,
        OPERATOR,
        EOF,
        SEPARATORS
    }

    public class Token
    {
        public Position Position { get; private set; }
        public TokenType Type { get; private set; }
        public string Source { get; private set; }
        public object Value { get; private set; }

        public Token(Position position, TokenType tokenType, string source, object value)
        {
            Position = position;
            Type = tokenType;
            Source = source;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Position}\t{Type}\t{Source}\t{Value}";
        }
    }
}
