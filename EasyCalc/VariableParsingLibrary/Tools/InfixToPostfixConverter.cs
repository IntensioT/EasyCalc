using System.Globalization;

namespace VariableParsingLibrary.Tools;

public class InfixToPostfixConverter
{
    private readonly Dictionary<string, int> _precedence = new ()
    {
        { "+", 1 },
        { "-", 1 },
        { "*", 2 },
        { "/", 2 },
    };
    
    public List<string> Convert(List<string> infixTokens)
    {
        var output = new List<string>();
        var operators = new Stack<string>();

        foreach (var token in infixTokens)
        {
            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out _) || token.All(char.IsLetter))
            {
                output.Add(token);
            }
            else if (_precedence.ContainsKey(token))
            {
                while (operators.Count > 0 && _precedence.ContainsKey(operators.Peek()) && _precedence[operators.Peek()] >= _precedence[token])
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }
                operators.Pop();
            }
        }

        while (operators.Count > 0)
        {
            output.Add(operators.Pop());
        }

        return output;
    }
}
