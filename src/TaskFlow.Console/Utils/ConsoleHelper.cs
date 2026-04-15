
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
                Console.WriteLine($"\n[Error] {ex.Message}");
            }
        }

        // 2. Conservamos LOS ESQUELETOS para el resto del equipo

        public static void PromptListTasks(TaskService taskService) 
        {
          Console.WriteLine("\n--- FILTROS DE B�SQUEDA ---");
    Console.WriteLine("1. Mostrar Todas");
    Console.WriteLine("2. Solo Pendientes");
    Console.WriteLine("3. Solo En Progreso");
    Console.WriteLine("4. Solo Completadas");
    Console.Write("Seleccione una opci�n (1-4): ");

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
            // Cualquier otra opci�n (incluyendo el 1) muestra todas
            result = taskService.GetTasks();
            break;
        }

        public static void PrintTaskList(List<TaskItem> tasks)
        {
           if (tasks.Count == 0)

    {

        Console.WriteLine("\n[Info] No hay tareas que coincidan con la b�squeda.");

        return;

    }



    Console.WriteLine("\n--- LISTADO DE TAREAS ---");

    foreach (var task in tasks)

    {

        // Formateamos la fecha de actualizaci�n si existe, si no, mostramos "N/A"

        string updatedDate = task.UpdatedAt.HasValue

            ? task.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm")

            : "N/A";



        Console.WriteLine($"ID: {task.Id} | T�tulo: {task.Title}");

        Console.WriteLine($"Responsable: {task.Responsible} | Estado: {task.Status}");

        Console.WriteLine($"Creada: {task.CreatedAt:dd/MM/yyyy HH:mm} | Modificada: {updatedDate}");

        Console.WriteLine(new string('-', 40));
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
                    newStatus = TaskStatus.EnProgreso;
                }
                else if (statusChoice == "3")
                {
                    newStatus = TaskStatus.Completada;
                }
                else
                {
                    Console.WriteLine("\n[Error] Opción de estado no válida.");
                    return; // Salimos del método si se equivoca
                }
                // Llamamos al servicio que creamos en el commit anterior
                bool success = taskService.UpdateTaskStatus(id, newStatus);

                if (success)
                {
                    Console.WriteLine($"\n[Éxito] El estado de la tarea #{id} fue actualizado a {newStatus}.");
                }
                else
                {
                    Console.WriteLine($"\n[Error] No se encontró ninguna tarea con el ID #{id}.");
                }
            }
            else
            {
                Console.WriteLine("\n[Error] Debe ingresar un número de ID válido.");
            }


        }

        public static void PromptUpdateResponsible(TaskService taskService) { }

        public static void PromptDeleteTask(TaskService taskService) { }
    }

}