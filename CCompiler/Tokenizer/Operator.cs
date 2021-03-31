using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace CCompiler.Tokenizer
{
    public class OperatorToken : Token
    {
        public enum OperatorType
        {
            LSBRACKET,
            RSBRACKET,
            LRBRACKET,
            RRBRACKET,
            DOT,
            COMMA,
            QUESTION,
            COLON,
            TILDE,
            SUB,
            RARROW,
            DEC,
            SUBASSIGN,
            ADD,
            INC,
            ADDASSIGN,
            BITAND,
            AND,
            ANDASSIGN,
            MULT,
            MULTASSIGN,
            LESS,
            LESSEQ,
            LSHIFT,
            LSHIFTASSIGN,
            MORE,
            MOREEQ,
            RSHIFT,
            RSHIFTASSIGN,
            ASSIGN,
            EQ,
            BITOR,
            OR,
            ORASSIGN,
            NOT,
            NEQ,
            DIV,
            DIVASSIGN,
            MOD,
            MODASSIGN,
            XOR,
            XORASSIGN,
            SEMICOLON,
            LFBRACKET,
            RFBRACKET
        }

        public OperatorType Type { get; private set; }

        public OperatorToken(TokenType tokenType, string source, object value, OperatorType operatorType) : base(
            tokenType, source, value)
        {
            Type = operatorType;
        }

        #region Debug

        //public override string ToString()
        //{
        //    return base.ToString() + $"\t{Type}";
        //}

        #endregion

        public static Dictionary<String, OperatorType> Operators { get; } = new Dictionary<String, OperatorType>
        {
            {"[", OperatorType.LSBRACKET},
            {"]", OperatorType.RSBRACKET},
            {"(", OperatorType.LRBRACKET},
            {")", OperatorType.RRBRACKET},
            {".", OperatorType.DOT},
            {",", OperatorType.COMMA},
            {"?", OperatorType.QUESTION},
            {":", OperatorType.COLON},
            {"~", OperatorType.TILDE},
            {"-", OperatorType.SUB},
            {"->", OperatorType.RARROW},
            {"--", OperatorType.DEC},
            {"-=", OperatorType.SUBASSIGN},
            {"+", OperatorType.ADD},
            {"++", OperatorType.INC},
            {"+=", OperatorType.ADDASSIGN},
            {"&", OperatorType.BITAND},
            {"&&", OperatorType.AND},
            {"&=", OperatorType.ANDASSIGN},
            {"*", OperatorType.MULT},
            {"*=", OperatorType.MULTASSIGN},
            {"<", OperatorType.LESS},
            {"<=", OperatorType.LESSEQ},
            {"<<", OperatorType.LSHIFT},
            {"<<=", OperatorType.LSHIFTASSIGN},
            {">", OperatorType.MORE},
            {">=", OperatorType.MOREEQ},
            {">>", OperatorType.RSHIFT},
            {">>=", OperatorType.RSHIFTASSIGN},
            {"=", OperatorType.ASSIGN},
            {"==", OperatorType.EQ},
            {"|", OperatorType.BITOR},
            {"||", OperatorType.OR},
            {"|=", OperatorType.ORASSIGN},
            {"!", OperatorType.NOT},
            {"!=", OperatorType.NEQ},
            {"/", OperatorType.DIV},
            {"/=", OperatorType.DIVASSIGN},
            {"%", OperatorType.MOD},
            {"%=", OperatorType.MODASSIGN},
            {"^", OperatorType.XOR},
            {"^=", OperatorType.XORASSIGN},
            {";", OperatorType.SEMICOLON},
            {"{", OperatorType.LFBRACKET},
            {"}", OperatorType.RFBRACKET}
        };
    }

    class Operator : FSM
    {
        private enum State
        {
            START,
            END,
            ERROR,
            FINISH,
            SUB,
            ADD,
            AMP,
            MULT,
            LT,
            LTLT,
            GT,
            GTGT,
            EQ,
            OR,
            NOT,
            DIV,
            MOD,
            XOR
        };

        private StringBuilder _value;
        private State _state;

        private static ImmutableHashSet<char> OperatorChars = ImmutableHashSet.Create(
            '[',
            ']',
            '(',
            ')',
            '.',
            ',',
            '?',
            ':',
            '-',
            '>',
            '+',
            '&',
            '*',
            '~',
            '!',
            '/',
            '%',
            '<',
            '=',
            '^',
            '|',
            ';',
            '{',
            '}'
        );

        public Operator()
        {
            _value = new StringBuilder();
            _state = State.START;
        }

        public override FSMState GetState()
        {
            return _state switch
            {
                State.START => FSMState.NONE,
                State.END => FSMState.END,
                State.ERROR => FSMState.ERROR,
                _ => FSMState.RUNNING
            };
        }

        public override void ReadChar(char ch)
        {
            if (GetState() != FSMState.END)
                _value.Append(ch);

            switch (_state)
            {
                case State.START:
                    if (OperatorChars.Contains(ch))
                    {
                        switch (ch)
                        {
                            case '-':
                                _state = State.SUB;
                                break;
                            case '+':
                                _state = State.ADD;
                                break;
                            case '&':
                                _state = State.AMP;
                                break;
                            case '*':
                                _state = State.MULT;
                                break;
                            case '<':
                                _state = State.LT;
                                break;
                            case '>':
                                _state = State.GT;
                                break;
                            case '=':
                                _state = State.EQ;
                                break;
                            case '|':
                                _state = State.OR;
                                break;
                            case '!':
                                _state = State.NOT;
                                break;
                            case '/':
                                _state = State.DIV;
                                break;
                            case '%':
                                _state = State.MOD;
                                break;
                            case '^':
                                _state = State.XOR;
                                break;
                            default:
                                _state = State.FINISH;
                                break;
                        }
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("operator symbol not found");
                        _state = State.ERROR;
                    }
                    break;
                case State.FINISH:
                    _state = State.END;
                    break;
                case State.SUB:
                    if (ch == '>' || ch == '-' || ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.ADD:
                    if (ch == '+' || ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.AMP:
                    if (ch == '&' || ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.MULT:
                    if (ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.LT:
                    if (ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else if (ch == '<')
                    {
                        _state = State.LTLT;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.LTLT:
                    if (ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.GT:
                    if (ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else if (ch == '>')
                    {
                        _state = State.GTGT;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.GTGT:
                    if (ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.EQ:
                    if (ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.OR:
                    if (ch == '=' || ch == '|')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.NOT:
                    if (ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.DIV:
                    if (ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.MOD:
                    if (ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.XOR:
                    if (ch == '=')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
            }
        }

        public override void Reset()
        {
            _value.Clear();
            _state = State.START;
        }

        public override Token GetToken()
        {
            var source = _value.ToString(0, _value.Length - 1);
            return new OperatorToken(TokenType.OPERATOR, source, OperatorToken.Operators[source],
                OperatorToken.Operators[source]);
        }
    }
}
