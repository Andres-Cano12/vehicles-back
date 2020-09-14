﻿using AccessData.Context;
using AccessData.Entities;
using App.Common.Classes.Base.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccessData.Repository
{
    public class UserRepository : BaseCRUDRepository<User>, IUserRepository
    {


        public DBContext Context
        {
            get
            {
                return (DBContext)_Database;
            }
        }

        public UserRepository(DBContext database)
            : base(database)
        {

        }

    }
}
