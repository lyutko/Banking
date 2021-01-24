using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Operations")]
    public class Operation : ICloneable
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public bool ResultIsSuccess { get; set; }


        [Required]
        public string FromAccountNumber { get; set; }
        [Required]
        public string ToAccountNumber { get; set; }


        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

        public object Clone()
        {
            return new Operation
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
