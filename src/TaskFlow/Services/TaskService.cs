using System.Threading.Tasks;
using System;

public bool UpdateTaskStatus(int id, TaskStatus newStatus)
{
    // 1. Buscamos la tarea por su ID usando un foreach tradicional
    TaskItem taskFound = null;

    foreach (TaskItem task in _tasks)
    {
        if (task.Id == id)
        {
            taskFound = task;
            break; // Cortamos el bucle porque ya la encontramos
        }
    }

    // 2. Verificamos si encontramos la tarea
    if (taskFound != null)
    {
        // Actualizamos el estado
        taskFound.Status = newStatus;
        // Registramos la fecha exacta de la modificación
        taskFound.UpdatedAt = DateTime.Now;

        return true; // Indicamos que la operación fue un éxito
    }
    else
    {
        return false; // Indicamos que no se encontró el ID
    }
}
// Método auxiliar para saber si hay tareas registradas
public bool HasTasks()
{
    if (_tasks.Count > 0)
    {
        return true;
    }
    else
    {
        return false;
    }
}