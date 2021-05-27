using System;
using System.IO;
using System.Linq;
using CCompiler.Parser;
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
                }
                
                if (args.Contains("-ps"))
                {
                    var parser = new SyntaxParser(tokenizer, SyntaxParserType.STAT);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"error: file {e.FileName} not found");
            }
            catch (TokenizerException e)
            {
                Console.WriteLine(e);
            }
            catch (ParserException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}