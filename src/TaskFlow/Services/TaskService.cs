// Método para obtener tareas, opcionalmente filtradas por estado
using System.Collections.Generic;
using System.Threading.Tasks;

public List<TaskItem> GetTasks(TaskStatus? filter = null)
{
    // 1. Creamos una nueva lista vacía para guardar los resultados
    List<TaskItem> result = new List<TaskItem>();

    // 2. Verificamos si el usuario envió un filtro
    if (filter.HasValue)
    {
        // Si hay filtro, recorremos todas las tareas una por una
        foreach (TaskItem task in _tasks)
        {
            // Si el estado de la tarea coincide con el filtro que buscamos
            if (task.Status == filter.Value)
            {
                //la agregamos a nuestra lista de resultados
                result.Add(task);
            }
        }
    }
    else
    {
        // Si NO hay filtro, recorremos y agregamos absolutamente todas
        foreach (TaskItem task in _tasks)
        {
            result.Add(task);
        }
    }

    // 3. Devolvemos la lista final lista para ser mostrada
    return result;
}