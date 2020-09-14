using DotNetGigs.ViewModels.Validations;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Common.Classes.BussinesLogic
{
    public class UserRegisterDTO
    {
        public string Email { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
