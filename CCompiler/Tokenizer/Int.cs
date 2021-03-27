using System;
using System.Collections.Generic;
using System.Text;

namespace CCompiler.Tokenizer
{
    public class IntToken : Token
    {
        public enum IntType
        {
            INT,
            LONG,
            ULONG,
            UINT
        }

        public IntType Type { get; private set; }

        public IntToken(Position position, TokenType tokenType, string source, object value, IntType type) : base(position, tokenType, source, value)
        {
            Type = type;
        }
    }

    public class Int : FSM
    {
        private enum State
        {
            START,
            END,
            ERROR,
            ZERO,
            OCTAL,
            DECIMAL,
            SIXTEEN,
            X,
            L,
            U,
            UL
        }

        private enum BitType
        {
            OCTAL,
            DECIMAL,
            SIXTEEN
        }

        private StringBuilder _value;
        private State _state;
        private Position _position;
        private TokenizerException _exception;
        private BitType _bitType;
        private IntToken.IntType _intType;

        public Int()
        {
            _value = new StringBuilder();
            _state = State.START;
            _position = new Position(1, 1);
            _bitType = BitType.DECIMAL;
            _intType = IntToken.IntType.INT;
        }

        public override FSMState GetState()
        {
            return _state switch
            {
                State.START => FSMState.NONE,
                State.END => FSMState.END,
                State.ERROR => FSMState.ERROR,
                State.ZERO => FSMState.RUNNING,
                State.OCTAL => FSMState.RUNNING,
                State.DECIMAL => FSMState.RUNNING,
                State.SIXTEEN => FSMState.RUNNING,
                State.L => FSMState.RUNNING,
                State.U => FSMState.RUNNING,
                State.UL => FSMState.RUNNING,
                State.X => FSMState.RUNNING,
            };
        }

        public override void ReadChar(char ch)
        {
            _value.Append(ch);
            switch (_state)
            {
                case State.START:
                    if (ch == '0')
                    {
                        _state = State.ZERO;
                    }
                    else if (char.IsDigit(ch))
                    {
                        _state = State.DECIMAL;
                    }
                    else
                    {
                        _state = State.ERROR;
                        _exception = new TokenizerException(_position, "int must start with a number");
                    }

                    break;
                case State.ZERO:
                    if (ch == 'X' || ch == 'x')
                    {
                        _state = State.X;
                    }
                    else if (IsOct(ch))
                    {
                        _state = State.OCTAL;
                    }
                    else if (ch == '9' || ch == '8')
                    {
                        _state = State.ERROR;
                        _exception = new TokenizerException(_position, "invalid int");
                    }
                    else if (ch == 'L' || ch == 'l')
                    {
                        _state = State.L;
                    }
                    else if (ch == 'U' || ch == 'u')
                    {
                        _state = State.U;
                    }
                    else
                    {
                        _state = State.END;
                    }

                    break;
                case State.OCTAL:
                    _bitType = BitType.OCTAL;
                    if (IsOct(ch))
                    {
                        _state = State.OCTAL;
                    }
                    else if (ch == '9' || ch == '8')
                    {
                        _state = State.ERROR;
                        _exception = new TokenizerException(_position, "invalid int");
                    }
                    else if (ch == 'L' || ch == 'l')
                    {
                        _state = State.L;
                    }
                    else if (ch == 'U' || ch == 'u')
                    {
                        _state = State.U;
                    }
                    else
                    {
                        _state = State.END;
                    }

                    break;
                case State.DECIMAL:
                    _bitType = BitType.DECIMAL;
                    if (char.IsDigit(ch))
                    {
                        _state = State.DECIMAL;
                    }
                    else if (ch == 'L' || ch == 'l')
                    {
                        _state = State.L;
                    }
                    else if (ch == 'U' || ch == 'u')
                    {
                        _state = State.U;
                    }
                    else
                    {
                        _state = State.END;
                    }

                    break;
                case State.SIXTEEN:
                    _bitType = BitType.SIXTEEN;
                    if (IsHex(ch))
                    {
                        _state = State.SIXTEEN;
                    }
                    else if (ch == 'L' || ch == 'l')
                    {
                        _state = State.L;
                    }
                    else if (ch == 'U' || ch == 'u')
                    {
                        _state = State.U;
                    }
                    else
                    {
                        _state = State.END;
                    }

                    break;
                case State.L:
                    _intType = IntToken.IntType.LONG;
                    if (ch == 'U' || ch == 'u')
                    {
                        _state = State.UL;
                    }
                    else
                    {
                        _state = State.END;
                    }

                    break;
                case State.U:
                    _intType = IntToken.IntType.UINT;
                    if (ch == 'L' || ch == 'l')
                    {
                        _state = State.UL;
                    }
                    else
                    {
                        _state = State.END;
                    }

                    break;
                case State.UL:
                    _intType = IntToken.IntType.ULONG;
                    _state = State.END;
                    break;
                case State.X:
                    if (IsHex(ch))
                    {
                        _state = State.SIXTEEN;
                    }
                    else
                    {
                        _state = State.ERROR;
                        _exception = new TokenizerException(_position, "invalid int");
                    }

                    break;
            }
        }

        public override void Reset(Position tokenPosition)
        {
            _value.Clear();
            _state = State.START;
            _position = tokenPosition;
            _exception = null;
            _bitType = BitType.DECIMAL;
            _intType = IntToken.IntType.INT;
        }

        public override Token GetToken()
        {
            var source = _value.Remove(_value.Length - 1, 1).ToString();
            var value = Convert(source);
            var token = new IntToken(_position, TokenType.INT, source, value, _intType);
            return token;
        }

        public override TokenizerException GetException()
        {
            return _exception;
        }

        private bool IsHex(char ch)
        {
            return char.IsDigit(ch) || ch == 'A' || ch == 'a' || ch == 'B' || ch == 'b' || ch == 'C' || ch == 'c' ||
                   ch == 'D' || ch == 'd' || ch == 'E' || ch == 'e' || ch == 'F' || ch == 'f';
        }

        private bool IsOct(char ch)
        {
            return char.IsDigit(ch) && ch != '9' && ch != '8';
        }

        private long Convert(string source)
        {
            long res = 0;
            switch (_bitType)
            {
                case BitType.OCTAL:
                    res = System.Convert.ToInt64(source, 8);
                    break;
                case BitType.DECIMAL:
                    res = System.Convert.ToInt64(source, 10);
                    break;
                case BitType.SIXTEEN:
                    res = System.Convert.ToInt64(source, 16);
                    break;
            }

            return res;
        }
    }
}
