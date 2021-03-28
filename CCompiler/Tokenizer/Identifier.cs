using System;
using System.Collections.Generic;
using System.Text;

namespace CCompiler.Tokenizer
{
    class Identifier : FSM
    {
        private enum State
        {
            START,
            IDL,
            END,
            ERROR
        }

        private StringBuilder _value;
        private State _state;

        public Identifier()
        {
            _value = new StringBuilder();
            _state = State.START;
        }

        public override FSMState GetState()
        {
            return _state switch
            {
                State.START => FSMState.NONE,
                State.IDL => FSMState.RUNNING,
                State.ERROR => FSMState.ERROR,
                _ => FSMState.END
            };
        }

        public override void ReadChar(char ch)
        {
            switch (_state)
            {
                case State.START:
                    if (char.IsLetter(ch) || ch == '_')
                    {
                        _value.Append(ch);
                        _state = State.IDL;
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage(
                            "the identifier must start with a letter or '_'");
                        _state = State.ERROR;
                    }

                    break;
                case State.IDL:
                    if (char.IsLetterOrDigit(ch) || ch == '_')
                    {
                        _value.Append(ch);
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
            var token = new Token(TokenType.IDENTIFIER, _value.ToString(), _value.ToString());
            return token;
        }
    }
}
