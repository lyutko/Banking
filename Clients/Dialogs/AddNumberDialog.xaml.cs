using System.Windows.Controls;

namespace Client
{
    public partial class AddNumberDialog : UserControl
    {
        private object _dataContext = null;
        public object Context
        {
            get => _dataContext;
            set { _dataContext = value; this.DataContext = value; }
        }
        public AddNumberDialog(object dataContext)
        {
            InitializeComponent();
            Context = dataContext;
        }
        public AddNumberDialog()
        {
            InitializeComponent();
        }
    }
}
