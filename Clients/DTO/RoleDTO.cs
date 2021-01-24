using System;
using System.Collections.Generic;

namespace Client.Model
{
    [Serializable]
    public class RoleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public virtual List<UserDTO> Users { get; set; }
    }
}
