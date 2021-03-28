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
        public abstract void ReadChar(char ch);
        public abstract void Reset();
        public abstract Token GetToken();
    }
}
