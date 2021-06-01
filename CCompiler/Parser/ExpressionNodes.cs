using System;
using System.Collections.Generic;
using System.Globalization;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public class ValueType
    {
        public enum Type
        {
            INT,
            FLOAT,
            CHAR
        }

        public object Value { get; }
        public Type CurrentType { get; }

        private readonly Dictionary<TokenType, Type> _tokenTypeToValueType = new Dictionary<TokenType, Type>()
        {
            {TokenType.INT, Type.INT},
            {TokenType.FLOAT, Type.FLOAT},
            {TokenType.CHAR, Type.CHAR}
        };
        
        public ValueType(Token token)
        {
            Value = token.Value;
            CurrentType = _tokenTypeToValueType[token.TokenType];
        }
        
        public long GetInt()
        {
            return (long) Value;
        }

        public double GetFloat()
        {
            return (double) Value;
        }

        public char GetChar()
        {
            return (char) Value;
        }
    }
    
    public abstract class ExpType : Node
    {
        public abstract ValueType GetValue();
        public abstract bool IsLValue();
    }

    public class Const : Node
    {
        public Const(Token token)
        {
            Token = token;
        }

        public Token Token { get; }
        public override NodeType Type => NodeType.CONST;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Value}" + "\r\n";
        }
    }

    public class Id : Node
    {
        public string IdName { get; }

        public Id(string idName)
        {
            IdName = idName;
        }

        public override NodeType Type => NodeType.PRIMARYEXP;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + IdName + "\r\n";
        }
    }

    public class String : Node
    {
        public string Str { get; }
        
        public String(string str)
        {
            Str = str;
        }
        
        public override NodeType Type => NodeType.PRIMARYEXP;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Str + "\r\n";
        }
    }

    public class AccessingArrayElement : Node
    {
        public Node PostfixNode { get; }
        public Node Exp { get; }

        public AccessingArrayElement(Node postfixNode, Node exp)
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

    public class FuncCall : Node
    {
        public Node PostfixNode { get; }
        public Node Exp { get; }

        public FuncCall(Node postfixNode, Node exp)
        {
            PostfixNode = postfixNode;
            Exp = exp;
        }
        
        public override NodeType Type => NodeType.POSTFIXEXP;

        public override string ToString(string indent, bool last)
        {
            return
                indent + NodePrefix(last) + $"()" + "\r\n" +
                PostfixNode.ToString(indent + ChildrenPrefix(last), false) +
                Exp.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class MemberCall : Node
    {
        public enum CallType
        {
            VALUE,
            POINTER
        }
        
        public Node PostfixNode { get; }
        public Node Id { get; }

        private readonly CallType _callType;

        public MemberCall(Node postfixNode, Node id, CallType callType)
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

    public class PostfixIncDec : Node
    {
        private readonly OpType _opType;

        public enum OpType
        {
            INC,
            DEC
        }
        public Node PostfixNode { get; }

        public PostfixIncDec(Node postfixNode, OpType opType)
        {
            _opType = opType;
            PostfixNode = postfixNode;
        }
        
        public override NodeType Type => NodeType.POSTFIXEXP;

        public override string ToString(string indent, bool last)
        {
            return
                indent + NodePrefix(last) + $"POSTFIX {_opType}" + "\r\n" +
                PostfixNode.ToString(indent + ChildrenPrefix(last), true);
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

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Operator.Value}" + "\r\n";
        }
    }

    public class PrefixIncDec : Node
    {
        private readonly OpType _opType;

        public enum OpType
        {
            INC,
            DEC
        }
        public Node PostfixNode { get; }

        public PrefixIncDec(Node postfixNode, OpType opType)
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
    
    public class UnaryExp : Node
    {
        public UnaryExp(Node unaryExpNode, UnaryOperator unaryOperator)
        {
            UnaryOperator = unaryOperator;
            UnaryExpNode = unaryExpNode;
        }

        public Node UnaryExpNode { get; }
        public UnaryOperator UnaryOperator { get; }

        public override NodeType Type => NodeType.UNARYEXP;

        public override string ToString(string indent, bool last)
        {
            return
                indent + NodePrefix(last) + $"{UnaryOperator.Operator.Type}" + "\r\n" +
                UnaryExpNode.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public abstract class BinaryExp : Node
    {
        public BinaryExp(OperatorToken token, Node left, Node right)
        {
            Token = token;
            Left = left;
            Right = right;
        }

        public OperatorToken Token { get; }
        public Node Left { get; }
        public Node Right { get; }

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Value}" + "\r\n" +
                   Left.ToString(indent + ChildrenPrefix(last), false) +
                   Right.ToString(indent + ChildrenPrefix(last), true);
        }
    }
    
    public class AdditiveExp : BinaryExp
    {
        public AdditiveExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.ADDITIVEEXP;

        public static AdditiveExp Instance(OperatorToken token, Node left, Node right)
        {
            return new AdditiveExp(token, left, right);
        }
    }

    public class MultExp : BinaryExp
    {
        public MultExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.MULTEXP;
        
        public static MultExp Instance(OperatorToken token, Node left, Node right)
        {
            return new MultExp(token, left, right);
        }
    }

    public class ShiftExp : BinaryExp
    {
        public ShiftExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.SHIFTEXP;
        
        public static ShiftExp Instance(OperatorToken token, Node left, Node right)
        {
            return new ShiftExp(token, left, right);
        }
    }

    public class RelationalExp : BinaryExp
    {
        public RelationalExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.RELATIONALEXP;

        public static RelationalExp Instance(OperatorToken token, Node left, Node right)
        {
            return new RelationalExp(token, left, right);
        }
    }

    public class EqualityExp : BinaryExp
    {
        public EqualityExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.EQUALITYEXP;

        public static EqualityExp Instance(OperatorToken token, Node left, Node right)
        {
            return new EqualityExp(token, left, right);
        }
    }
    
    public class AndExp : BinaryExp
    {
        public AndExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.ANDEXP;

        public static AndExp Instance(OperatorToken token, Node left, Node right)
        {
            return new AndExp(token, left, right);
        }
    }

    public class ExclusiveOrExp : BinaryExp
    {
        public ExclusiveOrExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.EXCLUSIVEOREXP;

        public static ExclusiveOrExp Instance(OperatorToken token, Node left, Node right)
        {
            return new ExclusiveOrExp(token, left, right);
        }
    }

    public class InclusiveOrExp : BinaryExp
    {
        public InclusiveOrExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.INCLUSIVEOREXP;

        public static InclusiveOrExp Instance(OperatorToken token, Node left, Node right)
        {
            return new InclusiveOrExp(token, left, right);
        }
    }

    public class LogicalAndExp : BinaryExp
    {
        public LogicalAndExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.LOGICALAND;

        public static LogicalAndExp Instance(OperatorToken token, Node left, Node right)
        {
            return new LogicalAndExp(token, left, right);
        }
    }

    public class LogicalOrExp : BinaryExp
    {
        public LogicalOrExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.LOGICALOR;

        public static LogicalOrExp Instance(OperatorToken token, Node left, Node right)
        {
            return new LogicalOrExp(token, left, right);
        }
    }

    public class ConditionalExp : Node
    {
        public Node Condition { get; }
        public Node Exp1 { get; }
        public Node Exp2 { get; }

        public ConditionalExp(Node condition, Node exp1, Node exp2)
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

    public class AssignmentExp : BinaryExp
    {
        public AssignmentExp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.ASSIGNMENTEXP;
    }

    public class Exp : BinaryExp
    {
        public Exp(OperatorToken token, Node left, Node right) : base(token, left, right)
        {
        }
        
        public override NodeType Type => NodeType.EXP;
        
        public static Exp Instance(OperatorToken token, Node left, Node right)
        {
            return new Exp(token, left, right);
        }
    }
}