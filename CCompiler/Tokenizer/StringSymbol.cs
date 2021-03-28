using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Text;

namespace CCompiler.Tokenizer
{
    class StringSymbol : FSM
    {
        private enum State
        {
            START,
            END,
            ERROR,
            SPECIAL,
            CHAR,
            O,
            OI,
            OII,
            X,
            XI,
            XII,
        }

        private enum BitType
        {
            CHAR,
            OCTAL,
            SIXTEEN
        }

        private StringBuilder _value;
        private char _quote;
        private State _state;
        private BitType _bitType;

        public StringSymbol(char quote)
        {
            _value = new StringBuilder();
            _state = State.START;
            _bitType = BitType.CHAR;
            _quote = quote;
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
                    if (IsChar(ch))
                    {
                        _state = State.CHAR;
                    }
                    else if (ch == '\\')
                    {
                        _state = State.SPECIAL;
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("char is empty or contain invalid symbol");
                        _state = State.ERROR;
                    }
                    break;
                case State.CHAR:
                    _state = State.END;
                    break;
                case State.SPECIAL:
                    if (IsSpecial(ch))
                    {
                        _state = State.CHAR;
                    }
                    else if (Utils.IsOct(ch))
                    {
                        _state = State.O;
                    }
                    else if (ch == 'x' || ch == 'X')
                    {
                        _state = State.X;
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("invalid escape sequences");
                        _state = State.ERROR;
                    }
                    break;
                case State.O:
                    _bitType = BitType.OCTAL;
                    if (Utils.IsOct(ch))
                    {
                        _state = State.OI;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.OI:
                    if (Utils.IsOct(ch))
                    {
                        _state = State.OII;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.OII:
                    _state = State.END;
                    break;
                case State.X:
                    if (Utils.IsHex(ch))
                    {
                        _state = State.XI;
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("after \\x must be hexadecimal number");
                        _state = State.ERROR;
                    }
                    break;
                case State.XI:
                    _bitType = BitType.SIXTEEN;
                    if (Utils.IsHex(ch))
                    {
                        _state = State.XII;
                    }
                    else
                    {
                        _state = State.END;
                    }
                    break;
                case State.XII:
                    _state = State.END;
                    break;
            }
        }

        public override void Reset()
        {
            _value.Clear();
            _state = State.START;
            _bitType = BitType.CHAR;
        }

        public override Token GetToken()
        {
            return new Token(TokenType.CHAR, GetSource(), GetChar());
        }

        public char GetChar()
        {
            if (_bitType == BitType.SIXTEEN || _bitType == BitType.OCTAL)
            {
                char res;
                switch (_bitType)
                {
                    case BitType.OCTAL:
                        res = (char)Convert.ToInt64(_value.ToString(1, _value.Length - 2), 8);
                        break;
                    case BitType.SIXTEEN:
                        res = (char)Convert.ToInt64(_value.ToString(2, _value.Length - 3), 16);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return res;
            }

            if (_value.Length == 3)
            {
                switch (_value[1])
                {
                    case 'a':
                        return '\a';
                    case 'b':
                        return '\b';
                    case 'f':
                        return '\f';
                    case 'n':
                        return '\n';
                    case 'r':
                        return '\r';
                    case 't':
                        return '\t';
                    case 'v':
                        return '\v';
                    case '\'':
                        return '\'';
                    case '\"':
                        return '\"';
                    case '\\':
                        return '\\';
                    case '?':
                        return '?';
                    default:
                        return _value[1];
                }
            }
            return _value[0];
        }

        public string GetSource()
        {
            return _value.ToString(0, _value.Length - 1);
        }

        private bool IsChar(char ch)
        {
            return ch != _quote && ch != '\\' && ch != '\n';
        }

        private bool IsSpecial(char ch)
        {
            return "abfnrtv\'\"\\?".Contains(ch);
        }
    }
}
