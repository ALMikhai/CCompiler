using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;

namespace CCompiler.CodeGenerator
{
    public class Assembly
    {
        public string AssemblyName { get; }
        public AssemblyDefinition AssemblyDefinition { get; }
        private TypeDefinition _programType;

        public Assembly(string assemblyName)
        {
            AssemblyName = assemblyName;
            var assemblyNameDefinition = new AssemblyNameDefinition(AssemblyName, new Version(1, 0, 0, 0));
            AssemblyDefinition = AssemblyDefinition.CreateAssembly(assemblyNameDefinition,
                AssemblyName, ModuleKind.Console);
            var mainModule = AssemblyDefinition.MainModule;
            _programType = new TypeDefinition("app", "Program", TypeAttributes.NotPublic | TypeAttributes.Sealed,
                mainModule.TypeSystem.Object) {IsBeforeFieldInit = true};
            
            mainModule.Types.Add(_programType);
        }

        public void AddMethod(MethodDefinition methodDefinition) => _programType.Methods.Add(methodDefinition);

        public void Save(string directoryPath)
        {
            AssemblyDefinition.Write(directoryPath + AssemblyName + ".exe");
            var streamWriter = new StreamWriter(directoryPath + AssemblyName + ".runtimeconfig.json");
            streamWriter.Write(
                "{\n  \"runtimeOptions\": {\n    \"tfm\": \"netcoreapp3.1\",\n    \"framework\": {\n      " +
                "\"name\": \"Microsoft.NETCore.App\",\n      \"version\": \"3.1.0\"\n    }\n  }\n}");
            streamWriter.Close();
        }
    }
}