﻿using System;
using System.Collections.Generic;
using CCompiler.CodeGenerator;
using CCompiler.Tokenizer;
using Mono.Cecil;

namespace CCompiler.SemanticAnalysis
{
    public class SemanticEnvironment
    {
        private Stack<EnvironmentSnapshot> _snapshots;
        private int _nestedLoopsCount;
        private Stack<SymbolType> _returnTypes;
        public Dictionary<string, MethodDefinition> MethodDefinitions { get; } =
            new Dictionary<string, MethodDefinition>();

        public SemanticEnvironment()
        {
            _nestedLoopsCount = 0;
            _returnTypes = new Stack<SymbolType>();
            _snapshots = new Stack<EnvironmentSnapshot>();
            var environmentSnapshot = new EnvironmentSnapshot();
            environmentSnapshot.PushSymbol(new PrintfString());
            environmentSnapshot.PushSymbol(new PrintfInt());
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
        
        public bool InLoop() => _nestedLoopsCount > 0;
        public void LoopEntry() => ++_nestedLoopsCount;
        public void LoopExit()
        {
            --_nestedLoopsCount;
            if (_nestedLoopsCount < 0)
                throw new Exception("exit from the loop which does not exist");
        }

        public void PushReturnType(SymbolType type) => _returnTypes.Push(type);
        public SymbolType PopReturnType() => _returnTypes.Pop();
        public SymbolType PeekReturnType() => _returnTypes.Peek();
    }
}
