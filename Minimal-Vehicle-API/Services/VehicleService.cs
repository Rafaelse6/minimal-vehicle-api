using Microsoft.EntityFrameworkCore;
using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Domain.Interfaces;
using Minimal_Vehicle_API.Infrastructure.Db;
using System.Linq;

namespace Minimal_Vehicle_API.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly MySQLContext _context;

        public VehicleService(MySQLContext context)
        {
            _context = context;
        }

        public List<Vehicle> GetVehicles(int? page = 1, string? name = null, string? brand = null)
        {
            var query = _context.Vehicles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where
                    (v => EF.Functions.Like(v.Name.ToLower(), $"%{name}%"));
            }

            int itensPerPage = 10;

            if (page != null)
                query = query.Skip((int)(page - 1) * itensPerPage).Take(itensPerPage);


            return [.. query];
        }

        public Vehicle? FindById(int? id)
        {
            return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
        }

        public void Add(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
        }

        public void Update(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            _context.SaveChanges();
        }

        public void Delete(Vehicle vehicle)
        {
            _context.Remove(vehicle);
            _context.SaveChanges();
        }
    }
}
