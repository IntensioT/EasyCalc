using FunctionComposeLibrary;
using FunctionComposeLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyCalc.Services
{
    public class FunctionsService
    {
        FunctionComposer _composer;
        Dictionary<long, Delegate> _cachedFunctions = new();

        public FunctionsService()
        {
            _composer = new FunctionComposer();
        }


        public Delegate? CreateFunction(string text)
        {
            if(_cachedFunctions.ContainsKey(text.GetHashCode()))
                return _cachedFunctions[text.GetHashCode()];

            var args = text.Split('=');
            if (args.Length != 2)
                throw new FunctionComposerException($"Incorrect function string: {text}");

            Delegate? result;
            try
            {
                var signature = EnsureValidSignature(args[0]);
                var body = EnsureValidBody(args[1]);
                result = _composer.CreateFunction(signature, body);
                if(result != null)
                    _cachedFunctions.Add(text.GetHashCode(), result);
            }
            catch (FunctionComposerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FunctionComposerException($"Unable to create function: {text}", ex);
            }
            return result;
        }

        public double CallFunction(string text, double[] args)
        {
            _cachedFunctions.TryGetValue(text.GetHashCode(), out var function);
            if (function == null)
                return 0;

            double result;
            try
            {
                var functionResult = function.DynamicInvoke(args);
                result = functionResult != null ? (double)functionResult : 0;
            }
            catch (DivideByZeroException ex)
            {
                throw new FunctionComposerException($"Attempt to divide by zero in function: {text}", ex);
            }
            catch
            {
                throw new FunctionComposerException($"Unable to run funciton: {text}. Try to check its body and your params one more time");
            }
            return result;
        }

        public Delegate? GetFunction(string text)
        {
            _cachedFunctions.TryGetValue(text.GetHashCode(), out var result);
            return result;
        }

        public bool DeleteFunction(string text)
        {
            return _cachedFunctions.Remove(text.GetHashCode());
        }

        private bool CheckSignatureValidation(string signature)
        {
            string pattern = @"^[a-zA-Z][a-zA-Z0-9_]*\(([a-zA-Z0-9_]+,\s*)*[a-zA-Z0-9_]+\)$";
            return Regex.IsMatch(signature, pattern);
        }

        private string EnsureValidSignature(string signature)
        {
            if (!CheckSignatureValidation(signature))
            {
                if (signature.IndexOf(")") == -1)
                {
                    signature += ")";
                    if (!CheckSignatureValidation(signature))
                        throw new FunctionComposerException($"Incorrect function signature {signature}");
                }
            }
            return signature.Replace(" ", "");
        }

        private string EnsureValidBody(string body)
        {
            if (!CheckBracketsOrder(body))
                throw new FunctionComposerException($"Incorrect brackets order in {body}");
            int missedBracketsAmount = CountMissedBrackets(body);
            for (int i = 0; i < missedBracketsAmount; i++)
                body += ')';
            return body.Replace(" ", "");
        }

        private int CountMissedBrackets(string body)
        {
            int k = 0;
            foreach (var ch in body)
            {
                if (ch == '(')
                    k++;
                if (ch == ')')
                    k--;
            }
            return k;
        }
        private bool CheckBracketsOrder(string body)
        {
            var stack = new Stack<char>();
            foreach (var ch in body)
            {
                if (ch == '(')
                    stack.Push(ch);
                else if (ch == ')')
                {
                    if (stack.Count == 0)
                        return false;
                    stack.Pop();
                }
            }
            return true;
        }
    }
}
