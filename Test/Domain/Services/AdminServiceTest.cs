using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Infrastructure.Db;
using Minimal_Vehicle_API.Services;
using System.Reflection;

namespace Test.Domain.Services
{
    [TestClass]
    public class AdminServiceTest
    {

        private MySQLContext CreateTestContext()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

            var builder = new ConfigurationBuilder()
                .SetBasePath(path ?? Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            return new MySQLContext(configuration);
        }

        [TestMethod]
        public void FindByIdTest()
        {
            // Arrange
            var context = CreateTestContext();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE admins");

            var adm = new Admin();
            adm.Email = "test@test.com";
            adm.Password = "password";
            adm.Profile = "Adm";

            var adminService = new AdminService(context);

            //Act
            adminService.Add(adm);
            var foundAdm = adminService.FindById(adm.Id);

            // Assert

            Assert.AreEqual(1, foundAdm?.Id);
        }

        [TestMethod]
        public void AddAdminTest()
        {
            // Arrange
            var context = CreateTestContext();
            var adminService = new AdminService(context);
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE admins");

            var adm = new Admin();
            adm.Email = "test@test.com";
            adm.Password = "password";
            adm.Profile = "Adm";


            // Act
            adminService.Add(adm);

            // Assert
            Assert.AreEqual(1, adminService.FindAll(1).Count());
        }
    }
}
