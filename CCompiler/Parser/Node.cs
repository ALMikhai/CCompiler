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
        public NodeType Type { get; }

        public Node(NodeType type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public virtual string ToString(int spaces)
        {
            return "";
        }
    }
    
    public class Const : Node
    {
        public Token Token { get; }
        public Const(Token token) : base(NodeType.CONST)
        {
            Token = token;
        }

        public override string ToString(int spaces)
        {
            return new String(' ', spaces) + $"-{Token.Value}\n";
        }
    }

    public class PrimaryExp : Node
    {
        public Token Token { get; }
        public PrimaryExp(Token token) : base(NodeType.PRIMARYEXP)
        {
            Token = token;
        }
        
        public override string ToString(int spaces)
        {
            return new String(' ', spaces) + $"-{Token.Value}\n";
        }
    }

    public class PostfixExp : Node
    {
        public Token Token { get; }
        public Node Child { get; }

        public PostfixExp(Token token, Node child) : base(NodeType.POSTFIXEXP)
        {
            Child = child;
            Token = token;
        }
        
        public override string ToString(int spaces)
        {
            throw new NotImplementedException();
        }
    }

    public class UnaryOperator : Node
    {
        public Token Token { get; }

        public UnaryOperator(Token token) : base(NodeType.UNARYOPERATOR)
        {
            Token = token;
        }
        
        public override string ToString(int spaces)
        {
            return new String(' ', spaces) + $"-{Token.Value}\n";
        }
    }

    public class UnaryExp : Node
    {
        public UnaryExp() : base(NodeType.UNARYEXP)
        {
        }
    }
    
    public class CastExp : Node
    {
        public CastExp() : base(NodeType.CASTEXP)
        {
        }
    }

    public class AdditiveExp : Node
    {
        public Token Token { get; }
        public Node Left { get; }
        public Node Right { get; }

        public AdditiveExp(Token token, Node left, Node right) : base(
            NodeType.ADDITIVEEXP)
        {
            Token = token;
            Left = left;
            Right = right;
        }
        
        public override string ToString(int spaces)
        {
            return new String(' ', spaces) + $"-{Token.Value}\n" + Left.ToString(spaces + 1) + Right.ToString(spaces + 1);
        }
    }

    public class MultExp : Node
    {
        public Token Token { get; }
        public Node Left { get; }
        public Node Right { get; }

        public MultExp(Token token, Node left, Node right) : base(NodeType.MULTEXP)
        {
            Token = token;
            Left = left;
            Right = right;
        }
        
        public override string ToString(int spaces)
        {
            return new String(' ', spaces) + $"-{Token.Value}\n" + Left.ToString(spaces + 1) + Right.ToString(spaces + 1);
        }
    }
}
