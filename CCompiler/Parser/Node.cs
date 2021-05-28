using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public enum NodeType
    {
        NULL,
        CONST,
        PRIMARYEXP,
        POSTFIXEXP,
        UNARYOPERATOR,
        ADDITIVEEXP,
        MULTEXP,
        UNARYEXP,
        CASTEXP,
        TYPENAME,
        SHIFTEXP,
        RELATIONALEXP,
        EQUALITYEXP,
        ANDEXP,
        EXCLUSIVEOREXP,
        INCLUSIVEOREXP,
        LOGICALAND,
        LOGICALOR,
        CONDITIONALEXP,
        ASSIGNMENTEXP,
        EXP,
        JUMP,
        RETURN,
        IF,
        SWITCH,
        WHILE,
        FOR
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
}