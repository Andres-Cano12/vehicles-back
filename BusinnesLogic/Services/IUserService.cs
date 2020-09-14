using AccessData.Entities;
using App.Common.Classes.Base.Services;
using App.Common.Classes.DTO.Common;
using Common.Classes.BussinesLogic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinnesLogic.Services
{
    public interface IUserService: IBaseService<UserDTO>
    {
        Task<IActionResult> Login(UserDTO credentials);
        Task<IActionResult> CrearUsuario(UserRegisterDTO model);
    }
}
