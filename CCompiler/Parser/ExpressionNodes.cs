using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class Const : ExpNode
    {
        public Token Token { get; }

        public Const(Token token, Position startNodePosition) : base(startNodePosition)
        {
            Token = token;
        }

        public override NodeType Type => NodeType.CONST;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Value}" + "\r\n";
        }
    }

    public partial class Id : ExpNode
    {
        public string IdName { get; }

        public Id(string idName, Position startNodePosition) : base(startNodePosition)
        {
            IdName = idName;
        }

        public override NodeType Type => NodeType.PRIMARYEXP;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + IdName + "\r\n";
        }
    }

    public partial class String : ExpNode
    {
        public string Str { get; }
        
        public String(string str, Position startNodePosition) : base(startNodePosition)
        {
            Str = str;
        }
        
        public override NodeType Type => NodeType.PRIMARYEXP;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Str + "\r\n";
        }
    }

    public partial class AccessingArrayElement : ExpNode
    {
        public ExpNode PostfixNode { get; }
        public ExpNode Exp { get; }

        public AccessingArrayElement(ExpNode postfixNode, ExpNode exp, Position startNodePosition) : base(startNodePosition)
        {
            PostfixNode = postfixNode;
            Exp = exp;
        }
        
        public override NodeType Type => NodeType.POSTFIXEXP;
        
        public override string ToString(string indent, bool last)
        {
            return
                indent + NodePrefix(last) + $"[]" + "\r\n" +
                PostfixNode.ToString(indent + ChildrenPrefix(last), false) +
                Exp.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class FuncCall : ExpNode
    {
        public ExpNode PostfixNode { get; }
        public Node ExpList { get; }

        public FuncCall(ExpNode postfixNode, Node expList, Position startNodePosition) : base(startNodePosition)
        {
            PostfixNode = postfixNode;
            ExpList = expList;
        }
        
        public override NodeType Type => NodeType.POSTFIXEXP;

        public override string ToString(string indent, bool last)
        {
            return
                indent + NodePrefix(last) + $"()" + "\r\n" +
                PostfixNode.ToString(indent + ChildrenPrefix(last), ExpList is NullStat) +
                ExpList.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class MemberCall : ExpNode
    {
        public enum CallType
        {
            VALUE,
            POINTER
        }
        
        public ExpNode PostfixNode { get; }
        public ExpNode Id { get; }

        private readonly CallType _callType;

        public MemberCall(ExpNode postfixNode, ExpNode id, CallType callType, Position startNodePosition) : base(startNodePosition)
        {
            _callType = callType;
            PostfixNode = postfixNode;
            Id = id;
        }
        
        public override NodeType Type => NodeType.POSTFIXEXP;

        public override string ToString(string indent, bool last)
        {
            return
                indent + NodePrefix(last) + (_callType == CallType.VALUE ? "." : "->") + "\r\n" +
                PostfixNode.ToString(indent + ChildrenPrefix(last), false) +
                Id.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class PostfixIncDec : ExpNode
    {
        private readonly OpType _opType;

        public enum OpType
        {
            INC,
            DEC
        }
        public ExpNode PrefixNode { get; }

        public PostfixIncDec(ExpNode prefixNode, OpType opType, Position startNodePosition) : base(startNodePosition)
        {
            _opType = opType;
            PrefixNode = prefixNode;
        }
        
        public override NodeType Type => NodeType.POSTFIXEXP;

        public override string ToString(string indent, bool last)
        {
            return
                indent + NodePrefix(last) + $"POSTFIX {_opType}" + "\r\n" +
                PrefixNode.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class UnaryOperator : Node
    {
        public UnaryOperator(OperatorToken @operator)
        {
            Operator = @operator;
        }

        public OperatorToken Operator { get; }
        public override NodeType Type => NodeType.UNARYOPERATOR;
    }

    public partial class PrefixIncDec : ExpNode
    {
        private readonly OpType _opType;

        public enum OpType
        {
            INC,
            DEC
        }
        public ExpNode PostfixNode { get; }

        public PrefixIncDec(ExpNode postfixNode, OpType opType, Position startNodePosition) : base(startNodePosition)
        {
            _opType = opType;
            PostfixNode = postfixNode;
        }
        
        public override NodeType Type => NodeType.UNARYEXP;

        public override string ToString(string indent, bool last)
        {
            return
                indent + NodePrefix(last) + $"PREFIX {_opType}" + "\r\n" +
                PostfixNode.ToString(indent + ChildrenPrefix(last), true);
        }
    }
    
    public partial class UnaryExp : ExpNode
    {
        public UnaryExp(ExpNode unaryExpNode, UnaryOperator unaryOperator, Position startNodePosition) : base(startNodePosition)
        {
            UnaryOperator = unaryOperator;
            UnaryExpNode = unaryExpNode;
        }

        public ExpNode UnaryExpNode { get; }
        public UnaryOperator UnaryOperator { get; }

        public override NodeType Type => NodeType.UNARYEXP;

        public override string ToString(string indent, bool last)
        {
            return
                indent + NodePrefix(last) + $"{UnaryOperator.Operator.Type}" + "\r\n" +
                UnaryExpNode.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public abstract class BinaryExp : ExpNode
    {
        public BinaryExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(startNodePosition)
        {
            Token = token;
            Left = left;
            Right = right;
        }

        public OperatorToken Token { get; }
        public ExpNode Left { get; }
        public ExpNode Right { get; }

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Value}" + "\r\n" +
                   Left.ToString(indent + ChildrenPrefix(last), false) +
                   Right.ToString(indent + ChildrenPrefix(last), true);
        }
    }
    
    public partial class AdditiveExp : BinaryExp
    {
        public AdditiveExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.ADDITIVEEXP;
    }

    public partial class MultExp : BinaryExp
    {
        public MultExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.MULTEXP;
    }

    public partial class ShiftExp : BinaryExp
    {
        public ShiftExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.SHIFTEXP;
    }

    public partial class RelationalExp : BinaryExp
    {
        public RelationalExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.RELATIONALEXP;
    }

    public partial class EqualityExp : BinaryExp
    {
        public EqualityExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.EQUALITYEXP;
    }
    
    public partial class AndExp : BinaryExp
    {
        public AndExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.ANDEXP;
    }

    public partial class ExclusiveOrExp : BinaryExp
    {
        public ExclusiveOrExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.EXCLUSIVEOREXP;
    }

    public partial class InclusiveOrExp : BinaryExp
    {
        public InclusiveOrExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.INCLUSIVEOREXP;
    }

    public partial class LogicalAndExp : BinaryExp
    {
        public LogicalAndExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.LOGICALAND;
    }

    public partial class LogicalOrExp : BinaryExp
    {
        public LogicalOrExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.LOGICALOR;
    }

    public partial class ConditionalExp : ExpNode
    {
        public ExpNode Condition { get; }
        public ExpNode Exp1 { get; }
        public ExpNode Exp2 { get; }

        public ConditionalExp(ExpNode condition, ExpNode exp1, ExpNode exp2, Position startNodePosition) : base(startNodePosition)
        {
            Condition = condition;
            Exp1 = exp1;
            Exp2 = exp2;
        }
        
        public override NodeType Type => NodeType.CONDITIONALEXP;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"CONDITIONAL_EXP" + "\r\n" +
                   Condition.ToString(indent + ChildrenPrefix(last), false) +
                   Exp1.ToString(indent + ChildrenPrefix(last), false) +
                   Exp2.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class AssignmentExp : BinaryExp
    {
        public AssignmentExp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.ASSIGNMENTEXP;
    }

    public partial class Exp : BinaryExp
    {
        public Exp(OperatorToken token, ExpNode left, ExpNode right, Position startNodePosition) : base(token, left, right, startNodePosition) { }
        
        public override NodeType Type => NodeType.EXP;
    }
    
    public class ExpList : List
    {
        public override NodeType Type => NodeType.EXPLIST;
    }
}