using System.Collections.Generic;
using System.Linq;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class SyntaxParser
    {
        delegate IParseResult Parser();
        delegate List ListCtor();
        delegate Node ExpCtor(OperatorToken token, Node left, Node right);

        private IParseResult ParseBinaryExp(Parser parser, ExpCtor ctor, IEnumerable<OperatorType> availableOperators)
        {
            var left = parser();
            if (!left.IsSuccess || left.IsNullStat())
                return left;

            OperatorToken op = null;
            // ReSharper disable once PossibleMultipleEnumeration
            while (Accept(availableOperators, ref op))
            {
                var right = parser();
                if (!right.IsSuccess)
                    return right;
                if (right.IsNullStat())
                    return ExpectedExpressionFailure();

                left = new SuccessParseResult(ctor(op, left.ResultNode,
                    right.ResultNode));
            }

            return left;
        }

        private IParseResult ParseList(Parser parser, ListCtor ctor, OperatorType separator)
        {
            var list = ctor();
            
            do
            {
                var parseResult = parser();
                if (!parseResult.IsSuccess)
                    return parseResult;
                if (parseResult.IsNullStat())
                    if (list.Nodes.Any())
                        return ExpectedExpressionFailure();
                    else
                        break;

                list.Add(parseResult.ResultNode);
            } while (Accept(separator));

            if (list.Nodes.Count == 0)
                return new SuccessParseResult(new NullStat());

            return new SuccessParseResult(list);
        }
        
        private IParseResult ParseList(Parser parser, ListCtor ctor)
        {
            var list = ctor();
            
            do
            {
                var parseResult = parser();
                if (!parseResult.IsSuccess)
                    return parseResult;
                if (parseResult.IsNullStat())
                    break;
        
                list.Add(parseResult.ResultNode);
            } while (true);

            if (list.Nodes.Count == 0)
                return new SuccessParseResult(new NullStat());
            
            return new SuccessParseResult(list);
        }
    }
}