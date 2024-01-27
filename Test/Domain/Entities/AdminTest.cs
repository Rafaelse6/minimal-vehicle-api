using Minimal_Vehicle_API.Domain.Entities;

namespace Test.Domain.Entities
{
    [TestClass]
    public class AdminTest
    {

        [TestMethod]
        public void GetSetPropertiesTest()
        {
            //Arrange
            var adm = new Admin();

            //Act
            adm.Id = 1;
            adm.Email = "test@test.com";
            adm.Password = "password";
            adm.Profile = "Adm";

            //Assert
            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("test@test.com", adm.Email);
            Assert.AreEqual("password", adm.Password);
            Assert.AreEqual("Adm", adm.Profile);
        }
    }
}
