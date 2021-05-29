using System;
using System.Collections.Generic;

namespace CCompiler.Tokenizer
{
    public enum KeywordType
    {
        CASE,
        ENUM,
        REGISTER,
        TYPEDEF,
        CHAR,
        EXTERN,
        RESTRICT,
        RETURN,
        UNION,
        CONST,
        FLOAT,
        AUTO,
        DOUBLE,
        INLINE,
        INT,
        STRUCT,
        BREAK,
        ELSE,
        LONG,
        SWITCH,
        VOLATILE,
        DO,
        IF,
        STATIC,
        WHILE,
        SHORT,
        UNSIGNED,
        CONTINUE,
        FOR,
        SIGNED,
        VOID,
        DEFAULT,
        GOTO,
        SIZEOF
    }

    public class KeywordToken : Token
    {
        public KeywordToken(TokenType tokenType, string source, object value, KeywordType type) : base(tokenType,
            source, value)
        {
            Type = type;
        }

        public KeywordType Type { get; }

        public static Dictionary<string, KeywordType> Keywords { get; } =
            new Dictionary<string, KeywordType>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"case", KeywordType.CASE},
                {"enum", KeywordType.ENUM},
                {"register", KeywordType.REGISTER},
                {"typedef", KeywordType.TYPEDEF},
                {"char", KeywordType.CHAR},
                {"extern", KeywordType.EXTERN},
                {"restrict", KeywordType.RESTRICT},
                {"return", KeywordType.RETURN},
                {"union", KeywordType.UNION},
                {"const", KeywordType.CONST},
                {"float", KeywordType.FLOAT},
                {"auto", KeywordType.AUTO},
                {"double", KeywordType.DOUBLE},
                {"inline", KeywordType.INLINE},
                {"int", KeywordType.INT},
                {"struct", KeywordType.STRUCT},
                {"break", KeywordType.BREAK},
                {"else", KeywordType.ELSE},
                {"long", KeywordType.LONG},
                {"switch", KeywordType.SWITCH},
                {"volatile", KeywordType.VOLATILE},
                {"do", KeywordType.DO},
                {"if", KeywordType.IF},
                {"static", KeywordType.STATIC},
                {"while", KeywordType.WHILE},
                {"short", KeywordType.SHORT},
                {"unsigned", KeywordType.UNSIGNED},
                {"continue", KeywordType.CONTINUE},
                {"for", KeywordType.FOR},
                {"signed", KeywordType.SIGNED},
                {"void", KeywordType.VOID},
                {"default", KeywordType.DEFAULT},
                {"goto", KeywordType.GOTO},
                {"sizeof", KeywordType.SIZEOF}
            };

        #region Debug

        //public override string ToString()
        //{
        //    return base.ToString() + $"\t{Type}";
        //}

        #endregion
    }
}