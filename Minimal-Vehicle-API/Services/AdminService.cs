using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Domain.Interfaces;
using Minimal_Vehicle_API.Infrastructure.Db;

namespace Minimal_Vehicle_API.Services
{
    public class AdminService : IAdminService
    {
        private readonly MySQLContext _context;

        public AdminService(MySQLContext context)
        {
            _context = context;
        }

        public Admin? Login(LoginDTO loginDTO)
        {
            var adm = _context.Admins.Where
                (a => a.Email == loginDTO.Email && 
                a.Password == loginDTO.Password)
                .FirstOrDefault();
       
            return adm; 
        }
    }
}
