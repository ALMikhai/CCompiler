﻿using System;
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

                if (args.Contains("-s"))
                {
                    var parser = new SyntaxParser(tokenizer, SyntaxParserType.UNIT);
                    var syntaxTree = parser.SyntaxTree;
                    Console.WriteLine(syntaxTree);
                    var environment = new SemanticEnvironment();
                    syntaxTree.CheckSemantic(ref environment);
                    Console.WriteLine(environment.SymbolTable);
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