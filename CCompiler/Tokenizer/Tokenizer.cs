using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;

namespace CCompiler.Tokenizer
{
    class Tokenizer
    {
        public static TokenizerException LastException = new TokenizerException(new Position(1, 1), "");

        private StreamReader _reader;
        private readonly ImmutableList<FSM> _machines;
        private Token _lastToken;
        private string _lastString;
        private int _lastIndex;
        private int _lastStringNumber;

        public Tokenizer(string filePath)
        {
            _reader = new StreamReader(filePath);
            _machines = ImmutableList.Create<FSM>(
                new ConstString(),
                new ConstChar(),
                new Int(),
                new Identifier(),
                new Eof(),
                new Space()
            );
            _lastToken = null;
            _lastString = _reader.ReadLine();
            _lastIndex = 0;
            _lastStringNumber = 0;
        }

        public Token Peek()
        {
            return _lastToken;
        }

        public Token Get()
        {
            if (_lastToken?.TokenType == TokenType.EOF)
            {
                return _lastToken;
            }

            var tokenReceived = false;
            while (!tokenReceived)
            {
                var input = _lastString?.Length == _lastIndex ? '\n' : _lastString?[_lastIndex] ?? '\0';
                _machines.ForEach(fsm => fsm.ReadChar(input));

                if (_machines.FindIndex(fsm => fsm.GetState() == FSMState.RUNNING) == -1)
                {
                    var index = _machines.FindIndex(fsm => fsm.GetState() == FSMState.END);
                    if (index == -1)
                    {
                        var errorIndex = _machines.FindIndex(fsm => fsm.GetState() == FSMState.ERROR);
                        if (errorIndex == -1)
                        {
                            throw new NotImplementedException("WTF");
                        }
                        else
                        {
                            throw LastException;
                        }
                    }

                    var token = _machines[index].GetToken();
                    if (token.TokenType != TokenType.NONE)
                    {
                        _lastToken = token;
                        tokenReceived = true;
                    }

                    _machines.ForEach(fsm => fsm.Reset(new Position(_lastStringNumber + 1, _lastIndex + 1)));
                    --_lastIndex;
                }

                ++_lastIndex;
                if (_lastString?.Length < _lastIndex)
                {
                    _lastString = _reader.ReadLine();
                    ++_lastStringNumber;
                    _lastIndex = 0;
                }
            }

            return _lastToken;
        }
    }
}
