using System;
using System.Collections.Generic;

namespace Server.Model
{
    [Serializable]
    public class AccountDTO
    {
        public AccountDTO()
        {
            IsActive = true;
            IsBlocked = false;
        }
        public int Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }

        public int ClientId { get; set; }
        //public virtual ClientDTO Client { get; set; }
        public virtual ICollection<OperationDTO> Operations { get; set; }
    }
}
