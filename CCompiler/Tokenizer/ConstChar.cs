using System.Text;

namespace CCompiler.Tokenizer
{
    internal class ConstChar : FSM
    {
        private char _charValue;
        private State _state;
        private readonly StringSymbol _stringSymbol;

        private readonly StringBuilder _value;

        public ConstChar()
        {
            _value = new StringBuilder();
            _state = State.START;
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
            if (GetState() != FSMState.END)
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
                        _stringSymbol.Reset();
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("opening quote is not found");
                        _state = State.ERROR;
                    }

                    break;
                case State.L:
                    if (ch == '\'')
                    {
                        _state = State.QUOTE;
                        _stringSymbol.Reset();
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("after L, opening quote is not found");
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
                            _stringSymbol.Reset();
                            ReadChar(ch);
                            break;
                        case FSMState.ERROR:
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
                        Tokenizer.LastException.AddMessage("closing quote is not found");
                        _state = State.ERROR;
                    }

                    break;
                case State.QUOTE2:
                    _state = State.END;
                    break;
            }
        }

        public override void Reset()
        {
            _value.Clear();
            _state = State.START;
            _stringSymbol.Reset();
        }

        public override Token GetToken()
        {
            return new Token(TokenType.CHAR, _value.ToString(0, _value.Length - 2), _charValue);
        }

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
    }
}