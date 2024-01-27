using Minimal_Vehicle_API.Domain.Entities;

namespace Test.Domain.Entities
{
    [TestClass]
    public class VehicleTest
    {
        [TestMethod]
        public void GetSetPropertiesTest()
        {
            // Arrange
            var vehicle = new Vehicle();

            // Act
            vehicle.Id = 1;
            vehicle.Name = "Test";
            vehicle.Brand = "Ford";
            vehicle.Year = 2024;

            // Assert
            Assert.AreEqual(vehicle.Id, 1);
            Assert.AreEqual(vehicle.Name, "Test");
            Assert.AreEqual(vehicle.Brand, "Ford");
            Assert.AreEqual(vehicle.Year, 2024);

        }
    }
}
