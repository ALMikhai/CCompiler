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
        public Token(TokenType tokenType, string source, object value)
        {
            TokenType = tokenType;
            Source = source;
            Value = value;
        }

        public Position Position { get; private set; }
        public TokenType TokenType { get; }
        public string Source { get; }
        public object Value { get; }

        public Token AddPosition(Position position)
        {
            Position = position;
            return this;
        }

        public override string ToString()
        {
            return $"{Position}\t{TokenType}\t{Source}\t{Value}";
        }
    }
}