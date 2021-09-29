using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CompilerTester
    {
        public const string TestsDirectory = @"..\..\..\Tests";
        public const string ProgramsPath = @"..\..\..\ProgramTests";

        public static IEnumerable<object[]> TestMethodInput
        {
            get
            {
                var res = new List<object[]>();
                var directories = new DirectoryInfo(TestsDirectory).GetDirectories();
                foreach (var directory in directories)
                {
                    var keys = "";
                    try
                    {
                        keys = new StreamReader(directory.FullName + "\\.keys").ReadToEnd();
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }

                    var directoryInfo = new DirectoryInfo(directory.FullName + "\\Files");
                    var files = directoryInfo.GetFiles("*.in");
                    res.AddRange(files.Select(file => new object[]
                        {file.Directory + "\\" + Path.GetFileNameWithoutExtension(file.Name), keys}));
                }

                return res;
            }
        }

        public static IEnumerable<object[]> ProgramNames
        {
            get
            {
                var res = new List<object[]>();
                var files = new DirectoryInfo(ProgramsPath).GetFiles("*.c");
                res.AddRange(files.Select(file => new object[]
                    {file.Directory + "\\" + Path.GetFileNameWithoutExtension(file.Name)}));
                return res;
            }
        }

        [TestMethod]
        [DynamicData(nameof(TestMethodInput))]
        public void Test(string fileName, string compileKeys)
        {
            var outputs = Utils.RunAndGetOutputs(fileName, compileKeys);
            Assert.AreEqual(outputs.Item2, outputs.Item1);
        }

        [TestMethod]
        [DynamicData(nameof(ProgramNames))]
        public void TestProgram(string fileName)
        {
            var outputCompiler = Utils.RunAndGetOutputMyCompiler(fileName);
            var outputGCC = Utils.RunAndGetOutputGCC(fileName);
            Assert.AreEqual(outputCompiler, outputGCC);
        }
    }
}