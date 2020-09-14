using AccessData.Entities;
using App.Common.Classes.Base.Services;
using App.Common.Classes.DTO.Common;
using Common.Classes.BussinesLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogic.Services
{
    public interface IVehicleService: IBaseService<VehicleDTO>
    {
        Task<IActionResult> CreateVehicle(VehicleDTO vehicleDTO);
        Task<IActionResult> CreateFileVehicle(IFormFile file, string user);
    }
}
