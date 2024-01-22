using Minimal_Vehicle_API.Domain.DTOs;
using Minimal_Vehicle_API.Domain.Entities;

namespace Minimal_Vehicle_API.Domain.Interfaces
{
    public interface IAdminService
    {
        List<Admin> FindAll(int? page);

        Admin? FindById(int id);

        Admin? Login(LoginDTO loginDTO);

        Admin Add(Admin admin);
        
        
    }
}
