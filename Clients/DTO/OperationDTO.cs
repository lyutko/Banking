using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client.Model
{
    [Serializable]
    public class OperationDTO : ICloneable
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public string ShortDate => DateTime.ToShortDateString();
        public bool ResultIsSuccess { get; set; }
        public string IsSuccess => ResultIsSuccess ? "Successful" : "Wrong";
        //public string AmountIsPositive => FromAccountNumber != Account.Number ? "+" : "-";
        //public Brush OperationColor => ResultIsSuccess
        //    ? FromAccountNumber != Account.Number ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red)
        //    : new SolidColorBrush(Colors.Black);
        public string AmountIsPositive => FromAccountNumber != CurrentAccountNumber ? "+" : "-";
        public Brush OperationColor => ResultIsSuccess
            ? FromAccountNumber != CurrentAccountNumber ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red)
            : new SolidColorBrush(Colors.Black);
        public Brush ResultColor => ResultIsSuccess ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);

        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public int AccountId { get; set; }
        public string CurrentAccountNumber { get; set; }
        //public virtual AccountDTO Account { get; set; }

        public object Clone()
        {
            return new OperationDTO
            {
                Amount = Amount,
                Description = Description,
                DateTime = DateTime,
                ResultIsSuccess = ResultIsSuccess,
                FromAccountNumber = FromAccountNumber,
                ToAccountNumber = ToAccountNumber,
            };
        }
    }
}
