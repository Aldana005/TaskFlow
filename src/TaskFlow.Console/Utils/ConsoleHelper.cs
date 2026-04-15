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

        public static void PromptListTasks(TaskService taskService) { }

        public static void PrintTaskList(List<TaskItem> tasks) { }

        public static void PromptUpdateStatus(TaskService taskService) { }

        public static void PromptUpdateResponsible(TaskService taskService) { }

        public static void PromptDeleteTask(TaskService taskService) { }
    }
}