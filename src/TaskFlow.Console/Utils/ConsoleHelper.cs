
using TaskFlow.Services;

namespace TaskFlow.Utils
{
    public class ConsoleHelper
    {


        public static void PromptCreateTask(TaskService taskService)
        {
            Console.WriteLine("\n--- CREAR NUEVA TAREA ---");

            string title;
            do
            {
                Console.Write("Título (Obligatorio): ");
                title = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(title));

            Console.Write("Descripción (Opcional): ");
            string description = Console.ReadLine();

            Console.Write("Responsable: ");
            string responsible = Console.ReadLine();

            try
            {
                taskService.CreateTask(title, description, responsible);
                
            }
            catch (Exception ex)
            {
                PrintColorMessage($"\n[Error] {ex.Message}", ConsoleColor.Red);
            }
        }

        public static void PromptListTasks(TaskService taskService)
        {
            Console.WriteLine("\n--- FILTROS DE BÚSQUEDA ---");
            Console.WriteLine("1. Mostrar Todas");
            Console.WriteLine("2. Solo Pendientes");
            Console.WriteLine("3. Solo En Progreso");
            Console.WriteLine("4. Solo Completadas");
            Console.Write("Seleccione una opción (1-4): ");

            string choice = Console.ReadLine();
            List<TaskItem> result;

            switch (choice)
            {
                case "2":
                    result = taskService.GetTasks(TaskStatus.Pendiente);
                    break;
                case "3":
                    result = taskService.GetTasks(TaskStatus.EnProgreso);
                    break;
                case "4":
                    result = taskService.GetTasks(TaskStatus.Completada);
                    break;
                default:
                    // Cualquier otra opción (incluyendo el 1) muestra todas
                    result = taskService.GetTasks();
                    break;
            }
            PrintTaskList(result);
        }
        public static void PrintTaskList(List<TaskItem> tasks)
        {
            if (tasks.Count == 0)
            {
                // Usamos tu nuevo método de color para los avisos
                PrintColorMessage("\n No hay tareas creadas.", ConsoleColor.Yellow);
                return;
            }

            Console.WriteLine("\n--- LISTADO DE TAREAS ---");

            foreach (var task in tasks)
            {
                string updatedDate = task.UpdatedAt.HasValue ? task.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm") : "N/A";

                // Preparamos la columna izquierda reservando siempre 30 espacios de ancho
                string colIzq1 = $"ID: {task.Id}";
                string colIzq2 = $"Responsable: {task.Responsible}";
                string colIzq3 = $"Creada: {task.CreatedAt:dd/MM/yyyy HH:mm}";

                // Al imprimir, aplicamos el -30 a las variables de la izquierda
                Console.WriteLine($"{colIzq1,-30} | Título: {task.Title}");
                Console.WriteLine($"{colIzq2,-30} | Estado: {task.Status}");
                Console.WriteLine($"{colIzq3,-30} | Modificada: {updatedDate}");

                // Alargamos un poco la línea de guiones para que cubra la nueva tarjeta
                // (Los tests de tu equipo piden al menos 40 guiones, así que poner 65 es seguro y pasa igual)
                Console.WriteLine(new string('-', 65));
            }
        }

        public static void PromptUpdateStatus(TaskService taskService)
        {

            Console.WriteLine("\n--- ACTUALIZAR ESTADO DE TAREA ---");
            Console.Write("Ingrese el ID de la tarea a modificar: ");

            string inputId = Console.ReadLine();

            // Verificamos que el usuario haya ingresado un número válido
            if (int.TryParse(inputId, out int id))
            {
                Console.WriteLine("Seleccione el nuevo estado:");
                Console.WriteLine("1. Pendiente");
                Console.WriteLine("2. En Progreso");
                Console.WriteLine("3. Completada");
                Console.Write("Opción (1-3): ");

                string statusChoice = Console.ReadLine();
                TaskStatus newStatus;

                // Asignamos el estado según la opción usando if-else
                if (statusChoice == "1")
                {
                    newStatus = TaskStatus.Pendiente;
                }
                else if (statusChoice == "2")
                {
                    newStatus = TaskStatus.EnProgreso;
                    
                }
                else if (statusChoice == "3")
                {
                    newStatus = TaskStatus.Completada;
                }
                else
                {
                    PrintColorMessage("\n Opcción de estado no valida.", ConsoleColor.Red);
                    return; // Salimos del método si se equivoca
                }
                // Llamamos al servicio que creamos en el commit anterior
                bool success = taskService.UpdateTaskStatus(id, newStatus);

                if (success)
                {
                    PrintColorMessage($"\n El estado de la tarea #{id} fue actualizado a {newStatus}.", ConsoleColor.Green);
                }
                else
                {
                    PrintColorMessage($"\n No se encontró ninguna tarea con el ID #{id}.", ConsoleColor.Red);
                }
            }
            else
            {
                PrintColorMessage("\nDebe ingresar un número de ID válido.",ConsoleColor.Red);
            }


        }

        public static void PromptUpdateResponsible(TaskService taskService)
        {
            Console.WriteLine("\n--- ACTUALIZAR RESPONSABLE DE TAREA ---");
            Console.Write("Ingrese el ID de la tarea a modificar: ");

            string inputId = Console.ReadLine();

            // Verificamos que sea un ID válido
            if (int.TryParse(inputId, out int id))
            {
                Console.Write("Ingrese el nombre del nuevo responsable: ");
                string newResponsible = Console.ReadLine();

                // Pequeña validación para que no pongan un responsable vacío
                if (string.IsNullOrWhiteSpace(newResponsible))
                {
                    PrintColorMessage("\n El nombre del responsable no puede estar vacío.", ConsoleColor.Red);
                    return;
                }

                // Llamamos al servicio
                bool success = taskService.UpdateTaskResponsible(id, newResponsible);

                if (success)
                {
                    PrintColorMessage($"\n El responsable de la tarea #{id} fue actualizado a '{newResponsible}'.", ConsoleColor.Green);
                }
                else
                {
                    PrintColorMessage($"\n[Error] No se encontró ninguna tarea con el ID #{id}.", ConsoleColor.Yellow);
                }
            }
            else
            {
                PrintColorMessage("\n[Error] Debe ingresar un número de ID válido.", ConsoleColor.Red);
            }
        }

        public static void PromptDeleteTask(TaskService taskService)
        {
            Console.WriteLine("\n--- ELIMINAR TAREA ---");
            Console.Write("Ingrese el ID de la tarea a eliminar: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                // Usamos la nueva función del servicio
                var task = taskService.GetTaskById(id);

                if (task != null)
                {
                    // Reutilizamos tu función de imprimir lista pasando solo esta tarea 
                    PrintTaskList(new List<TaskItem> { task });

                    Console.Write("\n¿Está seguro de que desea eliminar esta tarea? (s/n): ");
                    if (Console.ReadLine().ToLower() == "s")
                    {
                        taskService.DeleteTask(id);
                        PrintColorMessage("\n Tarea eliminada correctamente.", ConsoleColor.Green);
                    }
                    else
                    {
                        PrintColorMessage("\n Operación cancelada.", ConsoleColor.Red);
                    }
                }
                else
                {
                    PrintColorMessage($"\n No se encontró la tarea con ID #{id}.", ConsoleColor.Yellow);
                }
            }
            else
            {
                PrintColorMessage("\n Entrada no válida.", ConsoleColor.Red);
            }
        }

        public static bool PromptLogin(AuthService authService)
        {
            Console.WriteLine("\n=== TASKFLOW: INICIO DE SESIÓN ===");

            while (true)
            {
                Console.Write("Ingrese su Nombre: ");
                string nombre = Console.ReadLine();

                Console.Write("Ingrese su Apellido: ");
                string apellido = Console.ReadLine();

                // Validamos
                if (authService.Authenticate(nombre, apellido))
                {
                    PrintColorMessage($"\n ¡Bienvenida, {nombre}! Acceso concedido.", ConsoleColor.Green);
                    return true; // Login exitoso
                }
                else
                {
                    PrintColorMessage("\n Acceso denegado. Sus datos no coinciden.", ConsoleColor.Red);
                    Console.Write("¿Desea intentar de nuevo? (S/N): ");

                    if (Console.ReadLine()?.Trim().ToUpper() != "S")
                    {
                        return false; // El usuario decide salir
                    }
                    Console.WriteLine(); // Espacio para el próximo intento
                }
            }
        }

        public static void PrintColorMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

    }

}