using System;
using System.Collections.Generic;
using NUnit.Framework;
using storeapi.Bussisnes;
//PROYECTO12
namespace StoreApi.Tests
{
    public class AddressValidatorTests
    {
        private AddressValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new AddressValidator();
        }

        [Test]
        public void Validate_AddressWithInvalidFormat_ReturnsFormatError()
        {
            // Arrange
            string address = "Juan Perez, Calle 1, San Jose";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("Formato"));
            Assert.AreEqual("La dirección debe tener cinco partes: nombre del destinatario, calle, número, ciudad y país.", result["Formato"]);
        }

        [Test]
        public void Validate_ValidAddress_ReturnsNoErrors()
        {
            // Arrange
            string address = "Juan Perez, Calle Falsa, 123, San Jose, Costa Rica";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void Validate_NameTooShort_ReturnsNameError()
        {
            // Arrange
            string address = "Juan, Calle Falsa, 123, San Jose, Costa Rica";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("NombreDestinatario"));
            Assert.AreEqual("El nombre del destinatario debe tener al menos 5 caracteres.", result["NombreDestinatario"]);
        }

        [Test]
        public void Validate_NameContainsInvalidCharacters_ReturnsNameError()
        {
            // Arrange
            string address = "Juan123, Calle Falsa, 123, San Jose, Costa Rica";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("NombreDestinatario"));
            Assert.AreEqual("El nombre solo debe contener letras y espacios.", result["NombreDestinatario"]);
        }

        [Test]
        public void Validate_StreetTooShort_ReturnsStreetError()
        {
            // Arrange
            string address = "Juan Perez, St, 123, San Jose, Costa Rica";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("DireccionCalle"));
            Assert.AreEqual("La calle debe tener al menos 5 caracteres.", result["DireccionCalle"]);
        }

        [Test]
        public void Validate_StreetContainsInvalidCharacters_ReturnsStreetError()
        {
            // Arrange
            string address = "Juan Perez, Calle $%Falsa, 123, San Jose, Costa Rica";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("DireccionCalle"));
            Assert.AreEqual("La dirección contiene caracteres inválidos.", result["DireccionCalle"]);
        }

        [Test]
        public void Validate_NumberTooShort_ReturnsNumberError()
        {
            // Arrange
            string address = "Juan Perez, Calle Falsa, , San Jose, Costa Rica";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("DireccionNumero"));
            Assert.AreEqual("El número debe tener al menos 1 carácter.", result["DireccionNumero"]);
        }

        [Test]
        public void Validate_NumberContainsInvalidCharacters_ReturnsNumberError()
        {
            // Arrange
            string address = "Juan Perez, Calle Falsa, 123#, San Jose, Costa Rica";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("DireccionNumero"));
            Assert.AreEqual("El número de la casa es inválido.", result["DireccionNumero"]);
        }

        [Test]
        public void Validate_CityTooShort_ReturnsCityError()
        {
            // Arrange
            string address = "Juan Perez, Calle Falsa, 123, SJ, Costa Rica";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("DireccionCiudad"));
            Assert.AreEqual("La ciudad debe tener al menos 3 caracteres.", result["DireccionCiudad"]);
        }

        [Test]
        public void Validate_CityContainsInvalidCharacters_ReturnsCityError()
        {
            // Arrange
            string address = "Juan Perez, Calle Falsa, 123, San Jose123, Costa Rica";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("DireccionCiudad"));
            Assert.AreEqual("El nombre de la ciudad solo debe contener letras y espacios.", result["DireccionCiudad"]);
        }

        [Test]
        public void Validate_CountryTooShort_ReturnsCountryError()
        {
            // Arrange
            string address = "Juan Perez, Calle Falsa, 123, San Jose, CR";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("DireccionPais"));
            Assert.AreEqual("El país debe tener al menos 3 caracteres.", result["DireccionPais"]);
        }

        [Test]
        public void Validate_CountryContainsInvalidCharacters_ReturnsCountryError()
        {
            // Arrange
            string address = "Juan Perez, Calle Falsa, 123, San Jose, Costa Rica123";

            // Act
            var result = _validator.Validate(address);

            // Assert
            Assert.IsTrue(result.ContainsKey("DireccionPais"));
            Assert.AreEqual("El nombre del país solo debe contener letras y espacios.", result["DireccionPais"]);
        }
    }
}


