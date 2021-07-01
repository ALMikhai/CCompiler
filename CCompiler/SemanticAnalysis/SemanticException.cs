using System;
using CCompiler.Tokenizer;

namespace CCompiler.SemanticAnalysis
{
    public class SemanticException : Exception
    {
        public Position Position { get; }
        
        public SemanticException(string message, Position position) : base(message)
        {
            Position = position;
        }

        public override string ToString()
        {
            return $"{Position}: semantic error: {Message}";
        }
    }
}