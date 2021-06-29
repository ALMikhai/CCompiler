using System.Collections.Generic;
using System.Text;

namespace CCompiler.SemanticAnalysis
{
    public class SymbolTable
    {
        private Dictionary<string, Symbol> _symbols;
        
        public SymbolTable()
        {
            _symbols = new Dictionary<string, Symbol>();
        }

        public void PushSymbol(Symbol symbol) => _symbols.Add(symbol.Id, symbol);
        
        public bool SymbolExist(string id) => _symbols.ContainsKey(id);

        public override string ToString()
        {
            var symbols = new StringBuilder();
            foreach (var symbol in _symbols)
            {
                symbols.Append(symbol.Value + "\n");
            }

            return "Symbol table {\n" + symbols.ToString() + "}";
        }
    }
}