using System;
using System.Diagnostics;
using System.IO;

namespace Tests.TokenizerTests
{
    public class Utils
    {
        public static bool CheckCorrect(string fileName)
        {
            var testsDirectory = @"..\..\..\TokenizerTests\Tests\";
            var inputPath = $"{testsDirectory}{fileName}.in";
            var outputPath = $"{testsDirectory}{fileName}.out";

            var output = RunAndGetOutput(inputPath);
            var correctOutput = new StreamReader(outputPath).ReadToEnd();

            return string.Equals(output, correctOutput);
        }

        public static string RunAndGetOutput(string path)
        {
            var args = $"\"{path}\" -l";
            using var proc =
                Process.Start(@"..\..\..\..\CCompiler\bin\Debug\netcoreapp3.1\CCompiler.exe",
                    args);
            if (proc != null)
            {
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                var res = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                return res;
            }

            throw new ArgumentException("path is not correct");
        }
    }
}