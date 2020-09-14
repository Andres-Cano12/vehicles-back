using AccessData.Entities;
using App.Common.Classes.Base.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccessData.Repository
{
    public interface IUserRepository: IBaseCRUDRepository<User>
    {
    }
}
