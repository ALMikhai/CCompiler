using System.Collections.Generic;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
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

    public class PrimaryExp : Node
    {
        public PrimaryExp(Token token)
        {
            Token = token;
        }

        public Token Token { get; }
        public override NodeType Type => NodeType.PRIMARYEXP;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Value}" + "\r\n";
        }
    }

    public class PostfixExp : Node
    {
        public PostfixExp(Node postfixNode, OperatorToken @operator, Node suffixNode)
        {
            PostfixNode = postfixNode;
            SuffixNode = suffixNode;
            Operator = @operator;
            Identifier = null;
        }

        public PostfixExp(Node postfixNode, OperatorToken @operator, Token identifier)
        {
            PostfixNode = postfixNode;
            SuffixNode = null;
            Operator = @operator;
            Identifier = identifier;
        }

        public Node PostfixNode { get; }
        public OperatorToken Operator { get; }
        public Node SuffixNode { get; }
        public Token Identifier { get; }
        public override NodeType Type => NodeType.POSTFIXEXP;

        public override string ToString(string indent, bool last)
        {
            return
                indent + NodePrefix(last) + $"{Operator.Value}" + "\r\n" +
                PostfixNode.ToString(indent + ChildrenPrefix(last), SuffixNode == null && Identifier == null) +
                (SuffixNode != null ? SuffixNode.ToString(indent + ChildrenPrefix(last), true) : "") +
                (Identifier != null ? new PrimaryExp(Identifier).ToString(indent + ChildrenPrefix(last), true) : "");
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

    public class UnaryExp : Node
    {
        public UnaryExp(Node unaryExpNode, UnaryOperator unaryOperator)
        {
            UnaryOperator = unaryOperator;
            OperatorOperator = null;
            UnaryExpNode = unaryExpNode;
        }
        
        public UnaryExp(Node unaryExpNode, OperatorToken @operator)
        {
            UnaryOperator = null;
            OperatorOperator = @operator;
            UnaryExpNode = unaryExpNode;
        }

        public Node UnaryExpNode { get; }
        public OperatorToken OperatorOperator { get; }
        public UnaryOperator UnaryOperator { get; }

        public override NodeType Type => NodeType.UNARYEXP;

        public override string ToString(string indent, bool last)
        {
            return (OperatorOperator != null ? indent + NodePrefix(last) + $"{OperatorOperator.Value}" + "\r\n" : "") +
                   (UnaryOperator != null ? UnaryOperator.ToString(indent, last) : "") +
                   UnaryExpNode.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class CastExp : Node
    {
        public CastExp(Typename typename, Node castExpNode)
        {
            Typename = typename;
            CastExpNode = castExpNode;
        }

        public Typename Typename { get; }
        public Node CastExpNode { get; }
        public override NodeType Type => NodeType.CASTEXP;

        public override string ToString(string indent, bool last)
        {
            return Typename.ToString(indent, last) +
                   CastExpNode.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class Typename : Node
    {
        public enum TypenameTypes
        {
            NONE,
            CHAR,
            SCHAR,
            UCHAR,
            SHORT,
            SHORTINT,
            SSHORT,
            SSHORTINT,
            USHORT,
            USHORTINT,
            INT,
            SIGNED,
            SINT,
            UNSIGNED,
            UINT,
            LONG,
            LINT,
            SLONG,
            SLONGINT,
            ULONG,
            ULONGINT,
            LONGLONG,
            LONGLONGINT,
            SLONGLONG,
            SLONGLONGINT,
            ULONGLONG,
            ULONGLONGINT,
            FLOAT,
            DOUBLE,
            LDOUBLE
        }

        public static Dictionary<TypenameTypes, List<KeywordType>> Nodes2Type =
            new Dictionary<TypenameTypes, List<KeywordType>>
            {
                {TypenameTypes.CHAR, new List<KeywordType> {KeywordType.CHAR}},
                {TypenameTypes.SCHAR, new List<KeywordType> {KeywordType.SIGNED, KeywordType.CHAR}},
                {TypenameTypes.UCHAR, new List<KeywordType> {KeywordType.UNSIGNED, KeywordType.CHAR}},
                {TypenameTypes.SHORT, new List<KeywordType> {KeywordType.SHORT}},
                {TypenameTypes.SHORTINT, new List<KeywordType> {KeywordType.SHORT, KeywordType.INT}},
                {TypenameTypes.SSHORT, new List<KeywordType> {KeywordType.SIGNED, KeywordType.SHORT}},
                {
                    TypenameTypes.SSHORTINT,
                    new List<KeywordType> {KeywordType.SIGNED, KeywordType.SHORT, KeywordType.INT}
                },
                {TypenameTypes.USHORT, new List<KeywordType> {KeywordType.UNSIGNED, KeywordType.SHORT}},
                {
                    TypenameTypes.USHORTINT,
                    new List<KeywordType> {KeywordType.UNSIGNED, KeywordType.SHORT, KeywordType.INT}
                },
                {TypenameTypes.INT, new List<KeywordType> {KeywordType.INT}},
                {TypenameTypes.SIGNED, new List<KeywordType> {KeywordType.SIGNED}},
                {TypenameTypes.SINT, new List<KeywordType> {KeywordType.SIGNED, KeywordType.INT}},
                {TypenameTypes.UNSIGNED, new List<KeywordType> {KeywordType.UNSIGNED}},
                {TypenameTypes.UINT, new List<KeywordType> {KeywordType.UNSIGNED, KeywordType.INT}},
                {TypenameTypes.LONG, new List<KeywordType> {KeywordType.LONG}},
                {TypenameTypes.LINT, new List<KeywordType> {KeywordType.LONG, KeywordType.INT}},
                {TypenameTypes.SLONG, new List<KeywordType> {KeywordType.SIGNED, KeywordType.LONG}},
                {TypenameTypes.SLONGINT, new List<KeywordType> {KeywordType.SIGNED, KeywordType.LONG, KeywordType.INT}},
                {TypenameTypes.ULONG, new List<KeywordType> {KeywordType.UNSIGNED, KeywordType.LONG}},
                {
                    TypenameTypes.ULONGINT,
                    new List<KeywordType> {KeywordType.UNSIGNED, KeywordType.LONG, KeywordType.INT}
                },
                {TypenameTypes.LONGLONG, new List<KeywordType> {KeywordType.LONG, KeywordType.LONG}},
                {
                    TypenameTypes.LONGLONGINT,
                    new List<KeywordType> {KeywordType.LONG, KeywordType.LONG, KeywordType.INT}
                },
                {
                    TypenameTypes.SLONGLONG,
                    new List<KeywordType> {KeywordType.SIGNED, KeywordType.LONG, KeywordType.LONG}
                },
                {
                    TypenameTypes.SLONGLONGINT,
                    new List<KeywordType> {KeywordType.SIGNED, KeywordType.LONG, KeywordType.LONG, KeywordType.INT}
                },
                {
                    TypenameTypes.ULONGLONG,
                    new List<KeywordType> {KeywordType.UNSIGNED, KeywordType.LONG, KeywordType.LONG}
                },
                {
                    TypenameTypes.ULONGLONGINT,
                    new List<KeywordType> {KeywordType.UNSIGNED, KeywordType.LONG, KeywordType.LONG, KeywordType.INT}
                },
                {TypenameTypes.FLOAT, new List<KeywordType> {KeywordType.FLOAT}},
                {TypenameTypes.DOUBLE, new List<KeywordType> {KeywordType.DOUBLE}},
                {TypenameTypes.LDOUBLE, new List<KeywordType> {KeywordType.LONG, KeywordType.DOUBLE}}
            };

        public Typename(TypenameTypes typenameType)
        {
            TypenameType = typenameType;
        }

        public TypenameTypes TypenameType { get; }
        public override NodeType Type => NodeType.TYPENAME;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{TypenameType}" + "\r\n";
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

        public static AssignmentExp Instance(OperatorToken token, Node left, Node right)
        {
            return new AssignmentExp(token, left, right);
        }
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