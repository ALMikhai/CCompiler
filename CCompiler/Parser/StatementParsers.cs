using System;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class SyntaxParser
    {
        /*
         * stat	: labeled_stat
            | exp_stat
            | compound_stat
            | selection_stat
            | iteration_stat
            | jump_stat
            ;
         */

        public IParseResult ParseStat()
        {
            var selectionStat = ParseSelectionStat();
            if (selectionStat.IsSuccess)
                return selectionStat;
            
            var jumpStat = ParseJumpStat();
            if (jumpStat.IsSuccess)
                return jumpStat;

            return new FailedParseResult("expected statement", _currentToken);
        }
        
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
        
        /*
         * selection_stat : 'if' '(' exp ')' stat
            | 'if' '(' exp ')' stat 'else' stat
            | 'switch' '(' exp ')' stat
            ;
         */

        public IParseResult ParseSelectionStat()
        {
            if (AcceptKeyword(KeywordType.IF))
            {
                if (ExceptOp(OperatorType.LRBRACKET))
                {
                    var exp = ParseExp();
                    if (!exp.IsSuccess)
                    {
                        return exp;
                    }
                    
                    if (ExceptOp(OperatorType.RRBRACKET))
                    {
                        var stat = ParseStat();
                        if (!stat.IsSuccess)
                        {
                            return stat;
                        }

                        if (AcceptKeyword(KeywordType.ELSE))
                        {
                            var stat2 = ParseJumpStat();
                            if (!stat2.IsSuccess)
                            {
                                return stat2;
                            }

                            return new SuccessParseResult(new IfStat(exp.ResultNode, stat.ResultNode,
                                stat2.ResultNode));
                        }

                        return new SuccessParseResult(new IfStat(exp.ResultNode, stat.ResultNode, null));
                    }
                }
            }

            if (AcceptKeyword(KeywordType.SWITCH))
            {
                if (ExceptOp(OperatorType.LRBRACKET))
                {
                    var exp = ParseExp();
                    if (!exp.IsSuccess)
                    {
                        return exp;
                    }

                    if (ExceptOp(OperatorType.RRBRACKET))
                    {
                        var stat = ParseStat();
                        if (!stat.IsSuccess)
                        {
                            return stat;
                        }
                        
                        return new SuccessParseResult(new SwitchStat(exp.ResultNode, stat.ResultNode));
                    }
                }
            }
            
            return new FailedParseResult("expected selection statement", _currentToken);
        }
    }
}