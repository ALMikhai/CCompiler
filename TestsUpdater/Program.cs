using System;
using System.IO;
using Tests;

namespace TestsUpdater
{
    class Program
    {
        public static void UpdateOutputs(string path, string compileKeys)
        {
            var inputPath = $"{path}.in";
            var outputPath = $"{path}.out";

            var output = Utils.RunAndGetOutput(inputPath, compileKeys);
            var writer = new StreamWriter(outputPath);
            writer.Write(output);
            writer.Close();
        }
        
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Are you sure you want to update your tests? [y, n]");
                var ans = Console.ReadLine();

                if (ans == "y")
                    break;
                if (ans == "n")
                    return;
            }
            
            foreach (var objects in CompilerTester.TestMethodInput)
            {
                UpdateOutputs(objects[0] as string, objects[1] as string);
            }
        }
    }
}