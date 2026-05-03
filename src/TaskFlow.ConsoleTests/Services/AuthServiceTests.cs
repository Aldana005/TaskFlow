using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskFlow.Services;

namespace TaskFlow.Services.Tests
{
    [TestClass()]
    public class AuthServiceTests
    {
        [TestMethod()]
        public void Authenticate_CaminoFeliz_UsuarioValidoIgnoraMayusculas_RetornaTrue()
        {
            // Arrange (Preparación: Instanciamos el servicio)
            var authService = new AuthService();

            // Act (Ejecución: Probamos con el usuario registrado usando distintas mayúsculas/minúsculas)
            bool result = authService.Authenticate("aldana", "sánchez");

            // Assert (Validación: Verificamos que el resultado sea Verdadero)
            Assert.IsTrue(result, "El sistema debería permitir el acceso ignorando mayúsculas y minúsculas.");
        }

        [TestMethod()]
        public void Authenticate_CaminoTriste_UsuarioInvalido_RetornaFalse()
        {
            // Arrange
            var authService = new AuthService();

            // Act (Probamos con alguien que no está en la lista)
            bool result = authService.Authenticate("Juan", "Perez");

            // Assert (Verificamos que el resultado sea Falso)
            Assert.IsFalse(result, "El sistema debería denegar el acceso a usuarios no registrados.");
        }

        [TestMethod()]
        public void Authenticate_CaminoTriste_CamposVacios_RetornaFalse()
        {
            // Arrange
            var authService = new AuthService();

            // Act (Simulamos que el usuario apretó Enter sin escribir nada)
            bool result = authService.Authenticate("", "   ");

            // Assert
            Assert.IsFalse(result, "El sistema debería rechazar ingresos vacíos o de solo espacios.");
        }
    }
}