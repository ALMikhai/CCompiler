using System;
using System.Collections.Generic;
using System.Text;

namespace CCompiler.Tokenizer
{
    class ConstChar : FSM
    {
        private enum State
        {
            START,
            END,
            ERROR,
            L,
            QUOTE,
            CHAR,
            QUOTE2
        }

        private StringBuilder _value;
        private State _state;
        private Position _position;
        private TokenizerException _exception;
        private char _charValue;
        private StringSymbol _stringSymbol;

        public ConstChar()
        {
            _value = new StringBuilder();
            _state = State.START;
            _position = new Position(1, 1);
            _stringSymbol = new StringSymbol('\'');
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
            _value.Append(ch);
            switch (_state)
            {
                case State.START:
                    if (ch == 'L')
                    {
                        _state = State.L;
                    }
                    else if (ch == '\'')
                    {
                        _state = State.QUOTE;
                        _stringSymbol.Reset(_position);
                    }
                    else
                    {
                        _exception = new TokenizerException(_position, "opening quote is not found");
                        _state = State.ERROR;
                    }
                    break;
                case State.L:
                    if (ch == '\'')
                    {
                        _state = State.QUOTE;
                        _stringSymbol.Reset(_position);
                    }
                    else
                    {
                        _exception = new TokenizerException(_position, "after L, opening quote is not found");
                        _state = State.ERROR;
                    }
                    break;
                case State.QUOTE:
                    _stringSymbol.ReadChar(ch);
                    switch (_stringSymbol.GetState())
                    {
                        case FSMState.END:
                            _state = State.CHAR;
                            _charValue = _stringSymbol.GetChar();
                            _stringSymbol.Reset(_position);
                            ReadChar(ch);
                            break;
                        case FSMState.ERROR:
                            _exception = _stringSymbol.GetException();
                            _state = State.ERROR;
                            break;
                    }
                    break;
                case State.CHAR:
                    if (ch == '\'')
                    {
                        _state = State.QUOTE2;
                    }
                    else
                    {
                        _exception = new TokenizerException(_position, "closing quote is not found");
                        _state = State.ERROR;
                    }
                    break;
                case State.QUOTE2:
                    _state = State.END;
                    break;
            }
        }

        public override void Reset(Position tokenPosition)
        {
            _value.Clear();
            _state = State.START;
            _position = tokenPosition;
            _exception = null;
            _stringSymbol.Reset(tokenPosition);
        }

        public override Token GetToken()
        {
            return new Token(_position, TokenType.CHAR, _value.ToString(0, _value.Length - 2), _charValue);
        }

        public override TokenizerException GetException()
        {
            return _exception;
        }
    }
}
