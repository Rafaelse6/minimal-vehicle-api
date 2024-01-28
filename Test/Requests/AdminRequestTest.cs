using Minimal_Vehicle_API.Domain.Entities;
using Minimal_Vehicle_API.Domain.ModelViews;
using System.Net;
using System.Text;
using System.Text.Json;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class AdminRequestTest
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Setup.ClassInit(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }

        [TestMethod]
        public async Task GetAndSetPropertiesTest()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "adm@teste.com",
                Password = "123456"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

            // Act
            var response = await Setup.client.PostAsync("/admins/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var admLogged = JsonSerializer.Deserialize<AdmLogged>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admLogged?.Email ?? "");
            Assert.IsNotNull(admLogged?.Profile ?? "");
            Assert.IsNotNull(admLogged?.Token ?? "");

            Console.WriteLine(admLogged?.Token);
        }
    }
}
