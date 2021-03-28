using System;
using System.Collections.Generic;
using System.Text;

namespace CCompiler.Tokenizer
{
    class Eof : FSM
    {
        private enum State
        {
            START,
            END,
            ERROR
        }

        private State _state;

        public Eof()
        {
            _state = State.START;
        }

        public override FSMState GetState()
        {
            return _state switch
            {
                State.START => FSMState.NONE,
                State.ERROR => FSMState.ERROR,
                _ => FSMState.END
            };
        }

        public override void ReadChar(char ch)
        {
            switch (_state)
            {
                case State.START:
                    if (ch == '\0')
                    {
                        _state = State.END;
                    }
                    else
                    {
                        _state = State.ERROR;
                        Tokenizer.LastException.AddMessage("EOF is not found");
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
            return new Token(TokenType.EOF, "", "");
        }
    }
}
