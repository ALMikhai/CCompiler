using System;
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

        public IntToken(TokenType tokenType, string source, object value, IntType type) : base(tokenType, source, value)
        {
            Type = type;
        }

        public IntType Type { get; }

        #region Debug

        public override string ToString()
        {
            return base.ToString() + $"\t{Type}";
        }

        #endregion
    }

    public class Int : FSM
    {
        private BitType _bitType;
        private IntToken.IntType _intType;
        private State _state;

        private readonly StringBuilder _value;

        public Int()
        {
            _value = new StringBuilder();
            _state = State.START;
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
                        Tokenizer.LastException.AddMessage("int must start with a number");
                    }

                    break;
                case State.ZERO:
                    if (ch == 'X' || ch == 'x')
                    {
                        _state = State.X;
                    }
                    else if (Utils.IsOct(ch))
                    {
                        _state = State.OCTAL;
                        _bitType = BitType.OCTAL;
                    }
                    else if (ch == '9' || ch == '8')
                    {
                        _state = State.ERROR;
                        Tokenizer.LastException.AddMessage($"octal number cannot contain {ch}");
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
                    if (Utils.IsOct(ch))
                    {
                        _state = State.OCTAL;
                    }
                    else if (ch == '9' || ch == '8')
                    {
                        _state = State.ERROR;
                        Tokenizer.LastException.AddMessage($"octal number cannot contain {ch}");
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
                    if (char.IsDigit(ch))
                        _state = State.DECIMAL;
                    else if (ch == 'L' || ch == 'l')
                        _state = State.L;
                    else if (ch == 'U' || ch == 'u')
                        _state = State.U;
                    else
                        _state = State.END;

                    break;
                case State.SIXTEEN:
                    if (Utils.IsHex(ch))
                        _state = State.SIXTEEN;
                    else if (ch == 'L' || ch == 'l')
                        _state = State.L;
                    else if (ch == 'U' || ch == 'u')
                        _state = State.U;
                    else
                        _state = State.END;

                    break;
                case State.L:
                    _intType = IntToken.IntType.LONG;
                    if (ch == 'U' || ch == 'u')
                        _state = State.UL;
                    else
                        _state = State.END;

                    break;
                case State.U:
                    _intType = IntToken.IntType.UINT;
                    if (ch == 'L' || ch == 'l')
                        _state = State.UL;
                    else
                        _state = State.END;

                    break;
                case State.UL:
                    _intType = IntToken.IntType.ULONG;
                    _state = State.END;
                    break;
                case State.X:
                    if (Utils.IsHex(ch))
                    {
                        _state = State.SIXTEEN;
                        _bitType = BitType.SIXTEEN;
                    }
                    else
                    {
                        _state = State.ERROR;
                        Tokenizer.LastException.AddMessage("after 'x' must be a hexadecimal number");
                    }

                    break;
            }

            if (_state == State.DECIMAL || _state == State.OCTAL || _state == State.SIXTEEN)
                try
                {
                    Convert(_value.ToString(), _intType, _bitType);
                }
                catch (OverflowException)
                {
                    Tokenizer.LastException.AddMessage(
                        "integer literal is too large to be represented in any integer type");
                    _state = State.ERROR;
                }
        }

        public override void Reset()
        {
            _value.Clear();
            _state = State.START;
            _bitType = BitType.DECIMAL;
            _intType = IntToken.IntType.INT;
        }

        public override Token GetToken()
        {
            var source = _value.ToString(0, _value.Length - 1);
            var value = Convert(source, _intType, _bitType);
            var token = new IntToken(TokenType.INT, source, value, _intType);
            return token;
        }

        private static long Convert(string source, IntToken.IntType intType, BitType bitType)
        {
            var number = intType switch
            {
                IntToken.IntType.INT => source,
                IntToken.IntType.LONG => source.Substring(0, source.Length - 1),
                IntToken.IntType.ULONG => source.Substring(0, source.Length - 2),
                IntToken.IntType.UINT => source.Substring(0, source.Length - 1),
                _ => throw new ArgumentOutOfRangeException(nameof(intType), intType, null)
            };

            var res = bitType switch
            {
                BitType.OCTAL => System.Convert.ToInt64(number, 8),
                BitType.DECIMAL => System.Convert.ToInt64(number, 10),
                BitType.SIXTEEN => System.Convert.ToInt64(number, 16),
                _ => throw new ArgumentOutOfRangeException(nameof(bitType), bitType, null)
            };

            return res;
        }

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
    }
}