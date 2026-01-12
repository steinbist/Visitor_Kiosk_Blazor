using KioskCheckIn.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskCheckIn.API.Repository
{
    public class VisitorRepository : IVisitorRepository
    {
        private readonly KioskContext _ctx;

        public VisitorRepository(KioskContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Visitor> GetVisitor(int id)
        {
            return await _ctx.Visitors.FindAsync(id);
        }

        public async Task AddVisitorAsync(Visitor visitor)
        {
            await _ctx.Visitors.AddAsync(visitor);
            SaveChanges();
        }

        public void SaveChanges()
        {
            _ctx.SaveChanges();
        }
    }
}
