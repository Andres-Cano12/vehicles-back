using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BusinnesLogic.Services;
using App.Common.Classes.Base.WebApi;
using Common.Classes.BussinesLogic;
using Microsoft.Extensions.Localization;
using App.Common.Resources;
using System.Net;
using App.Common.Classes.Extensions;
using Rollbar;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;  
using System;    
using System.Collections.Generic;    
using System.IO;    
using System.Linq;    
using System.Net;    
using System.Net.Http;   
using System.Web;    
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class VehicleController :  BaseController<VehicleDTO>
    {
        protected IStringLocalizer<GlobalResource> _globalLocalizer;
        protected IConfiguration _configuration;
        protected IVehicleService vehicleService;

        public VehicleController(IStringLocalizer<GlobalResource> globalLocalizer, IConfiguration configuration,
            IVehicleService vehicleService)
            : base(vehicleService, globalLocalizer)
        {
            _globalLocalizer = globalLocalizer;
            _configuration = configuration;

            this.vehicleService = vehicleService;
        }

        [HttpPost]  
        [Route("CreateVehicle")]
        public async Task<IActionResult> Post([FromForm]
                VehicleDTO vehicleDTO)
        {        
            try
            {
                vehicleDTO.UserAdd = this.User.ToString();
                vehicleDTO.UserEdit = this.User.ToString();

                if (vehicleDTO.File == null || vehicleDTO.File.Length == 0)
                    return Content("file not selected");

                return Json( this.vehicleService.CreateVehicle(vehicleDTO).AsResponseDTO((int)HttpStatusCode.OK));
            }
            catch (ApplicationException ex)
            {
                return Json(ResponseExtension.AsResponseDTO<string>(null,
                    (int)HttpStatusCode.InternalServerError, ex.Message));
            }
            catch (Exception ex)
            {
                return Json(ResponseExtension.AsResponseDTO<string>(null,
                    (int)HttpStatusCode.InternalServerError, ex.StackTrace));
            }
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm]
                IFormFile file)
        {
            try
            {

                if (file == null || file.Length == 0)
                    return Content("file not selected");

                return  Json(this.vehicleService.CreateFileVehicle(file, this.User.ToString()).AsResponseDTO((int)HttpStatusCode.OK));
            }
            catch (ApplicationException ex)
            {
                return Json(ResponseExtension.AsResponseDTO<string>(null,
                    (int)HttpStatusCode.InternalServerError, ex.Message));
            }
            catch (Exception ex)
            {
                return Json(ResponseExtension.AsResponseDTO<string>(null,
                    (int)HttpStatusCode.InternalServerError, ex.StackTrace));
            }
        }

    }
}
