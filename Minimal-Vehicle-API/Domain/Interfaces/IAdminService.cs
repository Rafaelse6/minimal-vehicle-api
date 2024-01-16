using Minimal_Vehicle_API.Domain.Entities;

namespace Minimal_Vehicle_API.Domain.Interfaces
{
    public interface IAdminService
    {
        Admin? Login(LoginDTO loginDTO);
    }
}
