using System.Windows.Controls;

namespace Client
{
    public partial class ClientDialog : UserControl
    {
        private object _dataContext = null;
        public object Context
        {
            get => _dataContext;
            set { _dataContext = value; this.DataContext = value; }
        }
        public ClientDialog(object dataContext)
        {
            InitializeComponent();
            Context = dataContext;
        }
        public ClientDialog()
        {
            InitializeComponent();
        }
    }
}
