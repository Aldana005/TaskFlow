using System.Collections.Generic;
using System;

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