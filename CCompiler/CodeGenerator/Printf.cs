using System;
using CCompiler.SemanticAnalysis;
using CCompiler.Tokenizer;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CCompiler.CodeGenerator
{
    public class PrintfString : FuncSymbol
    {
        private static EnvironmentSnapshot _argument = new EnvironmentSnapshot();

        static PrintfString()
        {
            _argument.SymbolTable.Push("s",
                new VarSymbol("s", new SymbolType(false, false, SymbolTypeKind.STRING), new Position(0, 0)));
        }
            
        public PrintfString() : base("printf_string", new FuncType(new SymbolType(false, false, SymbolTypeKind.VOID), _argument), new Position(0, 0))
        {
            IsDefined = true;
        }

        public override void Generate(ref Assembly assembly, ref SemanticEnvironment environment)
        {
            var funcType = Type as FuncType;
            var retType = funcType.ReturnType;
            var function = new MethodDefinition(Id, MethodAttributes.Public | MethodAttributes.Static,
                retType.ToTypeReference(ref assembly)) {Body = {InitLocals = true}};
            var mainModule = assembly.AssemblyDefinition.MainModule;
            var writeLineInfo = typeof(Console).GetMethod("WriteLine", new Type[] {typeof(string)});
            var writeLineReference = mainModule.ImportReference(writeLineInfo);
            var il = function.Body.GetILProcessor();
            
            foreach (var argument in funcType.GetArguments())
            {
                var parameterDefinition = new ParameterDefinition(argument.Key, ParameterAttributes.None,
                    argument.Value.Type.ToTypeReference(ref assembly));
                ((VarSymbol) argument.Value).ParameterDefinition = parameterDefinition;
                function.Parameters.Add(parameterDefinition);
            }
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, writeLineReference);
            il.Emit(OpCodes.Ret);
            
            environment.MethodDefinitions.Add(Id, function);
            assembly.AddMethod(function);
        }
    }
    
    public class PrintfInt : FuncSymbol
    {
        private static EnvironmentSnapshot _argument = new EnvironmentSnapshot();

        static PrintfInt()
        {
            _argument.SymbolTable.Push("i",
                new VarSymbol("i", new SymbolType(false, false, SymbolTypeKind.INT), new Position(0, 0)));
        }
        
        public PrintfInt() : base("printf_int", new FuncType(new SymbolType(false, false, SymbolTypeKind.VOID), _argument), new Position(0, 0))
        {
            IsDefined = true;
        }

        public override void Generate(ref Assembly assembly, ref SemanticEnvironment environment)
        {
            var funcType = Type as FuncType;
            var retType = funcType.ReturnType;
            var function = new MethodDefinition(Id, MethodAttributes.Public | MethodAttributes.Static,
                retType.ToTypeReference(ref assembly)) {Body = {InitLocals = true}};
            var mainModule = assembly.AssemblyDefinition.MainModule;
            var writeLineInfo = typeof(Console).GetMethod("WriteLine", new Type[] {typeof(long)});
            var writeLineReference = mainModule.ImportReference(writeLineInfo);
            var il = function.Body.GetILProcessor();
            
            foreach (var argument in funcType.GetArguments())
            {
                var parameterDefinition = new ParameterDefinition(argument.Key, ParameterAttributes.None,
                    argument.Value.Type.ToTypeReference(ref assembly));
                ((VarSymbol) argument.Value).ParameterDefinition = parameterDefinition;
                function.Parameters.Add(parameterDefinition);
            }
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, writeLineReference);
            il.Emit(OpCodes.Ret);
            
            environment.MethodDefinitions.Add(Id, function);
            assembly.AddMethod(function);
        }
    }
}