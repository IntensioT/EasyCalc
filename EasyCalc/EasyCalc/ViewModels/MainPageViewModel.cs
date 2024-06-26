﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyCalc.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpressionParser;
using static System.Runtime.InteropServices.JavaScript.JSType;
using VariableParsingLibrary.Exceptions;

namespace EasyCalc
{
    public partial class MainPageViewModel : ObservableObject
    {
        public ObservableCollection<FunctionUI> Functions { get; } = new ObservableCollection<FunctionUI>();
        public ObservableCollection<VariableUI> Variables { get; } = new ObservableCollection<VariableUI>();

        public ICommand AddFuncCommand { get; }
        public ICommand RemoveFuncCommand { get; }

        public ICommand AddVarCommand { get; }
        public ICommand RemoveVarCommand { get; }

        private int OpenParenthesesCount = 0;


        [ObservableProperty]
        private string _expressionDisplay = string.Empty;

        [ObservableProperty]
        private string _resultDisplay = string.Empty;


        [RelayCommand]
        public void HandleButtonPress(string buttonText)
        {
            if (buttonText == "AC")
            {
                ExpressionDisplay = string.Empty;
                ResultDisplay = string.Empty;
            }

            else if (buttonText == "=")
            {
                EvaluateExpression();
            }

            //else if (int.TryParse(buttonText, out var _) || buttonText == "%" || buttonText == ".")
            //{
            //    var ch = buttonText[0];
            //    ExpressionDisplay += ch;
            //}
            else if (int.TryParse(buttonText, out var _) || buttonText == "%" || buttonText == "." || buttonText == "(" || buttonText == ")")
            {
                var ch = buttonText[0];
                if (ch == '(')
                {
                    // Обработка открывающей скобки
                    ExpressionDisplay += ch;
                    OpenParenthesesCount++;
                }
                else if (ch == ')')
                {
                    // Обработка закрывающей скобки
                    if (OpenParenthesesCount > 0)
                    {
                        ExpressionDisplay += ch;
                        OpenParenthesesCount--;
                    }
                    else
                    {
                        DisplayMessage("Error", "Too many closing parentheses.", "ok");
                        //ResultDisplay = "Too many closing parentheses.";
                    }
                }
                else
                {
                    // Обработка других символов
                    ExpressionDisplay += ch;
                }
            }

            else if (buttonText == "DEL")
            {
                if (!string.IsNullOrEmpty(ExpressionDisplay))
                {
                    ExpressionDisplay = ExpressionDisplay.Substring(0, ExpressionDisplay.Length - 1);
                }
            }
            else if (buttonText == "+" || buttonText == "-" || buttonText == "×" || buttonText == "/")
            {
                // Check if ExpressionDisplay already ends with an operation symbol
                if (ExpressionDisplay.Length > 0 && "+-*//".Contains(ExpressionDisplay[ExpressionDisplay.Length - 1]))
                {
                    // Remove the last character from ExpressionDisplay
                    var builder = new StringBuilder(ExpressionDisplay);
                    builder.Remove(builder.Length - 1, 1);
                    ExpressionDisplay = builder.ToString();
                }

                // Append the operation symbol to ExpressionDisplay
                ExpressionDisplay += buttonText;
            }
        }

        public void EvaluateExpression()
        {
            try
            {
                Dictionary<string, string> functionValues = new Dictionary<string, string>();
                for (int i = 0; i < Functions.Count; i++)
                {
                    functionValues[$"func{i + 1}"] = Functions[i].Value;

                }

                Dictionary<string, string> variableValues = new Dictionary<string, string>();
                for (int i = 0; i < Variables.Count; i++)
                {
                    variableValues[$"var{i + 1}"] = Variables[i].Value;
                    //Variables[i].Value = ReplaceEqualsOrMinusWithZero(Variables[i].Value);
                }

                foreach (var functionValue in functionValues)
                {
                    ExpressionDisplay = ExpressionDisplay.Replace(functionValue.Key, functionValue.Value);
                }

                List<string> functions = new List<string>();

                for (int i = 0; i < Functions.Count; i++)
                {
                    functions.Add(ReplaceEqualsOrMinusWithZero(Functions[i].Value));
                    //functions.Add(Functions[i].Value);
                }

                List<string> variables = new List<string>();

                for (int i = 0; i < Variables.Count; i++)
                {
                    variables.Add(ReplaceEqualsOrMinusWithZero(Variables[i].Value));
                    //variables.Add(Variables[i].Value);
                }

                var expressionHandler = new ExpressionHandler(variables, functions);
                var result = expressionHandler.EvaluateExpression(ExpressionDisplay);
                ResultDisplay = result.ToString();
            }
            catch (Exception ex)
            {
                DisplayMessage("Error", ex.Message, "ok");
                ResultDisplay = "Error";
                //ResultDisplay = ex.Message;
                return;
            }

        }

        public MainPageViewModel()
        {
            AddFuncCommand = new Command(AddFunc);
            RemoveFuncCommand = new Command<FunctionUI>(RemoveFunc);

            AddVarCommand = new Command(AddVar);
            RemoveVarCommand = new Command<VariableUI>(RemoveVar);
        }

        private void AddFunc()
        {
            Functions.Add(new FunctionUI());
        }

        private void RemoveFunc(FunctionUI function)
        {
            Functions.Remove(function);
        }


        private void AddVar()
        {
            Variables.Add(new VariableUI());
        }

        private void RemoveVar(VariableUI variable)
        {
            Variables.Remove(variable);
        }

        public string ReplaceEqualsOrMinusWithZero(string input)
        {
            if (input.Contains("=-") || input.Contains("(-"))
            {
                return input.Replace("=-", "=0-").Replace("(-", "(0-");
            }
            else
            {
                return input;
            }
        }


        async void DisplayMessage(string title, string message, string cancel)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}
