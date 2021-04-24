using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        CASTEXP,
        TYPENAME
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
        public Node PostfixNode { get; }
        public OperatorToken Token { get; }
        public Node Child { get; }
        public Token Identifier { get; }
        public override NodeType Type => NodeType.POSTFIXEXP;

        public PostfixExp(Node postfixNode, OperatorToken token, Node child)
        {
            PostfixNode = postfixNode;
            Child = child;
            Token = token;
            Identifier = null;
        }

        public PostfixExp(Node postfixNode, OperatorToken token, Token identifier)
        {
            PostfixNode = postfixNode;
            Child = null;
            Token = token;
            Identifier = identifier;
        }

        public override string ToString(string indent, bool last)
        {
            return
                PostfixNode.ToString(indent, last) +
                indent + ChildrenPrefix(last) + NodePrefix(Child == null && Identifier == null) + $"{Token.Value}\n" +
                (Child != null ? Child.ToString(indent + ChildrenPrefix(last), true) : "") +
                (Identifier != null ? new PrimaryExp(Identifier).ToString(indent + ChildrenPrefix(last), true) : "");
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
        public Node UnaryExpNode { get; }
        public OperatorToken OperatorToken { get; }
        public UnaryOperator UnaryOperator { get; }
        
        public override NodeType Type => NodeType.UNARYEXP;

        public UnaryExp(Node unaryExpNode, OperatorToken token, UnaryOperator unaryOperator)
        {
            UnaryOperator = unaryOperator;
            OperatorToken = token;
            UnaryExpNode = unaryExpNode;
        }
        
        public override string ToString(string indent, bool last)
        {
            return (OperatorToken != null ? indent + NodePrefix(last) + $"{OperatorToken.Value}\n" : "") +
                   (UnaryOperator != null ? UnaryOperator.ToString(indent, last) : "") +
                   UnaryExpNode.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class CastExp : Node 
    {
        public Typename Typename { get; }
        public Node CastExpNode { get; }
        public override NodeType Type => NodeType.CASTEXP;

        public CastExp(Typename typename, Node castExpNode)
        {
            Typename = typename;
            CastExpNode = castExpNode;
        }

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
            new Dictionary<TypenameTypes, List<KeywordType>>()
            {
                {TypenameTypes.CHAR, new List<KeywordType>() {KeywordType.CHAR}},
                {TypenameTypes.SCHAR, new List<KeywordType>() {KeywordType.SIGNED, KeywordType.CHAR}},
                {TypenameTypes.UCHAR, new List<KeywordType>() {KeywordType.UNSIGNED, KeywordType.CHAR}},
                {TypenameTypes.SHORT, new List<KeywordType>() {KeywordType.SHORT}},
                {TypenameTypes.SHORTINT, new List<KeywordType>() {KeywordType.SHORT, KeywordType.INT}},
                {TypenameTypes.SSHORT, new List<KeywordType>() {KeywordType.SIGNED, KeywordType.SHORT}},
                {TypenameTypes.SSHORTINT, new List<KeywordType>() {KeywordType.SIGNED, KeywordType.SHORT, KeywordType.INT}},
                {TypenameTypes.USHORT, new List<KeywordType>() {KeywordType.UNSIGNED, KeywordType.SHORT}},
                {TypenameTypes.USHORTINT, new List<KeywordType>() {KeywordType.UNSIGNED, KeywordType.SHORT, KeywordType.INT}},
                {TypenameTypes.INT, new List<KeywordType>() {KeywordType.INT}},
                {TypenameTypes.SIGNED, new List<KeywordType>() {KeywordType.SIGNED}},
                {TypenameTypes.SINT, new List<KeywordType>() {KeywordType.SIGNED, KeywordType.INT}},
                {TypenameTypes.UNSIGNED, new List<KeywordType>() {KeywordType.UNSIGNED}},
                {TypenameTypes.UINT, new List<KeywordType>() {KeywordType.UNSIGNED, KeywordType.INT}},
                {TypenameTypes.LONG, new List<KeywordType>() {KeywordType.LONG}},
                {TypenameTypes.LINT, new List<KeywordType>() {KeywordType.LONG, KeywordType.INT}},
                {TypenameTypes.SLONG, new List<KeywordType>() {KeywordType.SIGNED, KeywordType.LONG}},
                {TypenameTypes.SLONGINT, new List<KeywordType>() {KeywordType.SIGNED, KeywordType.LONG, KeywordType.INT}},
                {TypenameTypes.ULONG, new List<KeywordType>() {KeywordType.UNSIGNED, KeywordType.LONG}},
                {TypenameTypes.ULONGINT, new List<KeywordType>() {KeywordType.UNSIGNED, KeywordType.LONG, KeywordType.INT}},
                {TypenameTypes.LONGLONG, new List<KeywordType>() {KeywordType.LONG, KeywordType.LONG}},
                {TypenameTypes.LONGLONGINT, new List<KeywordType>() {KeywordType.LONG, KeywordType.LONG, KeywordType.INT}},
                {TypenameTypes.SLONGLONG, new List<KeywordType>() {KeywordType.SIGNED, KeywordType.LONG, KeywordType.LONG}},
                {TypenameTypes.SLONGLONGINT, new List<KeywordType>() {KeywordType.SIGNED, KeywordType.LONG, KeywordType.LONG, KeywordType.INT}},
                {TypenameTypes.ULONGLONG, new List<KeywordType>() {KeywordType.UNSIGNED, KeywordType.LONG, KeywordType.LONG}},
                {TypenameTypes.ULONGLONGINT, new List<KeywordType>() {KeywordType.UNSIGNED, KeywordType.LONG, KeywordType.LONG, KeywordType.INT}},
                {TypenameTypes.FLOAT, new List<KeywordType>() {KeywordType.FLOAT}},
                {TypenameTypes.DOUBLE, new List<KeywordType>() {KeywordType.DOUBLE}},
                {TypenameTypes.LDOUBLE, new List<KeywordType>() {KeywordType.LONG, KeywordType.DOUBLE}}
            };

        public TypenameTypes TypenameType { get; }
        public override NodeType Type => NodeType.TYPENAME;

        public Typename(TypenameTypes typenameType)
        {
            TypenameType = typenameType;
        }

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{TypenameType}\n";
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