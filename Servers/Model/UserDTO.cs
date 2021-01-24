using System;

namespace Server.Model
{
    [Serializable]
    public class UserDTO
    {
        public UserDTO()
        {
            IsActive = true;
            LastLoginDate = DateTime.Now;
        }
        public int Id { get; set; }
        public string Login { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }

        private string _name;
        public string Name
        {
            get { return Client != null ? Client.FullName : _name; }
            set { _name = value; }
        }


        public int ClientId { get; set; }
        public virtual ClientDTO Client { get; set; }
        public int RoleId { get; set; }
        public virtual RoleDTO Role { get; set; }
    }
}
