﻿using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyCalc
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            ((MainPageViewModel)BindingContext).EvaluateExpression();

        }

        private void ToggleButton_Clicked(object sender, EventArgs e)
        {
            HiddenList.IsVisible = !HiddenList.IsVisible;

            if (HiddenList.IsVisible)
            {
                //var window = sender as Window;
                ToggleButton.Text = "Func<-";
                MainGrid.ColumnDefinitions[1].Width = new GridLength(300, GridUnitType.Absolute);

                // Увеличиваем размер окна
#if WINDOWS
                var window = Application.Current.Windows.OfType<Window>().FirstOrDefault();
                if (window != null)
                {
                    window.MinimumWidth = window.MinimumWidth + 300;
                    //window.Width = window.Width;

                    var disp = DeviceDisplay.Current.MainDisplayInfo;
                    window.X = (disp.Width / disp.Density - window.Width) / 2;
                    window.Y = (disp.Height / disp.Density - window.Height) / 2;
                }
#endif
            }
            else
            {
                ToggleButton.Text = "Func->";
                MainGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Auto);

                // Возвращаем прежний размер окна
#if WINDOWS
                var window = Application.Current.Windows.OfType<Window>().FirstOrDefault();
                if (window != null)
                {
                    window.MinimumWidth = window.MinimumWidth - 300;
                    window.Width = window.Width - 300;
                    
                    var disp = DeviceDisplay.Current.MainDisplayInfo;
                    window.X = (disp.Width / disp.Density - window.Width) / 2;
                    window.Y = (disp.Height / disp.Density - window.Height) / 2;
                }
#endif
            }
        }

        private void ToggleVarsButton_Clicked(object sender, EventArgs e)
        {
            HiddenVarsList.IsVisible = !HiddenVarsList.IsVisible;

            if (HiddenVarsList.IsVisible)
            {
                ToggleVarsButton.Text = "Vars<-";
                MainGrid.ColumnDefinitions[2].Width = new GridLength(300, GridUnitType.Absolute);

                // Увеличиваем размер окна
#if WINDOWS
                var window = Application.Current.Windows.OfType<Window>().FirstOrDefault();
                if (window != null)
                {
                    window.MinimumWidth = window.MinimumWidth + 300;
                    //window.Width = window.Width;
                    
                    var disp = DeviceDisplay.Current.MainDisplayInfo;
                    window.X = (disp.Width / disp.Density - window.Width) / 2;
                    window.Y = (disp.Height / disp.Density - window.Height) / 2;
                }
#endif
            }
            else
            {
                ToggleVarsButton.Text = "Vars->";
                MainGrid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Auto);

                // Возвращаем прежний размер окна
#if WINDOWS
                var window = Application.Current.Windows.OfType<Window>().FirstOrDefault();
                if (window != null)
                {
                    window.MinimumWidth = window.MinimumWidth - 300;
                    window.Width = window.Width - 300;
                    
                    var disp = DeviceDisplay.Current.MainDisplayInfo;
                    window.X = (disp.Width / disp.Density - window.Width) / 2;
                    window.Y = (disp.Height / disp.Density - window.Height) / 2;

                } 
                // TODO: negative expr handler
#endif
            }
        }

    }
}
