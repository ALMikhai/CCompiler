using System;
using System.Collections.Generic;
using System.Text;

namespace CCompiler.Tokenizer
{
    class Space : FSM
    {
        private enum State
        {
            START,
            IDL,
            END,
            ERROR
        }

        private State _state;

        public Space()
        {
            _state = State.START;
        }

        public override FSMState GetState()
        {
            return _state switch
            {
                State.START => FSMState.NONE,
                State.ERROR => FSMState.ERROR,
                State.IDL => FSMState.RUNNING,
                _ => FSMState.END
            };
        }

        public override void ReadChar(char ch)
        {
            switch (_state)
            {
                case State.START:
                    if (char.IsWhiteSpace(ch))
                    {
                        _state = State.IDL;
                    }
                    else
                    {
                        _state = State.ERROR;
                        Tokenizer.LastException.AddMessage("space symbol not found");
                    }
                    break;
                case State.IDL:
                    if (char.IsWhiteSpace(ch))
                    {
                        _state = State.IDL;
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
            _state = State.START;
        }

        public override Token GetToken()
        {
            return new Token(TokenType.NONE, "", "");
        }
    }
}
