using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        private BaseLoginViewModel loginVM;
        public MainWindow()
        {
            InitializeComponent();
            loginVM = new BaseLoginViewModel(this);
            this.DataContext = loginVM;
        }
    }
}
