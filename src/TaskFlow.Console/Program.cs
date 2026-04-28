using TaskFlow.Services;
using TaskFlow.Utils;
// Asegúrense de que el namespace de su ConsoleHelper esté referenciado aquí


public class Program
{
    public static void Main(string[] args)
    {
        // Instanciamos el servicio una sola vez. 
        // Esta instancia guarda la lista de tareas en memoria mientras el programa esté abierto.
        TaskService taskService = new TaskService();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n=== NOVA TECH: TASKFLOW ===");
            Console.WriteLine("1. Crear tarea");
            Console.WriteLine("2. Listar tareas");
            Console.WriteLine("3. Actualizar estado");
            Console.WriteLine("4. Cambiar Responsable");
            Console.WriteLine("5. Eliminar tarea");
            Console.WriteLine("Presione cualquier otra tecla para salir.");
            Console.Write("Seleccione una opción: ");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    // Llama a la funcionalidad de crear tarea
                    ConsoleHelper.PromptCreateTask(taskService);
                    break;

                case "2":
                    // Llama a la funcionalidad de listar
                    ConsoleHelper.PromptListTasks(taskService);
                    break;
                
                case "3":
                    //Llama a al funcionalidad de actualizar estado
                    ConsoleHelper.PromptUpdateStatus(taskService);
                    break;

                case "4":
                    //Llama a al funcionalidad de actualizar responsable
                    ConsoleHelper.PromptUpdateResponsible(taskService);
                    break;

                case "5":
                    //Llama a al funcionalidad de eliminar tarea
                    ConsoleHelper.PromptDeleteTask(taskService);
                    break;

                default:
                    // Cualquier otra tecla / entrada hace que salgamos del menú
                    Console.WriteLine("\nSaliendo. ¡Hasta luego!");
                    exit = true;
                    break;
            }
        }
    }
}
