using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AccessData.Entities
{
    public class JobSeekerDTO
    {
        public int Id { get; set; }
        public string IdentityId { get; set; }
        public Common.Classes.BussinesLogic.UserDTO Identity { get; set; }  // navigation property
        public string Location { get; set; }
    }
}
