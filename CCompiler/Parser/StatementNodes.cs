using System.Collections.Generic;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public class JumpStat : Node
    {
        public KeywordType JumpType { get; }

        public JumpStat(KeywordType type)
        {
            JumpType = type;
        }
        
        public override NodeType Type => NodeType.JUMP;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{JumpType}" + "\r\n";
        }
    }

    public class ReturnStat : Node
    {
        public Node Exp { get; }

        public ReturnStat(Node exp)
        {
            Exp = exp;
        }
        
        public override NodeType Type => NodeType.RETURN;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"RETURN" + "\r\n" +
                   Exp.ToString(indent + ChildrenPrefix(last), true);
        }
    }
}