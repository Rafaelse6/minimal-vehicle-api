using Microsoft.EntityFrameworkCore;
using Minimal_Vehicle_API.Domain.DTOs;
using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Domain.Interfaces;
using Minimal_Vehicle_API.Infrastructure.Db;
using System.Xml.Linq;

namespace Minimal_Vehicle_API.Services
{
    public class AdminService : IAdminService
    {
        private readonly MySQLContext _context;

        public AdminService(MySQLContext context)
        {
            _context = context;
        }

        public List<Admin> FindAll(int? page)
        {
            var query = _context.Admins.AsQueryable();
           
            int itensPerPage = 10;

            if (page != null)
                query = query.Skip((int)(page - 1) * itensPerPage).Take(itensPerPage);


            return [.. query];
        }

        public Admin? FindById(int id)
        {
            return _context.Admins.Where(a => a.Id == id).FirstOrDefault();
        }

        public Admin Add(Admin admin)
        {
            _context.Admins.Add(admin);
            _context.SaveChanges();

            return admin;
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
