using System.Text;

namespace CCompiler.Tokenizer
{
    internal class ConstString : FSM
    {
        private State _state;
        private readonly StringSymbol _stringSymbol;
        private readonly StringBuilder _stringValue;

        private readonly StringBuilder _value;

        public ConstString()
        {
            _value = new StringBuilder();
            _stringValue = new StringBuilder();
            _state = State.START;
            _stringSymbol = new StringSymbol('\"');
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
            switch (_state)
            {
                case State.START:
                    _value.Append(ch);
                    if (ch == 'L')
                    {
                        _state = State.L;
                    }
                    else if (ch == '\"')
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
                    _value.Append(ch);
                    if (ch == '\"')
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
                    if (ch == '\"' && _stringSymbol.GetState() == FSMState.NONE)
                    {
                        _value.Append(ch);
                        _state = State.QUOTE2;
                        _stringSymbol.Reset();
                        break;
                    }

                    _stringSymbol.ReadChar(ch);
                    switch (_stringSymbol.GetState())
                    {
                        case FSMState.END:
                            _state = State.QUOTE;
                            _value.Append(_stringSymbol.GetSource());
                            _stringValue.Append(_stringSymbol.GetChar());
                            _stringSymbol.Reset();
                            ReadChar(ch);
                            break;
                        case FSMState.ERROR:
                            _state = State.ERROR;
                            break;
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
            _stringValue.Clear();
            _state = State.START;
            _stringSymbol.Reset();
        }

        public override Token GetToken()
        {
            return new Token(TokenType.STRING, _value.ToString(), _stringValue.ToString());
        }

        private enum State
        {
            START,
            END,
            ERROR,
            L,
            QUOTE,
            QUOTE2
        }
    }
}