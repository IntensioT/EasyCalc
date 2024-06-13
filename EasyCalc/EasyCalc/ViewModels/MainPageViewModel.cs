using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
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

        public MainPageViewModel() 
        {
            ExpressionDisplay = "4 + 5";
            ResultDisplay = "9";
        }
    }
}
