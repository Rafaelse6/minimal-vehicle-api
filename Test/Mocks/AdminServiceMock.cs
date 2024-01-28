using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Domain.Interfaces;

namespace Test.Mocks
{
    public class AdminServiceMock : IAdminService
    {
        private static List<Admin> admins = new List<Admin>(){
            new Admin{
                Id = 1,
                Email = "adm@teste.com",
                Password = "123456",
                Profile = "Adm"
            },
            new Admin{
                Id = 2,
                Email = "editor@teste.com",
                Password = "123456",
                Profile = "Editor"
            }
        };

        public Admin? FindById(int id)
        {
            return admins.Find(a => a.Id == id);
        }

        public List<Admin> FindAll(int? page)
        {
            return admins;
        }

        public Admin Add(Admin admin)
        {
            admin.Id = admins.Count() + 1;
            admins.Add(admin);

            return admin;
        }

        public Admin? Login(LoginDTO loginDTO)
        {
            return admins.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
        }
    }
}
