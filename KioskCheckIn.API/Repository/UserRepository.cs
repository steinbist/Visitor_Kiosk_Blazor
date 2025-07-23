using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KioskCheckIn.Data.Models;


namespace KioskCheckIn.API.Repository
{
#pragma warning disable
    public class UserRepository : IUserRepository
    {
        private readonly KioskContext _ctx;
        private readonly IMapper _mapper;


        public UserRepository(KioskContext kioskContext, IMapper mapper)
        {
            _ctx = kioskContext;
            _mapper = mapper;
        }

        public async Task<User> GetUser(string username)
        {
            try
            {
               return await _ctx.Users.FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                // For Now
                throw ex;
            }
        }

        public async Task<bool> InsertUser(User newUser)
        {
            try
            {
                await _ctx.AddAsync(newUser);

                var result = await _ctx.SaveChangesAsync();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}
