using System;
using TaskFlow.Services;

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
            Console.WriteLine($"[Error] {ex.Message}");
        }
    }
}