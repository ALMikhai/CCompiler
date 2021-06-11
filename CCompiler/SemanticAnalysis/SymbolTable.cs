using System.Collections.Generic;
using System.Linq;

namespace CCompiler.SemanticAnalysis
{
    public class SymbolTable
    {
        private Stack<Dictionary<string, Symbol>> _symbolsStack;
        
        public SymbolTable()
        {
            _symbolsStack = new Stack<Dictionary<string, Symbol>>();
        }

        public void AddSymbolTable() => _symbolsStack.Push(new Dictionary<string, Symbol>());
        public void RemoveSymbolTable() => _symbolsStack.Pop();
        public void PushSymbol(Symbol symbol) => _symbolsStack.Peek().Add(symbol.Id, symbol);
        public bool SymbolExist(string id) => _symbolsStack.Any(table => table.ContainsKey(id));
        
        public T GetSymbol<T>(string id)
            where T : Symbol
        {
            if (!SymbolExist(id))
                return null;

            var table = _symbolsStack.First(dictionary => dictionary.ContainsKey(id));
            return table[id] as T;
        }
    }
}