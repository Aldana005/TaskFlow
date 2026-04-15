using System.Threading.Tasks;
using System;

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