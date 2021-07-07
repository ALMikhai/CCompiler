using System.Collections.Generic;
using System.Linq;
using CCompiler.SemanticAnalysis;
using CCompiler.Tokenizer;
using Mono.Cecil.Cil;

namespace CCompiler.Parser
{
    public partial class JumpStat : Node
    {
        public KeywordToken Token { get; }

        public JumpStat(KeywordToken token)
        {
            Token = token;
        }
        
        public override NodeType Type => NodeType.JUMP;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + $"{Token.Type}" + "\r\n";
        }
    }

    public partial class ReturnStat : Node
    {
        public Node Exp { get; }

        public ReturnStat(Node exp)
        {
            Exp = exp;
        }
        
        public override NodeType Type => NodeType.RETURN;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   Exp.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class IfStat : Node
    {
        public ExpNode Exp { get; }
        public Node Stat1 { get; }
        public Node Stat2 { get; }

        public IfStat(ExpNode exp, Node stat1, Node stat2)
        {
            Exp = exp;
            Stat1 = stat1;
            Stat2 = stat2;
        }
        
        public override NodeType Type => NodeType.IF;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   Exp.ToString(indent + ChildrenPrefix(last), false) +
                   Stat1.ToString(indent + ChildrenPrefix(last), false) +
                   Stat2.ToString(indent + ChildrenPrefix(last), true);
        }
    }
    
    public partial class SwitchStat : Node
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
            return indent + NodePrefix(last) + Type + "\r\n" +
                   Exp.ToString(indent + ChildrenPrefix(last), false) +
                   Stat.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class NullStat : Node
    {
        public override NodeType Type => NodeType.NULL;

        public override string ToString(string indent, bool last)
        {
            return "";
        }
    }
    
    public enum WhileType
    {
        WHILE,
        DOWHILE
    }
    
    public partial class WhileStat : Node
    {
        public class Labels
        {
            public Instruction Start { get; }
            public Instruction End { get; }
            public Labels(Instruction start, Instruction end)
            {
                Start = start;
                End = end;
            }
            public Labels()
            {
            }
        }
        
        public ExpNode Exp { get; }
        public Node Stat { get; }
        public WhileType WhileType { get; }

        public WhileStat(ExpNode exp, Node stat, WhileType whileType)
        {
            Exp = exp;
            Stat = stat;
            WhileType = whileType;
        }
        
        public override NodeType Type => NodeType.WHILE;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + WhileType + "\r\n" +
                   Exp.ToString(indent + ChildrenPrefix(last), false) +
                   Stat.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class ForStat : Node
    {
        public Node Exp1 { get; }
        public Node Exp2 { get; }
        public Node Exp3 { get; }
        public Node Stat { get; }

        public ForStat(Node exp1, Node exp2, Node exp3, Node stat)
        {
            Exp1 = exp1;
            Exp2 = exp2;
            Exp3 = exp3;
            Stat = stat;
        }
        
        public override NodeType Type => NodeType.FOR;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   Exp1.ToString(indent + ChildrenPrefix(last), false) +
                   Exp2.ToString(indent + ChildrenPrefix(last), false) +
                   Exp3.ToString(indent + ChildrenPrefix(last), false) +
                   Stat.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class Initializer : Node
    {
        public Node InitializerList { get; }

        public Initializer(Node initializerList)
        {
            InitializerList = initializerList;
        }
        
        public override NodeType Type => NodeType.INITIALIZER;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   InitializerList.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public abstract class List : Node
    {
        public List<Node> Nodes { get; } = new List<Node>();

        public void Add(Node node)
        {
            Nodes.Add(node);
        }
        
        public override string ToString(string indent, bool last)
        {
            var result = indent + NodePrefix(last) + Type + "\r\n";
            for (int i = 0; i < Nodes.Count - 1; i++)
            {
                result += Nodes[i].ToString(indent + ChildrenPrefix(last), false);
            }

            result += Nodes.LastOrDefault()?.ToString(indent + ChildrenPrefix(last), true);
            
            return result;
        }
    }

    public class InitializerList : List
    {
        public override NodeType Type => NodeType.INITIALIZERLIST;
    }

    public partial class ParamDecl : Node
    {
        public DeclSpecs DeclSpec { get; }
        public Declarator Declarator { get; }

        public ParamDecl(DeclSpecs declSpecs, Declarator declarator)
        {
            DeclSpec = declSpecs;
            Declarator = declarator;
        }
        
        public override NodeType Type => NodeType.PARAMDECLARATOR;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   DeclSpec.ToString(indent + ChildrenPrefix(last), false) +
                   Declarator.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class ParamList : List
    {
        public override NodeType Type => NodeType.PARAMLIST;
    }

    public partial class Pointer : Node
    {
        public Node TypeQualifierList { get; }
        public Node PointerNode { get; }

        public Pointer(Node pointerNode, Node typeQualifierList)
        {
            TypeQualifierList = typeQualifierList;
            PointerNode = pointerNode;
        }
        
        public override NodeType Type => NodeType.POINTER;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   TypeQualifierList.ToString(indent + ChildrenPrefix(last), false) +
                   PointerNode.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public abstract partial class GenericDeclaration : Node
    {
        public Node Left { get; }
        
        public GenericDeclaration(Node left)
        {
            Left = left;
        }

        public override NodeType Type => NodeType.DIRECTDECLARATOR;
    }
    
    public partial class ArrayDecl : GenericDeclaration
    {
        public Node ConstExp { get; }

        public ArrayDecl(Node left, Node constExp) : base(left)
        {
            ConstExp = constExp;
        }
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + "ARRAY" + "\r\n" +
                   Left.ToString(indent + ChildrenPrefix(last), false) +
                   ConstExp.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class FuncDecl : GenericDeclaration
    {
        public Node ParamList { get; }

        public FuncDecl(Node left, Node paramList) : base(left)
        {
            ParamList = paramList;
        }

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + "FUNC" + "\r\n" +
                   Left.ToString(indent + ChildrenPrefix(last), false) +
                   ParamList.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class IdList : List
    {
        public override NodeType Type => NodeType.IDLIST;
    }

    public class TypeQualifier : Node
    {
        public KeywordToken Token { get; }

        public TypeQualifier(KeywordToken token)
        {
            Token = token;
        }
        
        public override NodeType Type => NodeType.TYPEQUALIFIER;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Token.Type + "\r\n";
        }
    }

    public class TypeQualifierList : List
    {
        public override NodeType Type => NodeType.TYPEQUALIFIERLIST;
    }

    public partial class Declarator : Node
    {
        public Node Pointer { get; }
        public Node DirectDeclarator { get; }

        public Declarator(Node pointer, Node directDeclarator)
        {
            Pointer = pointer;
            DirectDeclarator = directDeclarator;
        }
        
        public override NodeType Type => NodeType.DECLARATOR;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   Pointer.ToString(indent + ChildrenPrefix(last), false) +
                   DirectDeclarator.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class InitDeclarator : Node
    {
        public Declarator Declarator { get; }
        
        public InitDeclarator(Declarator declarator)
        {
            Declarator = declarator;
        }
        
        public override NodeType Type => NodeType.INITDECLARATOR;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   Declarator.ToString(indent + ChildrenPrefix(last), true);
        }
    }
    
    public partial class InitDeclaratorByExp : InitDeclarator
    {
        public ExpNode Initializer { get; }

        public InitDeclaratorByExp(Declarator declarator, ExpNode initializer) : base(declarator)
        {
            Initializer = initializer;
        }
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   Declarator.ToString(indent + ChildrenPrefix(last), false) +
                   Initializer.ToString(indent + ChildrenPrefix(last), true);
        }
    }
    
    public partial class InitDeclaratorByList : InitDeclarator
    {
        public InitializerList InitializerList { get; }

        public InitDeclaratorByList(Declarator declarator, InitializerList initializerList) : base(declarator)
        {
            InitializerList = initializerList;
        }
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   Declarator.ToString(indent + ChildrenPrefix(last), false) +
                   InitializerList.ToString(indent + ChildrenPrefix(last), true);
        }
    }
    
    public class InitDeclaratorList : List
    {
        public override NodeType Type => NodeType.INITDECLARATORLIST;
    }

    public class TypeSpec : Node
    {
        public KeywordToken TokenType { get; }

        public TypeSpec(KeywordToken tokenType)
        {
            TokenType = tokenType;
        }
        
        public override NodeType Type => NodeType.TYPESPEC;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + TokenType.Type + "\r\n";
        }
    }

    public class StorageClassSpec : Node
    {
        public KeywordToken Token { get; }

        public StorageClassSpec(KeywordToken token)
        {
            Token = token;
        }
        
        public override NodeType Type => NodeType.STORAGECLASSSPEC;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Token.Value + "\r\n";
        }
    }

    public partial class DeclSpecs : Node
    {
        public Node Spec { get; }
        public Node NextSpec { get; }

        public DeclSpecs(Node spec, Node nextSpec)
        {
            Spec = spec;
            NextSpec = nextSpec;
        }
        
        public override NodeType Type => NodeType.DECLSPEC;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   Spec.ToString(indent + ChildrenPrefix(last), false) +
                   NextSpec.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class Decl : Node
    {
        public DeclSpecs DeclSpec { get; }
        public InitDeclaratorList InitDeclaratorList { get; }

        public Decl(DeclSpecs declSpec, InitDeclaratorList initDeclaratorList)
        {
            DeclSpec = declSpec;
            InitDeclaratorList = initDeclaratorList;
        }
        
        public override NodeType Type => NodeType.DECL;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   DeclSpec.ToString(indent + ChildrenPrefix(last), false) +
                   InitDeclaratorList.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class StatList : List
    {
        public override NodeType Type => NodeType.STATLIST;
    }
    
    public class DeclList : List
    {
        public override NodeType Type => NodeType.DECLLIST;
    }

    public partial class CompoundStat : Node
    {
        public Node DeclList { get; }
        public Node StatList { get; }
        public EnvironmentSnapshot Snapshot { get; private set; }

        public CompoundStat(Node declList, Node statList)
        {
            DeclList = declList;
            StatList = statList;
        }
        
        public override NodeType Type => NodeType.COMPOUNDSTAT;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   DeclList.ToString(indent + ChildrenPrefix(last), false) +
                   StatList.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class EmptyExp : Node
    {
        public override NodeType Type => NodeType.EMPTY;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n";
        }
    }

    public partial class FuncDef : Node
    {
        public Node DeclSpec { get; }
        public InitDeclarator Declarator { get; }
        public Node DeclList { get; }
        public CompoundStat CompoundStat { get; }

        public FuncDef(Node declSpec, InitDeclarator declarator, Node declList, CompoundStat compoundStat)
        {
            DeclSpec = declSpec;
            Declarator = declarator;
            DeclList = declList;
            CompoundStat = compoundStat;
        }
        
        public override NodeType Type => NodeType.FUNC;

        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   DeclSpec.ToString(indent + ChildrenPrefix(last), false) +
                   Declarator.ToString(indent + ChildrenPrefix(last), false) +
                   DeclList.ToString(indent + ChildrenPrefix(last), false) +
                   CompoundStat.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class TranslationUnit : List
    {
        public override NodeType Type => NodeType.TRANSLATIONUNIT;
    }

    public class StructDeclaratorList : List
    {
        public override NodeType Type => NodeType.STRUCTDECLARATORLIST;
    }

    public class StructDecl : Node
    {
        public DeclSpecs DeclSpec { get; }
        public StructDeclaratorList DeclaratorList { get; }

        public StructDecl(DeclSpecs declSpec, StructDeclaratorList declaratorList)
        {
            DeclSpec = declSpec;
            DeclaratorList = declaratorList;
        }
        
        public override NodeType Type => NodeType.STRUCTDECL;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   DeclSpec.ToString(indent + ChildrenPrefix(last), false) +
                   DeclaratorList.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public class StructDeclList : List
    {
        public override NodeType Type => NodeType.STRUCTDECLLIST;
    }

    public partial class StructSpec : Node
    {
        public Id Id { get; }
        public Node StructDeclList { get; }

        public StructSpec(Id id, Node structDeclList)
        {
            Id = id;
            StructDeclList = structDeclList;
        }
        
        public override NodeType Type => NodeType.STRUCTSPEC;
        
        public override string ToString(string indent, bool last)
        {
            return indent + NodePrefix(last) + Type + "\r\n" +
                   Id.ToString(indent + ChildrenPrefix(last), false) +
                   StructDeclList.ToString(indent + ChildrenPrefix(last), true);
        }
    }

    public partial class ExpStat : Node
    {
        public Node ExpNode { get; }

        public ExpStat(Node expNode)
        {
            ExpNode = expNode;
        }
        
        public override string ToString(string indent, bool last) => ExpNode.ToString(indent, last);
    }
}