using System;
using System.Collections.Generic;
using System.Linq;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Services
{
	public class AuthService
	{
		// Lista pre-cargada en memoria
		private readonly List<User> _validUsers = new List<User>
		{
			new User { Nombre = "Estefania", Apellido = "Molar" },
			new User { Nombre = "Aldana", Apellido = "Sánchez" },
			new User { Nombre = "Gonzalo", Apellido = "Vallejo" },
			new User { Nombre = "Martín pasiva", Apellido = "Paggi" },
			new User { Nombre = "Gonzalo", Apellido = "Mora" },
			new User { Nombre = "Martin", Apellido = "Videla" }
		};

		public bool Authenticate(string nombre, string apellido)
		{
			if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
				return false;

            // Buscamos si existe coincidencia ignorando mayúsculas, minúsculas y espacios extra
            // Recorremos la lista de usuarios válidos uno por uno
            foreach (User u in _validUsers)
            {
                // 1. Limpiamos los espacios en blanco que el usuario pudo haber tipeado por error
                string nombreIngresadoLimpio = nombre.Trim();
                string apellidoIngresadoLimpio = apellido.Trim();

                // 2. Comparamos los nombres ignorando mayúsculas y minúsculas
                bool nombreCoincide = u.Nombre.Equals(nombreIngresadoLimpio, StringComparison.OrdinalIgnoreCase);

                // 3. Comparamos los apellidos ignorando mayúsculas y minúsculas
                bool apellidoCoincide = u.Apellido.Equals(apellidoIngresadoLimpio, StringComparison.OrdinalIgnoreCase);

                // 4. Si AMBOS coinciden, el usuario es válido
                if (nombreCoincide && apellidoCoincide)
                {
                    return true; // Cortamos la búsqueda acá mismo y devolvemos el OK
                }
            }

            // 5. Si el bucle terminó de revisar toda la lista y no encontró coincidencias
            return false; // El acceso es denegado
        }
	}
}