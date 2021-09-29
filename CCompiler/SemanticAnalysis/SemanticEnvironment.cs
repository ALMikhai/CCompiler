using System;
using System.Collections.Generic;
using CCompiler.CodeGenerator;
using CCompiler.Parser;
using CCompiler.Tokenizer;
using Mono.Cecil;

namespace CCompiler.SemanticAnalysis
{
    public class SemanticEnvironment
    {
        private Stack<EnvironmentSnapshot> _snapshots;
        public Stack<WhileStat.Labels> LoopsLabels { get; }
        private Stack<SymbolType> _returnTypes;

        public SemanticEnvironment()
        {
            LoopsLabels = new Stack<WhileStat.Labels>();
            _returnTypes = new Stack<SymbolType>();
            _snapshots = new Stack<EnvironmentSnapshot>();
            var environmentSnapshot = new EnvironmentSnapshot();
            environmentSnapshot.PushSymbol(new Printf());
            _snapshots.Push(environmentSnapshot);
        }

        public EnvironmentSnapshot GetCurrentSnapshot() => _snapshots.Peek();
        
        public void PushSnapshot() => _snapshots.Push(new EnvironmentSnapshot());
        public void PushSnapshot(EnvironmentSnapshot snapshot)
        {
            PushSnapshot();
            foreach (var pair in snapshot.SymbolTable.GetData())
            {
                _snapshots.Peek().PushSymbol(pair.Value);
            }
            foreach (var pair in snapshot.StructTable.GetData())
            {
                _snapshots.Peek().PushStructType(pair.Value);
            }
        }
        public EnvironmentSnapshot PopSnapshot() => _snapshots.Pop();
        
        public StructType GetStructType(string name)
        {
            foreach (var snapshot in _snapshots)
            {
                if (snapshot.StructTable.Exist(name))
                    return snapshot.StructTable.Get(name);
            }

            throw new ArgumentException($"struct '{name}' is not define");
        }

        public Symbol GetSymbol(string id)
        {
            foreach (var snapshot in _snapshots)
            {
                if (snapshot.SymbolTable.Exist(id))
                    return snapshot.SymbolTable.Get(id);
            }

            throw new ArgumentException($"symbol '{id}' is not define");
        }
        
        public bool InLoop() => LoopsLabels.Count > 0;
        public void PushReturnType(SymbolType type) => _returnTypes.Push(type);
        public SymbolType PopReturnType() => _returnTypes.Pop();
        public SymbolType PeekReturnType() => _returnTypes.Peek();
    }
}
