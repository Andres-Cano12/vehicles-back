using AccessData.Entities;
using AccessData.Repository;
using App.Common.Classes.Base.Repositories;
using App.Common.Classes.Base.Services;
using App.Common.Classes.Constants;
using App.Common.Classes.DTO.Common;
using App.Common.Classes.Exceptions;
using App.Common.Classes.Helpers;
using App.Common.Classes.Validator;
using App.Common.Resources;
using App.Common.Resources.Users;
using AutoMapper;
using AutoMapper.Configuration;
using Common.Classes.BussinesLogic;
using DotNetGigs.Auth;
using DotNetGigs.Helpers;
using DotNetGigs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BusinnesLogic.Services
{
    public class UserService:  BaseService<UserDTO, User>, IUserService
    {
        //private IUserRepository userRepository;
        private IJobRepository jobRepository;
        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly JwtIssuerOptions _jwtOptions;

        public UserService(IBaseCRUDRepository<User> repository, App.Common.Classes.Cache.IMemoryCacheManager
            memoryCacheManager, IMapper mapper, IServiceValidator<User> validation, Microsoft.Extensions.Configuration.IConfiguration configuration,  IJobRepository jobRepository, UserManager<User> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
            : base(repository, memoryCacheManager, mapper, validation, configuration)
        {


            //this.userRepository = userRepository;
            this.jobRepository = jobRepository;
            _mapper = mapper;
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

        }


        public async Task<IActionResult> Login(UserDTO credentials)
        {


            var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
            if (identity == null)
            {
                throw new ValidationServiceException();
            }

            // Serialize and return the response
            var response = new
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                auth_token = await _jwtFactory.GenerateEncodedToken(credentials.UserName, identity),
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                // get the user to verifty
                var userToVerify = await _userManager.FindByNameAsync(userName);

                if (userToVerify != null)
                {
                    // check the credentials  
                    if (await _userManager.CheckPasswordAsync(userToVerify, password))
                    {
                        return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id));
                    }
                }
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }

        public async Task<IActionResult> CrearUsuario(UserRegisterDTO model)
        {


            var userIdentity = _mapper.Map<User>(model);
            string message = "";
            var result = await _userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded) {
                foreach (var item in result.Errors)
                {
                    message += $" {item.Description}";
                }
                throw new Exception(message);
            }

            await jobRepository.CreateAsync(new JobSeeker { IdentityId = userIdentity.Id });
            await jobRepository.SaveChangesAsync();


            var response = new
            {
                id = "Account created",

            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);

            return new OkObjectResult(json);
        }
    }

    #region Validator

    public class UserResourceValidator : BaseServiceValidator<User, UserResource>
    {
        IStringLocalizer<GlobalResource> _globalLocalizer;

        public UserResourceValidator(IStringLocalizer<UserResource> localizer,
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
