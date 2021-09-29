using System;
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

        public static string RunAndGetOutputGCC(string fileName)
        {
            fileName = char.ToLower(fileName[0]) + fileName.Substring(1);
            var wslPath = $"/mnt/{fileName.Replace("\\", "/").Replace(":", "")}";
            using var procGCC = Process.Start("wsl.exe", $"gcc \"{wslPath}.c\" -o \"{wslPath}GCC.exe\"");
            if (procGCC == null) throw new ArgumentException("path is not correct");
            procGCC.StartInfo.CreateNoWindow = true;
            procGCC.StartInfo.RedirectStandardOutput = true;
            procGCC.StartInfo.UseShellExecute = false;
            procGCC.Start();
            procGCC.WaitForExit();
            
            using var procExe = Process.Start("wsl.exe", $"\"{wslPath}GCC.exe\"");
            if (procExe == null) throw new ArgumentException("path is not correct");
            procExe.StartInfo.CreateNoWindow = true;
            procExe.StartInfo.RedirectStandardOutput = true;
            procExe.StartInfo.UseShellExecute = false;
            procExe.Start();
            var res = procExe.StandardOutput.ReadToEnd();
            procExe.WaitForExit();
            return res;
        }
        
        public static string RunAndGetOutputMyCompiler(string fileName)
        {
            var args = $@"""{fileName}.c"" -net";
            using var proc =
                Process.Start(@"..\..\..\..\CCompiler\bin\Debug\netcoreapp3.1\CCompiler.exe",
                    args);
            if (proc == null) throw new ArgumentException("path is not correct");
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            proc.WaitForExit();

            using var procExe =
                Process.Start("dotnet.exe",
                    $@"""{fileName}.exe""");
            if (procExe == null) throw new ArgumentException("path is not correct");
            procExe.StartInfo.CreateNoWindow = true;
            procExe.StartInfo.RedirectStandardOutput = true;
            procExe.StartInfo.UseShellExecute = false;
            procExe.Start();
            var res = procExe.StandardOutput.ReadToEnd();
            procExe.WaitForExit();
            
            return res;
        }
    }
}