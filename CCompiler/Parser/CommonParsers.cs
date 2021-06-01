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

        private IParseResult ParseBinaryExp(Parser parser, ExpCtor ctor, List<OperatorType> availableOperators)
        {
            var left = parser();
            if (!left.IsSuccess)
                return left;

            while (availableOperators.FirstOrDefault(AcceptOp) != OperatorType.NONE)
            {
                var operation = _acceptedToken;
                var right = parser();
                if (!right.IsSuccess)
                    return right;

                left = new SuccessParseResult(ctor(operation as OperatorToken, left.ResultNode,
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
                if (parseResult.ResultNode is NullStat) // TODO ??
                    break;

                list.Add(parseResult.ResultNode);
            } while (AcceptOp(separator));

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
                if (parseResult.IsSuccess && parseResult.ResultNode is NullStat)
                    break;
                if (!parseResult.IsSuccess)
                    return parseResult;
        
                list.Add(parseResult.ResultNode);
            } while (true);

            if (list.Nodes.Count == 0)
                return new SuccessParseResult(new NullStat());
            
            return new SuccessParseResult(list);
        }
    }
}