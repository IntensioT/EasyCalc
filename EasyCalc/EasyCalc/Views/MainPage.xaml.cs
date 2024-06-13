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
    }
}
