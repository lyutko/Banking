using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Users")]
    public class User
    {
        public User()
        {
            IsActive = true;
            LastLoginDate = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        [Index(IsUnique = true)]
        public string Login { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime LastLoginDate { get; set; }
        [Required, MaxLength(250)]
        public string PasswordSalt { get; set; }
        [Required, MaxLength(250)]
        public string PasswordHash { get; set; }

        private string _name;
        public string Name
        {
            get { return Client != null ? Client.FullName : _name; }
            set { _name = value; }
        }

        [Index(IsUnique = true)]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
