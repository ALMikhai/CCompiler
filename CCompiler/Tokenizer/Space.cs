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
        private Position _position;
        private TokenizerException _exception;

        public Space()
        {
            _state = State.START;
            _position = new Position(1, 1);
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
                        _exception = new TokenizerException(_position, "space symbol not found");
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

        public override void Reset(Position tokenPosition)
        {
            _state = State.START;
            _position = tokenPosition;
            _exception = null;
        }

        public override Token GetToken()
        {
            return new Token(_position, TokenType.NONE, "", "");
        }

        public override TokenizerException GetException()
        {
            return _exception;
        }
    }
}
