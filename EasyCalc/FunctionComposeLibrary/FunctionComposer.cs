﻿using EasyCalc.Exceptions;
using System.Linq.Expressions;
using System.Text;

namespace FunctionComposeLibrary
{
    public class FunctionComposer
    {
        Dictionary<string, Delegate> _functions = new();

        private Dictionary<char, Operation> operationsDictionary = new()
        {
            ['('] = new Operation(null, 0),
            ['+'] = new Operation(Expression.Add, 1),
            ['-'] = new Operation(Expression.Subtract, 1),
            ['*'] = new Operation(Expression.Multiply, 2),
            ['/'] = new Operation(Expression.Divide, 2),
            [')'] = new Operation(null, 10),
        };

        IEnumerable<char> operations => operationsDictionary.Keys;
        const char _divider = ',';

        public double CallFunction(string name, double[] args)
        {
            _functions.TryGetValue(GetFunctionName(name), out var function);
            if (function == null)
                return 0;

            var result = function.DynamicInvoke(args);
            return result != null ? (double)result : 0;
        }

        public Delegate? GetFunction(string name)
        {
            _functions.TryGetValue(GetFunctionName(name), out var result);
            return result;
        }

        public Delegate? CreateFunction(string signature, string body)
        {
            var name = GetFunctionName(signature);
            var result = GetFunction(name);
            if (result != null)
                return result;

            var parameters = GetSignatureParameters(signature);
            var args = Expression.Parameter(typeof(double[]), "args");

            body = ParseToPostfixForm(body);
            var stack = new Stack<Expression>();
            for (int i = 0; i < body.Length; i++)
            {
                if (operations.Contains(body[i]))
                {
                    var right = stack.Pop();
                    var left = stack.Pop();
                    if (operationsDictionary.TryGetValue(body[i], out var operation))
                    {
                        if (operation.ExpressionHandler != null)
                            stack.Push(operation.ExpressionHandler(left, right));
                    }
                }
                else
                {
                    var strVariable = ReadVariableLiteral(body, ref i);
                    if (double.TryParse(strVariable, out var variable))
                    {
                        stack.Push(Expression.Constant(variable, variable.GetType()));
                    }
                    else if (parameters?.Contains(strVariable) ?? false)
                    {
                        int ind = 0;
                        while (parameters[ind] != strVariable) ind++;
                        stack.Push(Expression.ArrayIndex(args, Expression.Constant(ind)));
                    }
                    else
                    {
                        if (strVariable != "")
                            throw new InvalidOperationException($"Unable to find lexem: {strVariable}");
                    }

                }
            }

            result = Expression.Lambda(stack.Pop(), args).Compile();
            _functions.Add(name, result);
            return result;
        }

        private string ReadVariableLiteral(string expression, ref int ind)
        {
            var result = "";
            while (ind < expression.Length && expression[ind] != _divider && !operations.Contains(expression[ind]))
            {
                result += expression[ind];
                ind++;
            }

            //decrease ind if we go to next lexema, outer method has to increase it by itself
            if (ind < expression.Length && expression[ind] != _divider)
                ind--;
            return result;
        }

        private string ParseToPostfixForm(string body)
        {
            body = body.Replace(" ", "");
            var output = new StringBuilder();
            var length = body.Length;
            var stack = new Stack<char>();
            for (int i = 0; i < length; i++)
            {
                if (body[i] == '(')
                    stack.Push(body[i]);
                else if (body[i] == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                        output.Append(stack.Pop());
                    stack.Pop();
                }
                else if (operations.Contains(body[i]))
                {
                    while (stack.Count > 0 && operationsDictionary[stack.Peek()].Priority >=
                           operationsDictionary[body[i]].Priority)
                        output.Append(stack.Pop());
                    stack.Push(body[i]);
                }
                else
                {
                    var strNumber = ReadVariableLiteral(body, ref i);
                    output.Append(strNumber + _divider);
                }
            }

            while (stack.Count > 0)
                output.Append(stack.Pop());
            return output.ToString();
        }

        private string[]? GetSignatureParameters(string signature)
        {
            signature = signature.Replace(" ", "");
            var startInd = signature.IndexOf("(") + 1;
            var endInd = signature.IndexOf(")");
            signature = signature.Substring(startInd, endInd - startInd);
            var result = signature.Split(',');
            return result;
        }

        private string GetFunctionName(string signature)
        {
            signature = signature.Replace(" ", "");
            var length = signature.IndexOf("(");
            return length > 0
                ? signature.Substring(0, signature.IndexOf("("))
                : signature;
        }
    }
}
