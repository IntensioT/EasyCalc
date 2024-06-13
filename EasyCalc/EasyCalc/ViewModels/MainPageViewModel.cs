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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyCalc
{
    public partial class MainPageViewModel : ObservableObject
    {
        public ObservableCollection<FunctionUI> Functions { get; } = new ObservableCollection<FunctionUI>();

        public ICommand AddFuncCommand { get; }

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

            else if (int.TryParse(buttonText, out var _) || buttonText == "%" || buttonText == ".")
            {
                var ch = buttonText[0];
                ExpressionDisplay += ch;
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

                foreach (var functionValue in functionValues)
                {
                    ExpressionDisplay = ExpressionDisplay.Replace(functionValue.Key, functionValue.Value);
                }

                double result = Convert.ToDouble(new DataTable().Compute(ExpressionDisplay, null));
                ResultDisplay = result.ToString();
            }
            catch
            {
                ResultDisplay = "Format error";
                return;
            }
        }

        public MainPageViewModel()
        {
            AddFuncCommand = new Command(AddFunc);
            RemoveFuncCommand = new Command<FunctionUI>(RemoveFunc);
        }

        private void AddFunc()
        {
            Functions.Add(new FunctionUI());
        }

        public ICommand RemoveFuncCommand { get; }


        private void RemoveFunc(FunctionUI function)
        {
            Functions.Remove(function);
        }
    }
}
