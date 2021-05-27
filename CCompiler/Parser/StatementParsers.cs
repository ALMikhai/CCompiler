using System;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class SyntaxParser
    {
        /*
         *jump_stat	: 'goto' id ';'
            | 'continue' ';'
            | 'break' ';'
            | 'return' exp ';'
            | 'return'	';'
            ;
         */

        public IParseResult ParseJumpStat()
        {
            if (AcceptKeyword(KeywordType.CONTINUE))
            {
                if (ExceptOp(OperatorType.SEMICOLON))
                {
                    return new SuccessParseResult(new JumpStat(KeywordType.CONTINUE));
                }
            }

            if (AcceptKeyword(KeywordType.BREAK))
            {
                if (ExceptOp(OperatorType.SEMICOLON))
                {
                    return new SuccessParseResult(new JumpStat(KeywordType.BREAK));
                }
            }
            
            if (AcceptKeyword(KeywordType.RETURN))
            {
                if (AcceptOp(OperatorType.SEMICOLON))
                {
                    return new SuccessParseResult(new JumpStat(KeywordType.RETURN));
                }

                var exp = ParseExp();
                if (!exp.IsSuccess)
                    return exp;
                
                if (ExceptOp(OperatorType.SEMICOLON))
                {
                    return new SuccessParseResult(new ReturnStat(exp.ResultNode));
                }
            }
            
            return new FailedParseResult("expected jump statement", _currentToken);
        }
    }
}