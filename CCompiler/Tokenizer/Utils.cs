using System.Text;

namespace CCompiler.Tokenizer
{
    public class Utils
    {
        public static bool IsHex(char ch)
        {
            return char.IsDigit(ch) || ch == 'A' || ch == 'a' || ch == 'B' || ch == 'b' || ch == 'C' || ch == 'c' ||
                   ch == 'D' || ch == 'd' || ch == 'E' || ch == 'e' || ch == 'F' || ch == 'f';
        }

        public static bool IsOct(char ch)
        {
            return char.IsDigit(ch) && ch != '9' && ch != '8';
        }

        public static string AddTab(string str)
        {
            var result = new StringBuilder();
            foreach (var s in str.Split('\n'))
                result.Append($"\t{s}\n");
            
            return result.ToString();
        } 
    }
}