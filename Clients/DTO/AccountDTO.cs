using System;
using System.Collections.Generic;

namespace Client.Model
{
    [Serializable]
    public class AccountDTO
    {
        public AccountDTO()
        {
            IsActive = true;
            IsBlocked = false;
            Amount = 0;
            Operations = new List<OperationDTO>();
        }
        public int Id { get; set; }
        public string Number { get; set; }
        public string EndNumber => "..." + Number.Substring(Number.Length - 4);
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }

        public int ClientId { get; set; }
        //public virtual ClientDTO Client { get; set; }
        public virtual List<OperationDTO> Operations { get; set; }

        public bool WithdrawMoney(decimal amount)
        {
            var canWithdraw = amount < Amount;
            if (canWithdraw) Amount -= amount;
            return canWithdraw;
        }
    }
}
