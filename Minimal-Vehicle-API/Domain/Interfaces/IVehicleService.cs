using Minimal_Vehicle_API.Domain.Entities;

namespace Minimal_Vehicle_API.Domain.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicle> GetVehicles(int? page = 1, string? name = null, string? brand = null);    
        Vehicle? FindById(int id);
        void Add(Vehicle vehicle);

        void Update(Vehicle vehicle);

        void Delete(Vehicle vehicle);
    }
}
