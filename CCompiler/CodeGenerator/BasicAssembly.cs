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
        public TypeDefinition ProgramType { get; }

        public Dictionary<string, MethodReference> MethodReferences { get; } =
            new Dictionary<string, MethodReference>();
        public Dictionary<string, MethodDefinition> MethodDefinitions { get; } =
            new Dictionary<string, MethodDefinition>();

        public Assembly(string assemblyName)
        {
            AssemblyName = assemblyName;
            var assemblyNameDefinition = new AssemblyNameDefinition(AssemblyName, new Version(1, 0, 0, 0));
            AssemblyDefinition = AssemblyDefinition.CreateAssembly(assemblyNameDefinition,
                AssemblyName, ModuleKind.Console);
            var mainModule = AssemblyDefinition.MainModule;
            ProgramType = new TypeDefinition("app", "Program", TypeAttributes.NotPublic | TypeAttributes.Sealed,
                mainModule.TypeSystem.Object) {IsBeforeFieldInit = true};
            
            mainModule.Types.Add(ProgramType);
        }

        public void AddMethod(MethodDefinition methodDefinition) => ProgramType.Methods.Add(methodDefinition);

        public void Save(string directoryPath)
        {
            AssemblyDefinition.Write(directoryPath + AssemblyName + ".exe");
            var streamWriter = new StreamWriter(directoryPath + AssemblyName + ".runtimeconfig.json");
            streamWriter.Write(
                "{\n  \"runtimeOptions\": {\n    \"tfm\": \"netcoreapp3.1\",\n    \"framework\": {\n      \"name\": \"Microsoft.NETCore.App\",\n      \"version\": \"3.1.0\"\n    }\n  }\n}");
            streamWriter.Close();
        }
    }
    
    public class BasicAssembly : Assembly
    {
        public BasicAssembly(string assemblyName) : base(assemblyName)
        {
            var mainModule = AssemblyDefinition.MainModule;
            var writeLineInfo = typeof(Console).GetMethod("WriteLine", new Type[] {typeof(long)});
            var writeLineReference = mainModule.ImportReference(writeLineInfo);
            MethodReferences.Add("WriteLine", writeLineReference);
        }
    }
}