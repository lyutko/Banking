using System;
using System.Collections.Generic;

namespace Server.Model
{
    [Serializable]
    public class RoleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public virtual ICollection<UserDTO> Users { get; set; }
    }
}
