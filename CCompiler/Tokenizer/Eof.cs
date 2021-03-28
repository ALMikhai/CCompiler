﻿using System;
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
        private Position _position;

        public Eof()
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
                        Tokenizer.LastException.Update(_position, "EOF is not found");
                    }

                    break;
            }
        }

        public override void Reset(Position tokenPosition)
        {
            _state = State.START;
            _position = tokenPosition;
        }

        public override Token GetToken()
        {
            return new Token(_position, TokenType.EOF, "", "");
        }
    }
}
