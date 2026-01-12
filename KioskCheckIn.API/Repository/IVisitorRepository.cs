using KioskCheckIn.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskCheckIn.API.Repository
{
    public interface IVisitorRepository
    {
        Task<Visitor> GetVisitor(int id);
        Task AddVisitorAsync(Visitor visitor);
        void SaveChanges();
    }
}
