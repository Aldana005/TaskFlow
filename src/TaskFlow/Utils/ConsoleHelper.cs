using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public static void PrintTaskList(List<TaskItem> tasks)

{

    if (tasks.Count == 0)

    {

        Console.WriteLine("\n[Info] No hay tareas que coincidan con la búsqueda.");

        return;

    }



    Console.WriteLine("\n--- LISTADO DE TAREAS ---");

    foreach (var task in tasks)

    {

        // Formateamos la fecha de actualización si existe, si no, mostramos "N/A"

        string updatedDate = task.UpdatedAt.HasValue

            ? task.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm")

            : "N/A";



        Console.WriteLine($"ID: {task.Id} | Título: {task.Title}");

        Console.WriteLine($"Responsable: {task.Responsible} | Estado: {task.Status}");

        Console.WriteLine($"Creada: {task.CreatedAt:dd/MM/yyyy HH:mm} | Modificada: {updatedDate}");

        Console.WriteLine(new string('-', 40));

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

    // Llamamos al método que creamos en el commit anterior
    PrintTaskList(result);
}