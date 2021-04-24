using System.Text;

namespace CCompiler.Tokenizer
{
    internal class Identifier : FSM
    {
        private State _state;

        private readonly StringBuilder _value;

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
                        _value.Append(ch);
                    else
                        _state = State.END;

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
            var source = _value.ToString();
            if (KeywordToken.Keywords.ContainsKey(source))
            {
                var keywordType = KeywordToken.Keywords[source];
                return new KeywordToken(TokenType.KEYWORD, source, keywordType, keywordType);
            }

            var token = new Token(TokenType.IDENTIFIER, source, source);
            return token;
        }

        private enum State
        {
            START,
            IDL,
            END,
            ERROR
        }
    }
}