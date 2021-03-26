using System;

namespace CCompiler.Tokenizer
{
    public enum FSMState
    {
        NONE,
        END,
        RUNNING,
        ERROR
    }

    public abstract class FSM
    {
        public abstract FSMState GetState();
        public abstract void ReadChar(Char ch);
        public abstract void Reset(Position tokenPosition);
        public abstract Token GetToken();
        public abstract TokenizerException GetException();
    }
}
