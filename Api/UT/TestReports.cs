using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using storeapi.Database;
using core;
//PROYECTO12

namespace UT
{
    [TestFixture]
    public class SalesDBTests
    {
        [SetUp]
        public void Setup()
        {

            var dbtestDefault = "Server=localhost;Database=lab;Uid=root;Pwd=123456;";
            DataConnection.Init(dbtestDefault);
            DatabaseInitializer.Initialize();

        }
        [Test]
        public async Task GetForWeekAsync_ValidDate_ReturnsDataWithTotal()
        {

            // Arrange
            var salesDB = new SalesDB();
            var validDate = new DateTime(2024, 04, 30); // Fecha válida

            // Act
            var result = await salesDB.GetForWeekAsync(validDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count()); // Verifica la cantidad de filas esperada

            // Validar el total de las transacciones semanales
            decimal totalWeek = result.Sum(row => decimal.Parse(row[0])); // Suma de los totales en la primera columna
            Assert.AreEqual(0m, totalWeek); // Verifica que el total sea el esperado
        }

        [Test]
        public async Task GetForWeekAsync_MinDate_ThrowsArgumentException()
        {
            // Arrange
            var salesDB = new SalesDB();

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await salesDB.GetForWeekAsync(DateTime.MinValue));
            Assert.AreEqual("La fecha no puede ser DateTime.MinValue", ex.Message);

        }

        [Test]
        public async Task GetForDayAsync_ValidDate_ReturnsDataWithTotal()
        {
            // Arrange
            // Arrange
            var salesDB = new SalesDB();
            var validDate = new DateTime(2024, 04, 30); // Fecha válida

            // Act
            var result = await salesDB.GetForWeekAsync(validDate);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count()); // Verifica la cantidad de filas esperada

            // Validar el total de las transacciones diarias
            decimal totalDay = result.Sum(row => decimal.Parse(row[0])); // Suma de los totales en la primera columna
            Assert.AreEqual(0m, totalDay); // Verifica que el total sea el esperado
        }

        [Test]
        public async Task GetForDayAsync_MinDate_ThrowsArgumentException()
        {
            // Arrange
            var salesDB = new SalesDB();

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await salesDB.GetForDayAsync(DateTime.MinValue));
            Assert.AreEqual("La fecha no puede ser DateTime.", ex.Message);

        }
    }
}