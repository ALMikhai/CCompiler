using System;
using System.Collections.Generic;
using System.Linq;
using CCompiler.Tokenizer;

namespace CCompiler.Parser
{
    public partial class SyntaxParser
    {
        delegate IParseResult Parser();

        private IParseResult ParseBinaryExp<T>(Parser parser, IEnumerable<OperatorType> availableOperators)
            where T : BinaryExp  
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


                var constructor =
                    typeof(T).GetConstructor(new[] {typeof(OperatorToken), typeof(ExpNode), typeof(ExpNode), typeof(Position)});
                if (constructor == null)
                    throw new ArgumentException($"{typeof(T)} not suitable for creating BinaryExp");

                var leftExpNode = left.ResultNode as ExpNode;
                var @object = constructor.Invoke(new object[]
                    {op, leftExpNode, right.ResultNode as ExpNode, leftExpNode.StartNodePosition});
                left = new SuccessParseResult(@object as Node);
            }

            return left;
        }

        private IParseResult ParseList<T>(Parser parser, OperatorType separator)
            where T : List
        {
            var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new ArgumentException($"{typeof(T)} not suitable for creating List");
            
            var list = constructor.Invoke(new object[] {}) as List;
            
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
        
        private IParseResult ParseList<T>(Parser parser)
            where T : List
        {
            var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new ArgumentException($"{typeof(T)} not suitable for creating List");
            
            var list = constructor.Invoke(new object[] {}) as List;
            
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