using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using ExpressionParser.Exceptions;
using FunctionComposeLibrary;
using Jace;
using VariableParsingLibrary;

namespace ExpressionParser
{
    public class ExpressionHandler
    {
        private readonly FunctionComposer _functionComposer;
        private readonly VariableParser _variableParser;
        private readonly List<string> _functionNames;
        private readonly Dictionary<string, List<string>> _variableExpressions;
        private CalculationEngine _calculationEngine;

        public ExpressionHandler(List<string> variableDeclarations, List<string> functionDeclarations)
        {
            _variableParser = new VariableParser(variableDeclarations);
            _variableExpressions = variableDeclarations
                .Select(line => line.Split('='))
                .GroupBy(parts => parts[0].Trim())
                .ToDictionary(group => group.Key, group => group.Select(parts => parts[1].Trim()).ToList());

            _functionComposer = new FunctionComposer();
            _functionNames = new List<string>();
            _calculationEngine = new CalculationEngine();
            ParseFunctions(functionDeclarations);
        }

        public double EvaluateExpression(string expression)
        {
            // Замена переменных и функций в выражении
            string parsedExpression = SubstituteVariablesAndFunctions(expression);
    
            // Вычисление выражения
            double result = Evaluate(parsedExpression);

            // Проверка на бесконечность результата
            if (double.IsInfinity(result))
            {
                throw new EvaluationException("Результат выражения является бесконечностью");
            }

            return result;
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
                var functionName = ExtractFunctionName(signature);
                _functionNames.Add(functionName);
            }
        }

        private string SubstituteVariablesAndFunctions(string expression)
        {
            // Замена переменных на их значения
            foreach (var variable in _variableExpressions.Keys)
            {
                var value = _variableParser.GetVariableValue(variable);
                expression = ReplaceWholeWord(expression, variable, value.ToString(CultureInfo.InvariantCulture));
            }

            // Замена функций на их результаты
            foreach (var functionName in _functionNames)
            {
                while (expression.Contains(functionName + "(", StringComparison.InvariantCulture))
                {
                    int startIndex = expression.IndexOf(functionName + "(", StringComparison.InvariantCulture);
                    int endIndex = FindClosingParenthesis(expression, startIndex + functionName.Length + 1);
                    string argsString = expression.Substring(startIndex + functionName.Length + 1, endIndex - startIndex - functionName.Length - 1);
                    string[] args = SplitArguments(argsString);

                    // Вычисляем аргументы функции
                    double[] evaluatedArgs = new double[args.Length];
                    for (int i = 0; i < args.Length; i++)
                    {
                        evaluatedArgs[i] = Evaluate(args[i].Trim());
                    }

                    // Вызываем функцию и заменяем ее результат в выражении
                    double functionResult = _functionComposer.CallFunction(functionName, evaluatedArgs);
                    expression = expression.Substring(0, startIndex) + functionResult.ToString(CultureInfo.InvariantCulture) + expression.Substring(endIndex + 1);
                }
            }

            return expression;
        }

        private int FindClosingParenthesis(string expression, int startIndex)
        {
            int depth = 0;
            for (int i = startIndex; i < expression.Length; i++)
            {
                if (expression[i] == '(') depth++;
                else if (expression[i] == ')')
                {
                    if (depth == 0) return i;
                    depth--;
                }
            }

            throw new OperationException("Unmatched parenthesis in expression.");
        }

        private double Evaluate(string expression)
        {
            try
            {
                // Вычисление выражения с помощью DataTable.Compute
                var dataTable = new DataTable();
                var value = dataTable.Compute(expression, string.Empty);

                // Преобразование результата в double
                return Convert.ToDouble(value);
            }
            catch (Exception ex) when (ex is InvalidExpressionException || ex is DivideByZeroException)
            {
                throw new EvaluationException($"Ошибка при оценке выражения '{expression}'", ex);
            }
        }



        private string ExtractFunctionName(string signature)
        {
            signature = signature.Replace(" ", "");
            int length = signature.IndexOf("(");
            return length > 0
                ? signature.Substring(0, signature.IndexOf("("))
                : signature;
        }

        private string[] SplitArguments(string argsString)
        {
            List<string> arguments = new List<string>();
            int depth = 0;
            int startIndex = 0;

            for (int i = 0; i < argsString.Length; i++)
            {
                if (argsString[i] == '(') depth++;
                else if (argsString[i] == ')') depth--;

                if (depth == 0 && argsString[i] == ',')
                {
                    arguments.Add(argsString.Substring(startIndex, i - startIndex));
                    startIndex = i + 1;
                }
            }

            arguments.Add(argsString.Substring(startIndex));

            return arguments.ToArray();
        }

        private string ReplaceWholeWord(string originalString, string wordToFind, string replacement)
        {
            string pattern = @"\b" + wordToFind + @"\b";
            return System.Text.RegularExpressions.Regex.Replace(originalString, pattern, replacement);
        }
    }
}
