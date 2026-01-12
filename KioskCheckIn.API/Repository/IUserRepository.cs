using KioskCheckIn.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskCheckIn.API.Repository
{
    public interface IUserRepository
    {
        Task<bool> InsertUser(User newUser);
        Task<User> GetUser(string username);

    }
}
