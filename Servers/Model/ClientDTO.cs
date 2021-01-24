using System;
using System.Collections.Generic;

namespace Server.Model
{
    [Serializable]
    public class ClientDTO
    {
        public ClientDTO()
        {
            RegisteredDate = DateTime.Now;
        }

        public int Id { get; set; }
        public string IPN { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return $"{FirstName}{(LastName?.Length > 0 ? " " + LastName : "")} {SecondName}"; } }

        private DateTime birthhday;
        public DateTime Birthday
        {
            get { return birthhday; }
            set { if ((DateTime.Now - value).TotalDays / 365.25 > 18) birthhday = value; }
        }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public DateTime RegisteredDate { get; set; }


        public virtual ICollection<AccountDTO> Accounts { get; set; }
        public int UserId { get; set; }
        //public virtual UserDTO User { get; set; }
    }
}
