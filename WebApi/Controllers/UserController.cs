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
using App.Common.Classes.Exceptions;
using Microsoft.AspNetCore.Identity;
using AccessData.Entities;
using DotNetGigs.Auth;
using Newtonsoft.Json;
using DotNetGigs.Models;
using Microsoft.Extensions.Options;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class UserController :  BaseController<UserDTO>
    {
        protected IStringLocalizer<GlobalResource> _globalLocalizer;
        protected IConfiguration _configuration;
        protected IUserService  userService;

        public UserController(IStringLocalizer<GlobalResource> globalLocalizer, IConfiguration configuration,
            IUserService userService)
            : base(userService, globalLocalizer)
        {
            _globalLocalizer = globalLocalizer;
            _configuration = configuration;

            this.userService = userService;
        }

        [HttpPost]
        [Route("Login")]
        public virtual async Task<IActionResult> Login([FromBody]UserDTO user)
        {
            try
            {
                var data = await this.userService.Login(user);
                return Json(data.AsResponseDTO((int)HttpStatusCode.OK));
            }
            catch (ApplicationException ex)
            {
                return Json(ResponseExtension.AsResponseDTO<string>(null,
                    (int)HttpStatusCode.InternalServerError, ex.Message));
            }
            catch (ValidationServiceException ex)
            {
                return Json(ResponseExtension.AsResponseDTO<string>(null,
                    (int)HttpStatusCode.NotAcceptable, ex.Message));
            }
            catch (Exception ex)
            {
                return Json(ResponseExtension.AsResponseDTO<string>(null,
                    (int)HttpStatusCode.InternalServerError, ex.StackTrace));
            }
        }

        [HttpPost]
        [Route("CreateAsync")]
        public async Task<IActionResult> CreateAsync([FromBody]UserRegisterDTO user)
        {
            try
            {

                var data = await userService.CrearUsuario(user);
                return Json(data.AsResponseDTO((int)HttpStatusCode.OK));
            }
            catch (ApplicationException ex)
            {
                return Json(ResponseExtension.AsResponseDTO<string>(null,
                    (int)HttpStatusCode.InternalServerError, ex.Message));
            }
            catch (ValidationServiceException ex)
            {
                return Json(ResponseExtension.AsResponseDTO<string>(null,
                    (int)HttpStatusCode.NotAcceptable, ex.Message));
            }
            catch (Exception ex)
            {
                return Json(ResponseExtension.AsResponseDTO<string>(null,
                    (int)HttpStatusCode.InternalServerError, ex.Message));
            }

        }

    }
}
