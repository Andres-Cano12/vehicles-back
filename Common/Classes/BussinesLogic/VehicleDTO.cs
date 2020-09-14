using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Classes.BussinesLogic
{
    public class VehicleDTO
    {
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

        public IFormFile File { get; set; }
        public DateTime DateAdd { get; set; }
        public DateTime DateEdit { get; set; }
    }
}
