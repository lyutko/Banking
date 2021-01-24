using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Accounts")]
    [Serializable]
    public class Account
    {
        public Account()
        {
            IsActive = true;
            IsBlocked = false;
        }
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(34)]
        [Index(IsUnique = true)]
        public string Number { get; set; }
        public string Description { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsBlocked { get; set; }

        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }

        public bool WithdrawMoney(decimal amount)
        {
            var canWithdraw = amount < Amount;
            if (canWithdraw) Amount -= amount;
            return canWithdraw;
        }
    }
}
