using System;
using System.Collections.Generic;
using System.Text;

namespace CCompiler.Tokenizer
{
    class Comment : FSM
    {
        private enum State
        {
            START,
            END,
            ERROR,
            S,
            LINE,
            MLINE,
            STAR,
            FINISH
        }

        private State _state;

        public Comment()
        {
            _state = State.START;
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
                    if (ch == '/')
                    {
                        _state = State.S;
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("comment must start with a '//' or '/*'");
                        _state = State.ERROR;
                    }
                    break;
                case State.S:
                    if (ch == '/')
                    {
                        _state = State.LINE;
                    }
                    else if (ch == '*')
                    {
                        _state = State.MLINE;
                    }
                    else
                    {
                        Tokenizer.LastException.AddMessage("after '/' must be a '/' or '*'");
                        _state = State.ERROR;
                    }
                    break;
                case State.LINE:
                    if (ch == '\n')
                    {
                        _state = State.FINISH;
                    }
                    break;
                case State.MLINE:
                    if (ch == '*')
                    {
                        _state = State.STAR;
                    }
                    else if (ch == '\0')
                    {
                        Tokenizer.LastException.AddMessage("unterminated comment");
                        _state = State.ERROR;
                    }
                    break;
                case State.STAR:
                    if (ch == '/')
                    {
                        _state = State.FINISH;
                    }
                    else
                    {
                        _state = State.MLINE;
                    }
                    break;
                case State.FINISH:
                    _state = State.END;
                    break;
            }
        }

        public override void Reset()
        {
            _state = State.START;
        }

        public override Token GetToken()
        {
            return new Token(TokenType.NONE, "", null);
        }
    }
}
