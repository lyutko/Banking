using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
    [Serializable]
    public class OperationDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public bool ResultIsSuccess { get; set; }

        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public string CurrentAccountNumber { get; set; }
    }
}
