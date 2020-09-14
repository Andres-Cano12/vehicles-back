using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AccessData.Entities
{
    [Table("Vehicle", Schema = "dbo")]
    public class Vehicle
    {
        [Key]
        public int IdVehicle { get; set; }
        public string Image { get; set; }
        public string Model { get; set; }
        public string LicencePlate { get; set; }
        public string IdVehicleOwner { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public bool Active { get; set; }
        public string UserAdd { get; set; }
        public string UserEdit { get; set; }
        public DateTime DateAdd { get; set; }
        public DateTime DateEdit { get; set; }
    }
}
