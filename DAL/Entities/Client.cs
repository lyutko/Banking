using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Clients")]
    public class Client
    {
        public Client()
        {
            RegisteredDate = DateTime.Now;
            Accounts = new List<Account>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        [Index(IsUnique = true)]
        public string IPN { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string SecondName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public string FullName { get { return $"{FirstName}{(LastName?.Length > 0 ? " " + LastName : "")} {SecondName}"; } }

        private DateTime birthhday;
        [Required]
        [Column(TypeName = "date")]
        public DateTime Birthday
        {
            get { return birthhday; }
            set { if ((DateTime.Now - value).TotalDays / 365.25 > 18) birthhday = value; }
        }
        [MaxLength(15)]
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        [MaxLength(250)]
        public string Image { get; set; }
        [Required]
        public DateTime RegisteredDate { get; set; }


        public virtual ICollection<Account> Accounts { get; set; }
        //public int UserId { get; set; }
        //public virtual User User { get; set; }
    }
}
