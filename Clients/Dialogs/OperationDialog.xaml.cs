using System.Windows.Controls;

namespace Client
{
    public partial class OperationDialog : UserControl
    {
        private object _dataContext = null;
        public object Context
        {
            get => _dataContext;
            set { _dataContext = value; this.DataContext = value; }
        }
        //public OperationDialog()
        //{
        //    InitializeComponent();
        //}
        public OperationDialog(object dataContext = null)
        {
            InitializeComponent();
            Context = dataContext;
        }
    }
}
