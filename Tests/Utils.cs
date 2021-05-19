﻿using System;
using System.Diagnostics;
using System.IO;

namespace Tests
{
    public class Utils
    {
        public static (string, string) RunAndGetOutputs(string path, string compileKeys)
        {
            var inputPath = $"{path}.in";
            var outputPath = $"{path}.out";

            var output = RunAndGetOutput(inputPath, compileKeys);
            var correctOutput = new StreamReader(outputPath).ReadToEnd();

            return (output, correctOutput);
        }

        public static string RunAndGetOutput(string path, string compileKeys)
        {
            var args = $"\"{path}\" {compileKeys}";
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