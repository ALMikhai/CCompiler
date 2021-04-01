using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Tests.TokenizerTests
{
    public class Utils
    {
        public static bool CheckCorrect(string fileName)
        {
            var testsDirectory = @"D:\Documents\All my projects\CCompiler\Tests\TokenizerTests\Tests\";
            var inputPath = testsDirectory + @"Input\" + fileName;
            var outputPath = testsDirectory + @"Output\" + fileName;

            var output = RunAndGetOutput(inputPath);
            var correctOutput = new StreamReader(outputPath).ReadToEnd();

            return string.Equals(output ,correctOutput);
        }

        public static string RunAndGetOutput(string path)
        {
            var args = $"\"{path}\" -l";
            using var proc =
                Process.Start(@"D:\Documents\All my projects\CCompiler\CCompiler\bin\Debug\netcoreapp3.1\CCompiler.exe",
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
            else
            {
                throw new ArgumentException("path is not correct");
            }
        }
    }
}
