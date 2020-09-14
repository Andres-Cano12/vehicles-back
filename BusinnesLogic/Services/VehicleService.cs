using AccessData.Entities;
using AccessData.Repository;
using App.Common.Classes.Base.Repositories;
using App.Common.Classes.Base.Services;
using App.Common.Classes.Constants;
using App.Common.Classes.DTO.Common;
using App.Common.Classes.Extensions;
using App.Common.Classes.Helpers;
using App.Common.Classes.Validator;
using App.Common.Resources;
using App.Common.Resources.Users;
using AutoMapper;
using AutoMapper.Configuration;
using Common.Classes.BussinesLogic;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BusinnesLogic.Services
{
    public class VehicleService:  BaseService<VehicleDTO, Vehicle>, IVehicleService
    {
        private IVehicleRepository vehicleRepository;
        private IMapper _mapper;
        private readonly JsonSerializerSettings _serializerSettings;
        public VehicleService(IBaseCRUDRepository<Vehicle> repository, App.Common.Classes.Cache.IMemoryCacheManager
            memoryCacheManager, IMapper mapper, IServiceValidator<Vehicle> validation, Microsoft.Extensions.Configuration.IConfiguration configuration, IVehicleRepository vehicleRepository)
            : base(repository, memoryCacheManager, mapper, validation, configuration)
        {

            this.vehicleRepository = vehicleRepository;
            _mapper = mapper;


            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

        }


        public async Task<IActionResult> CreateVehicle(VehicleDTO vehicleDTO)
        {
            vehicleDTO.Image = vehicleDTO.File.FileName;
            vehicleDTO.DateAdd = DateTime.Now;
            vehicleDTO.DateEdit = DateTime.Now;

            var filePath = Path.Combine(
            Directory.GetCurrentDirectory(), "Files/imgs");

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            using (var fileStream = new FileStream(Path.Combine(filePath, vehicleDTO.File.FileName), FileMode.Create))
            {
                await vehicleDTO.File.CopyToAsync(fileStream);
            }

            var json = JsonConvert.SerializeObject(this.CreateAsync(vehicleDTO), _serializerSettings);

            return new OkObjectResult(json);
        }

        public async Task<IActionResult> CreateFileVehicle(IFormFile file, string user)
        {




          


            var listVehicleDTO = new List<VehicleDTO>();

            try
            {
                using (var reader = ExcelReaderFactory.CreateReader(file.OpenReadStream()))
                {
                    var column = 0;
                    do
                    {
                        while (reader.Read()) //Each ROW
                        {
                       
                                var vehicleDTO = new VehicleDTO();
                                vehicleDTO.IdVehicle = 0;
                                vehicleDTO.DateAdd = DateTime.Now;
                                vehicleDTO.DateEdit = DateTime.Now;
                                vehicleDTO.UserAdd = user;
                                vehicleDTO.UserEdit = user;
                                vehicleDTO.Active = true;
                                vehicleDTO.Image = reader.GetValue(column).ToString();
                                vehicleDTO.Model =  reader.GetValue(1).ToString();
                                vehicleDTO.LicencePlate = reader.GetValue(2).ToString();
                                vehicleDTO.IdVehicleOwner = reader.GetValue(3).ToString();
                                vehicleDTO.Description = reader.GetValue(4).ToString();
                                vehicleDTO.Value = decimal.Parse(reader.GetValue(5).ToString());
                                listVehicleDTO.Add(vehicleDTO);

                            column++;
                        }
                    } while (reader.NextResult()); //Move to NEXT SHEET

               ;

                    var jsons = JsonConvert.SerializeObject( this._repository.BulkCreateAsync(_mapper.Map<IEnumerable<Vehicle>>(listVehicleDTO)), _serializerSettings);

                    return new OkObjectResult(jsons);
                }
            }
            catch (Exception ex)
            {

                throw;
            }



            var json = JsonConvert.SerializeObject("", _serializerSettings);

            return new OkObjectResult(json);
        }


    }

    #region Validator

    public class VehicleResourceValidator : BaseServiceValidator<Vehicle, UserResource>
    {
        IStringLocalizer<GlobalResource> _globalLocalizer;

        public VehicleResourceValidator(IStringLocalizer<UserResource> localizer,
            IStringLocalizer<GlobalResource> globalLocalizer) : base(localizer)
        {
            _globalLocalizer = globalLocalizer;

        }

        #region Insert Rules
        public override void LoadPreInsertRules()
        {
           

        }

        public override void LoadPostInsertRules()
        {

        }

        #endregion

        #region Update Rules

        public override void LoadPreUpdateRules()
        {
        }

        public override void LoadPostUpdateRules()
        {

        }

        #endregion

        #region Delete Rules

        public override void LoadPreDeleteRules()
        {

        }

        public override void LoadPostDeleteRules()
        {

        }

        #endregion

        #region Advanced Validations



        #endregion
    }

    #endregion
}
