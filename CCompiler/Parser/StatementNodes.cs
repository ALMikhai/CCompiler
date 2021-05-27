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

    public class IfStat : Node
    {
        public Node Exp { get; }
        public Node Stat1 { get; }
        public Node Stat2 { get; }

        public IfStat(Node exp, Node stat1, Node stat2)
        {
            Exp = exp;
            Stat1 = stat1;
            Stat2 = stat2;
        }
        
        public override NodeType Type => NodeType.IF;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"IF" + "\r\n" +
                   Exp.ToString(indent + ChildrenPrefix(last), false) +
                   Stat1.ToString(indent + ChildrenPrefix(last), false) +
                   Stat2?.ToString(indent + ChildrenPrefix(last), true);
        }
    }
    
    public class SwitchStat : Node
    {
        public Node Exp { get; }
        public Node Stat { get; }

        public SwitchStat(Node exp, Node stat)
        {
            Exp = exp;
            Stat = stat;
        }
        
        public override NodeType Type => NodeType.SWITCH;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"SWITCH" + "\r\n" +
                   Exp.ToString(indent + ChildrenPrefix(last), false) +
                   Stat.ToString(indent + ChildrenPrefix(last), true);
        }
    }
}