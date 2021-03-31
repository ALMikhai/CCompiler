using System;
using System.Collections.Generic;
using System.Text;

namespace CCompiler.Tokenizer
{
    public class FloatToken : Token
    {
        public enum FloatType
        {
            DOUBLE,
            FLOAT,
            LONG
        }

        public FloatType Type { get; private set; }

        public FloatToken(TokenType tokenType, string source, object value, FloatType type) : base(tokenType, source,
            value)
        {
            Type = type;
        }

        #region Debug

        public override string ToString()
        {
            return base.ToString() + $"\t{Type}";
        }

        #endregion
    }

    class Float : FSM
    {
        private enum State
        {
            START,
            END,
            ERROR,
            D,
            DOT,
            DDOT,
            DOTD,
            DE,
            DES,
            DED,
            L,
            F
        }

        private StringBuilder _value;
        private State _state;
        private FloatToken.FloatType _floatType;

        public Float()
        {
            _value = new StringBuilder();
            _state = State.START;
            _floatType = FloatToken.FloatType.DOUBLE;
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
                    if (char.IsDigit(ch))
                    {
                        _state = State.D;
                    }
                    else if (ch == '.')
                    {
                        _state = State.DOT;
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("float must start with a number or '.'");
                        _state = State.ERROR;
                    }

                    break;
                case State.D:
                    if (char.IsDigit(ch))
                    {
                        _state = State.D;
                    }
                    else if (ch == '.')
                    {
                        _state = State.DDOT;
                    }
                    else if (ch == 'e' || ch == 'E')
                    {
                        _state = State.DE;
                    }
                    else
                    {
                        // If this error happened it is int number and the error will throw from int
                        //Tokenizer.LastException.AddMessage("is not a double");
                        _state = State.ERROR;
                    }

                    break;
                case State.DOT:
                    if (char.IsDigit(ch))
                    {
                        _state = State.DOTD;
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("after . must be a number");
                        _state = State.ERROR;
                    }

                    break;
                case State.DDOT:
                    if (char.IsDigit(ch))
                    {
                        _state = State.DOTD;
                    }
                    else if (ch == 'e' || ch == 'E')
                    {
                        _state = State.DE;
                    }
                    else if (ch == 'l' || ch == 'L')
                    {
                        _state = State.L;
                    }
                    else if (ch == 'f' || ch == 'F')
                    {
                        _state = State.F;
                    }
                    else
                    {
                        _state = State.END;
                    }

                    break;
                case State.DOTD:
                    if (char.IsDigit(ch))
                    {
                        _state = State.DOTD;
                    }
                    else if (ch == 'e' || ch == 'E')
                    {
                        _state = State.DE;
                    }
                    else if (ch == 'l' || ch == 'L')
                    {
                        _state = State.L;
                    }
                    else if (ch == 'f' || ch == 'F')
                    {
                        _state = State.F;
                    }
                    else
                    {
                        _state = State.END;
                    }

                    break;
                case State.DE:
                    if (ch == '+' || ch == '-')
                    {
                        _state = State.DES;
                    }
                    else if (char.IsDigit(ch))
                    {
                        _state = State.DED;
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("after 'e' must be a number or sign");
                        _state = State.ERROR;
                    }

                    break;
                case State.DES:
                    if (char.IsDigit(ch))
                    {
                        _state = State.DED;
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("after sign must be a number");
                        _state = State.ERROR;
                    }

                    break;
                case State.DED:
                    if (char.IsDigit(ch))
                    {
                        _state = State.DED;
                    }
                    else if (ch == 'l' || ch == 'L')
                    {
                        _state = State.L;
                    }
                    else if (ch == 'f' || ch == 'F')
                    {
                        _state = State.F;
                    }
                    else
                    {
                        _state = State.END;
                    }

                    break;
                case State.L:
                    _floatType = FloatToken.FloatType.LONG;
                    _state = State.END;
                    break;
                case State.F:
                    _floatType = FloatToken.FloatType.FLOAT;
                    _state = State.END;
                    break;
            }
        }

        public override void Reset()
        {
            _value.Clear();
            _state = State.START;
            _floatType = FloatToken.FloatType.DOUBLE;
        }

        public override Token GetToken()
        {
            double value;
            switch (_floatType)
            {
                case FloatToken.FloatType.DOUBLE:
                    value = double.Parse(_value.ToString(0, _value.Length - 1).Replace('.', ','));
                    break;
                case FloatToken.FloatType.FLOAT:
                case FloatToken.FloatType.LONG:
                    value = double.Parse(_value.ToString(0, _value.Length - 2).Replace('.', ','));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new FloatToken(TokenType.FLOAT, _value.ToString(0, _value.Length - 1), value, _floatType);
        }
    }
}
