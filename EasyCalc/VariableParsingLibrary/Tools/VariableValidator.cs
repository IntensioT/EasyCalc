using System.Globalization;
using VariableParsingLibrary.Exceptions;

namespace VariableParsingLibrary.Tools;

public class VariableValidator
{
    private Tokenizer _tokenizer = new Tokenizer();
    private InfixToPostfixConverter _infixToPostfixConverter = new InfixToPostfixConverter();
    
    public void ValidateVariableName(string variable)
    {
        if (string.IsNullOrWhiteSpace(variable) || !char.IsLetter(variable[0]) || !variable.All(c => char.IsLetterOrDigit(c) || c == '_'))
        {
            throw new VariableParserException($"Invalid variable name: {variable}");
        }
    }
    
    public void ValidateExpressions(Dictionary<string, string> variableExpressions)
    {
        foreach (var variable in variableExpressions.Keys)
        {
            ValidateExpression(variable, variableExpressions[variable], variableExpressions);
        }
    }

    private void ValidateExpression(string variable, string expression, Dictionary<string, string> variableExpressions)
    {
        var tokens = _tokenizer.Tokenize(expression);
        var postfix = _infixToPostfixConverter.Convert(tokens);

        foreach (var token in postfix)
        {
            if (char.IsLetter(token[0]) && !variableExpressions.ContainsKey(token) && !double.TryParse(token, NumberStyles.Float | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out _))
            {
                throw new VariableParserException($"Undefined variable {token} in expression for {variable}");
            }
        }
    }
}