using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ExpressionParser.Exceptions;
using FunctionComposeLibrary;

namespace ExpressionParser
{
    public class ExpressionHandler
    {
        private readonly FunctionComposer _functionComposer;
        private readonly VariableParser _variableParser;
        private readonly List<string> _functionNames;

        public ExpressionHandler(FunctionComposer functionComposer, VariableParser variableParser)
        {
            _functionComposer = functionComposer;
            _variableParser = variableParser;
            _functionNames = new List<string>();
        }

        public double EvaluateExpression(string expression, List<string> variablesList, List<string> functionsList)
        {
            // Парсинг переменных и функций
            ParseVariables(variablesList);
            ParseFunctions(functionsList);

            // Замена переменных и функций в выражении
            string parsedExpression = SubstituteVariablesAndFunctions(expression);

            // Вычисление выражения
            double result = Evaluate(parsedExpression);

            return result;
        }

        private void ParseVariables(List<string> variablesList)
        {
            foreach (var line in variablesList)
            {
                var parts = line.Split('=');
                if (parts.Length != 2)
                {
                    throw new OperationException($"Invalid input line: {line}");
                }

                var variable = parts[0].Trim();
                var expression = parts[1].Trim();
                _variableParser.SetVariable(variable, expression);
            }
        }

        private void ParseFunctions(List<string> functionsList)
        {
            foreach (var function in functionsList)
            {
                var parts = function.Split('=');
                if (parts.Length != 2)
                {
                    throw new OperationException($"Invalid function definition: {function}");
                }

                var signature = parts[0].Trim();
                var body = parts[1].Trim();

                _functionComposer.CreateFunction(signature, body);
                var functionName = _functionComposer.GetFunctionName(signature);
                _functionNames.Add(functionName);
            }
        }

        private string SubstituteVariablesAndFunctions(string expression)
        {
            // Замена переменных на их значения
            foreach (var variable in _variableParser.GetAllVariables())
            {
                expression = expression.Replace(variable.Key, variable.Value.ToString());
            }

            // Замена функций на их результаты
            foreach (var functionName in _functionNames)
            {
                while (expression.Contains(functionName + "(", StringComparison.InvariantCulture))
                {
                    var startIndex = expression.IndexOf(functionName + "(", StringComparison.InvariantCulture);
                    var endIndex = expression.IndexOf(')', startIndex);
                    var argsString = expression.Substring(startIndex + functionName.Length + 1, endIndex - startIndex - functionName.Length - 1);
                    var args = argsString.Split(',').Select(arg => Evaluate(arg.Trim())).ToArray();

                    var functionResult = _functionComposer.CallFunction(functionName, args);
                    expression = expression.Substring(0, startIndex) + functionResult.ToString() + expression.Substring(endIndex + 1);
                }
            }

            return expression;
        }

        private double Evaluate(string expression)
        {
            DataTable table = new DataTable();
            return Convert.ToDouble(table.Compute(expression, string.Empty));
        }
    }
}
