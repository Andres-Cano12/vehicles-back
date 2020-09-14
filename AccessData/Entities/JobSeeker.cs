using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AccessData.Entities
{
    public class JobSeeker
    {
        [Key]
        public int Id { get; set; }
        public string IdentityId { get; set; }
        public User Identity { get; set; }  // navigation property
        public string Location { get; set; }
    }
}
