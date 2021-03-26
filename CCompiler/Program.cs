using System;
using System.IO;
using CCompiler.Tokenizer;

namespace CCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var tokenizer =
                    new Tokenizer.Tokenizer(@"D:\Documents\All my projects\CCompiler\CCompiler\TestProgram.txt");
                Token token;
                do
                {
                    token = tokenizer.Get();
                    Console.WriteLine(token);
                } while (token.Type != TokenType.EOF);
            }
            catch (TokenizerException e)
            {
                Console.WriteLine(e);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
