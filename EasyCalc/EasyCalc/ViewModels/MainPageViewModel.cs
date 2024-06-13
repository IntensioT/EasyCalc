using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCalc
{
    public partial class MainPageViewModel : ObservableObject
    {
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

        private void EvaluateExpression()
        {
            try
            {
                double result = Convert.ToDouble(new DataTable().Compute(ExpressionDisplay, null));
                ResultDisplay = result.ToString();
            }
            catch
            {
                ResultDisplay = "Format error";
                return;
            }
        }
    }
}
