using System;
using CCompiler.SemanticAnalysis;
using CCompiler.Tokenizer;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CCompiler.CodeGenerator
{
    public class Printf : FuncSymbol
    {
        private static EnvironmentSnapshot _arguments;
        static Printf()
        {
            _arguments = new EnvironmentSnapshot();
            _arguments.SymbolTable.Push("s",
                new VarSymbol("s", new SymbolType(false, false, SymbolTypeKind.STRING), new Position(0, 0)));
            _arguments.SymbolTable.Push("o",
                new VarSymbol("o", new SymbolType(false, false, SymbolTypeKind.INT), new Position(0, 0)));
        }
        
        public Printf() : base("printf", new FuncType(new SymbolType(false, false, SymbolTypeKind.VOID), _arguments), new Position(0, 0))
        {
            IsDefined = true;
        }

        public override void Generate(ref Assembly assembly, ref SemanticEnvironment environment)
        {
            var funcType = Type as FuncType;
            var retType = funcType.ReturnType;
            var methodDefinition = new MethodDefinition(Id, MethodAttributes.Public | MethodAttributes.Static,
                retType.ToTypeReference(ref assembly)) {Body = {InitLocals = true}};
            var mainModule = assembly.AssemblyDefinition.MainModule;
            var writeLineInfo = typeof(Console).GetMethod("Write", new Type[] {typeof(string), typeof(object)});
            var writeLineReference = mainModule.ImportReference(writeLineInfo);
            var il = methodDefinition.Body.GetILProcessor();

            foreach (var argument in funcType.GetArguments())
            {
                var parameterDefinition = new ParameterDefinition(argument.Id, ParameterAttributes.None,
                    argument.Type.ToTypeReference(ref assembly));
                ((VarSymbol) argument).ParameterDefinition = parameterDefinition;
                methodDefinition.Parameters.Add(parameterDefinition);
            }
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Box, assembly.AssemblyDefinition.MainModule.TypeSystem.Int64);
            il.Emit(OpCodes.Call, writeLineReference);
            il.Emit(OpCodes.Ret);
            
            environment.MethodDefinitions.Add(Id, methodDefinition);
            assembly.AddMethod(methodDefinition);
        }
        
        public override string ToString() => $"{Type.GetFullName()}" + $":: {Id}";
    }
}