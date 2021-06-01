using System;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public interface IParseResult
    {
        public bool IsSuccess { get; }
        public Node ResultNode { get; }
        public string ErrorMessage { get; }
        public bool IsNullStat();
    }

    public class SuccessParseResult : IParseResult
    {
        public SuccessParseResult(Node node)
        {
            ResultNode = node;
        }

        public bool IsSuccess => true;
        public Node ResultNode { get; }

        public string ErrorMessage =>
            throw new NullReferenceException("Attempt to get ErrorMessage in success parse result");

        public bool IsNullStat()
        {
            return ResultNode is NullStat;
        }
    }

    public class FailedParseResult : IParseResult
    {
        public FailedParseResult(string errorMessage, Token token)
        {
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess => false;
        public Node ResultNode => throw new NullReferenceException("Attempt to get Node in failed parse result");
        public string ErrorMessage { get; }
        public bool IsNullStat()
        {
            throw new NullReferenceException("Attempt to check failed parse node");
        }
    }
}