using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    public partial class OperationResultDialog : UserControl
    {
        private object _dataContext = null;
        public object Context
        {
            get => _dataContext;
            set { _dataContext = value; this.DataContext = value; }
        }
        public OperationResultDialog(object dataContext = null)
        {
            InitializeComponent();
            Context = dataContext;
        }
        public string Result { get; set; } = "Unknown";
    }
}
