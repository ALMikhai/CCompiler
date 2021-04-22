using System;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public enum NodeType
    {
        CONST,
        PRIMARYEXP,
        POSTFIXEXP,
        UNARYOPERATOR,
        ADDITIVEEXP,
        MULTEXP,
        UNARYEXP,
        CASTEXP
    }

    public class Node
    {
        public virtual NodeType Type { get; }

        public override string ToString()
        {
            return ToString("", true);
        }

        public virtual string ToString(string indent, bool last)
        {
            return "";
        }

        public static string NodePrefix(bool last)
        {
            return last ? "\\-" : "|-";
        }

        public static string ChildrenPrefix(bool last)
        {
            return last ? "  " : "| ";
        }
    }

    public class Const : Node
    {
        public Token Token { get; }
        public override NodeType Type => NodeType.CONST;

        public Const(Token token)
        {
            Token = token;
        }

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Value}\n";
        }
    }

    public class PrimaryExp : Node
    {
        public Token Token { get; }
        public override NodeType Type => NodeType.PRIMARYEXP;

        public PrimaryExp(Token token)
        {
            Token = token;
        }

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Value}\n";
        }
    }

    public class PostfixExp : Node
    {
        public Token Token { get; }
        public Node Child { get; }
        public override NodeType Type => NodeType.POSTFIXEXP;

        public PostfixExp(Token token, Node child)
        {
            Child = child;
            Token = token;
        }

        public override string ToString(string indent, bool last)
        {
            throw new NotImplementedException();
        }
    }

    public class UnaryOperator : Node
    {
        public OperatorToken Token { get; }
        public override NodeType Type => NodeType.UNARYOPERATOR;

        public UnaryOperator(OperatorToken token)
        {
            Token = token;
        }

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Value}\n";
        }
    }

    public class UnaryExp : Node
    {
        public override NodeType Type => NodeType.UNARYEXP;

        public UnaryExp()
        {
        }
    }

    public class CastExp : Node
    {
        public override NodeType Type => NodeType.CASTEXP;

        public CastExp()
        {
        }
    }

    public class AdditiveExp : Node
    {
        public OperatorToken Token { get; }
        public Node Left { get; }
        public Node Right { get; }
        public override NodeType Type => NodeType.ADDITIVEEXP;

        public AdditiveExp(OperatorToken token, Node left, Node right)
        {
            Token = token;
            Left = left;
            Right = right;
        }

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Value}\n" +
                   Left.ToString(indent + ChildrenPrefix(last), false) +
                   Right.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class MultExp : Node
    {
        public OperatorToken Token { get; }
        public Node Left { get; }
        public Node Right { get; }
        public override NodeType Type => NodeType.MULTEXP;

        public MultExp(OperatorToken token, Node left, Node right)
        {
            Token = token;
            Left = left;
            Right = right;
        }

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Value}\n" +
                   Left.ToString(indent + ChildrenPrefix(last), false) +
                   Right.ToString(indent + ChildrenPrefix(last), true);
        }
    }
}