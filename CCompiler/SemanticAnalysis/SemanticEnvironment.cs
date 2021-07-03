using System;
using System.Collections.Generic;
using CCompiler.Tokenizer;

namespace CCompiler.SemanticAnalysis
{
    public class SemanticEnvironment
    {
        private Stack<EnvironmentSnapshot> _snapshots;
        private int _nestedLoopsCount;
        private Stack<SymbolType> _returnTypes;

        public SemanticEnvironment()
        {
            _nestedLoopsCount = 0;
            _returnTypes = new Stack<SymbolType>();
            _snapshots = new Stack<EnvironmentSnapshot>();
            var environmentSnapshot = new EnvironmentSnapshot();

            var stringArgument = new EnvironmentSnapshot();
            stringArgument.SymbolTable.Push("s",
                new VarSymbol("s", new SymbolType(false, false, SymbolTypeKind.STRING), new Position(0, 0)));

            var printfSymbol = new FuncSymbol("printf", new FuncType(new SymbolType(false, false, SymbolTypeKind.VOID), stringArgument),
                new Position(0, 0));
            var scanfSymbol = new FuncSymbol("scanf", new FuncType(new SymbolType(false, false, SymbolTypeKind.VOID), stringArgument),
                new Position(0, 0));

            environmentSnapshot.SymbolTable.Push("printf", printfSymbol);
            environmentSnapshot.SymbolTable.Push("scanf", scanfSymbol);
            
            _snapshots.Push(environmentSnapshot);
        }

        public void PushSymbol(Symbol symbol)
        {
            if (SymbolExist(symbol.Id))
                throw new SemanticException($"redeclaration of '{symbol.Id}'", symbol.DeclPosition);

            _snapshots.Peek().SymbolTable.Push(symbol.Id, symbol);
        }

        public bool SymbolExist(string id) => _snapshots.Peek().SymbolTable.Exist(id);

        public void PushSnapshot() => _snapshots.Push(new EnvironmentSnapshot());

        public void PushSnapshot(EnvironmentSnapshot snapshot)
        {
            PushSnapshot();
            foreach (var pair in snapshot.SymbolTable.GetData())
            {
                PushSymbol(pair.Value);
            }
            foreach (var pair in snapshot.StructTable.GetData())
            {
                PushStructType(pair.Value);
            }
        }

        public EnvironmentSnapshot PopSnapshot() => _snapshots.Pop();

        public void PushStructType(StructType type)
        {
            if (StructExist(type.Name))
                throw new SemanticException($"redeclaration of '{type.Name}'", type.DeclPosition);
            
            _snapshots.Peek().StructTable.Push(type.Name, type);
        }

        public bool StructExist(string name) => _snapshots.Peek().StructTable.Exist(name);

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

        public void PushSnapshotAsSymbol(EnvironmentSnapshot snapshot) => PushSymbol(new SnapshotSymbol(snapshot));
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
