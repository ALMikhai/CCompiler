using System;
using System.IO;
using System.Linq;
using System.Xml;
using CCompiler.Parser;
using CCompiler.SemanticAnalysis;
using CCompiler.Tokenizer;

namespace CCompiler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
                // TODO Display help info.
                return;

            try
            {
                var tokenizer = new Tokenizer.Tokenizer(args[0]);
                if (args.Contains("-l"))
                {
                    var token = tokenizer.Get();
                    while (token.TokenType != TokenType.EOF)
                    {
                        Console.WriteLine(token);
                        token = tokenizer.Get();
                    }
                }
                
                if (args.Contains("-p"))
                {
                    var parser = new SyntaxParser(tokenizer, SyntaxParserType.EXP);
                    Console.WriteLine(parser.SyntaxTree);
                }
                
                if (args.Contains("-ps"))
                {
                    var parser = new SyntaxParser(tokenizer, SyntaxParserType.STAT);
                    Console.WriteLine(parser.SyntaxTree);
                }

                if (args.Contains("-pu"))
                {
                    var parser = new SyntaxParser(tokenizer, SyntaxParserType.UNIT);
                    Console.WriteLine(parser.SyntaxTree);
                }

                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"error: file {e.FileName} not found");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}