using System.Windows.Controls;

namespace Client
{
    public partial class OperationNumberDialog : UserControl
    {
        private object _dataContext = null;
        public object Context
        {
            get => _dataContext;
            set { _dataContext = value; this.DataContext = value; }
        }
        public OperationNumberDialog(object dataContext)
        {
            InitializeComponent();
            Context = dataContext;
        }
        public OperationNumberDialog()
        {
            InitializeComponent();
        }
    }
}
