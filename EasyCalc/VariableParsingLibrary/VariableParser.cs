using System.Globalization;
using System.Xml.Schema;
using VariableParsingLibrary.Exceptions;
using VariableParsingLibrary.Tools;
using Validator = System.ComponentModel.DataAnnotations.Validator;

namespace VariableParsingLibrary;

public class VariableParser
{
    private Dictionary<string, string> _variableExpressions = new Dictionary<string, string>();
    private Dictionary<string, double> _variableValues = new Dictionary<string, double>();
    private HashSet<string> _evaluating = new HashSet<string>();

    private Tokenizer _tokenizer = new Tokenizer();
    private InfixToPostfixConverter _infixToPostfixConverter = new InfixToPostfixConverter();
    private VariableValidator _validator = new VariableValidator();
    
    public VariableParser(List<string> variableDeclarations)
    {
        foreach (var line in variableDeclarations)
        {
            var parts = line.Split('=');
            if (parts.Length != 2)
            {
                throw new VariableParserException($"Invalid input line: {line}");
            }

            var variable = parts[0].Trim();
            _validator.ValidateVariableName(variable);
            var expression = parts[1].Trim();
            
            _variableExpressions[variable] = expression;
        }
        
        _validator.ValidateExpressions(_variableExpressions);
    }
    
    public double GetVariableValue(string variable)
    {
        if (!_variableValues.ContainsKey(variable))
        {
            if (_evaluating.Contains(variable))
            {
                throw new VariableParserException($"Circular dependency detected for variable {variable}");
            }

            _evaluating.Add(variable);
            
            var expression = _variableExpressions[variable];
            var tokens = _tokenizer.Tokenize(expression);
            var postfix = _infixToPostfixConverter.Convert(tokens);
            
            var value = EvaluatePostfix(postfix);
            _variableValues[variable] = value;
            
            _evaluating.Remove(variable);
        }

        return _variableValues[variable];
    }

    public bool ContainsVariable(string variable)
    {
        return _variableExpressions.ContainsKey(variable);
    }
    
    private double EvaluatePostfix(List<string> postfixTokens)
    {
        var stack = new Stack<double>();

        foreach (var token in postfixTokens)
        {
            if (double.TryParse(token, NumberStyles.Float | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var number))
            {
                stack.Push(number);
            }
            else if (ContainsVariable(token))
            {
                stack.Push(GetVariableValue(token));
            }
            else
            {
                var right = stack.Pop();
                var left = stack.Pop();
                
                var result = token switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => left / right,
                    _ => throw new VariableParserException($"Unexpected operator: {token}")
                };
                
                stack.Push(result);
            }
        }

        return stack.Pop();
    }
}