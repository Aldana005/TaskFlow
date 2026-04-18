
using System.Threading.Tasks;

namespace TaskFlow.Services
{
    public class TaskService
    {
        
        private List<TaskItem> _tasks = new List<TaskItem>();

        
        public void CreateTask(string title, string description, string responsible)
        {
            // Validación: El título es obligatorio
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("El título de la tarea es obligatorio.");
            }

            // Generación de ID autoincremental
            int newId;
            if (_tasks.Count > 0)
            {
                newId = _tasks.Max(t => t.Id) + 1;
            }
            else
            {
                newId = 1;
            }

            var newTask = new TaskItem
            {
                Id = newId,
                Title = title,
                Description = description,
                Responsible = responsible,
                Status = TaskStatus.Pendiente, // Estado inicial por defecto
                CreatedAt = DateTime.Now       // Fecha automática
            };

            _tasks.Add(newTask);
            Console.WriteLine($"\n[Éxito] Tarea '{title}' creada con el ID #{newId}.");
        }

       

        // Listar tareas
       
        public List<TaskItem> GetTasks(TaskStatus? filter = null)
                {
                // 1. Creamos una nueva lista vac�a para guardar los resultados
                List<TaskItem> result = new List<TaskItem>();

                // 2. Verificamos si el usuario envi� un filtro
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

        // Cambiar estado
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
    
        // Validar si hay tareas
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

        // Cambiar responsable
        public bool UpdateTaskResponsible(int id, string newResponsible)
        {
            // 1. Buscamos la tarea por su ID 
            TaskItem taskFound = null;
            foreach (TaskItem task in _tasks)
            {
                if (task.Id == id)
                {
                    taskFound = task;
                    break;
                }
            }

            // 2. Si la encontramos, actualizamos el responsable
            if (taskFound != null)
            {
                taskFound.Responsible = newResponsible;
                taskFound.UpdatedAt = DateTime.Now; // Dejamos registro de que se modificó
                return true; // Operación exitosa
            }

            return false; // No se encontró el ID
        }
        // Método nuevo para buscar y devolver la tarea completa
        public TaskItem GetTaskById(int id)
        {
            foreach (var task in _tasks) 
                {
                    if (task.Id == id) 
                    {
                        return task;
                    }
            }
            return null;
        }

        // Borrar tarea
        public bool DeleteTask(int id) 
        {
            var task = GetTaskById(id);
            if (task != null)
            {
                return _tasks.Remove(task);
            }
            return false;
        }
    }
}