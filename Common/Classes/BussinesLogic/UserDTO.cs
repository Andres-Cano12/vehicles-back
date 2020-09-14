using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using DotNetGigs.ViewModels.Validations;
namespace Common.Classes.BussinesLogic
{

    //[Validator(typeof(CredentialsViewModelValidator))]
    public class UserDTO
    {

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
