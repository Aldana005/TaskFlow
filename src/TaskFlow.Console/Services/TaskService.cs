using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace TaskFlow.Services
{
    public class TaskService
    {
        private List<TaskItem> _tasks = new List<TaskItem>();

        private readonly string _folderPath;
        private readonly string _filePath;

        // Modificamos el constructor para recibir parámetros opcionales
        public TaskService(string customFolder = "data", string customFile = "data/tasks.json")
        {
            _folderPath = customFolder;
            _filePath = customFile;
            LoadTasks();
        }

        // 3. Método para GUARDAR (Espejar la lista al JSON)
        private void SaveTasks()
        {
            // Requerimiento: Si la carpeta no existe, se crea
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }

            // Convertimos la lista a texto JSON 
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(_tasks, options);

            // Sobreescribimos el archivo completo
            File.WriteAllText(_filePath, jsonString);
        }

        // 4. Método para CARGAR (Del JSON a la lista)
        private void LoadTasks()
        {
            // Si el archivo no existe, no hacemos nada (la lista queda vacía)
            if (!File.Exists(_filePath)) return;

            try
            {
                // Leemos el texto y lo convertimos a lista de C#
                string jsonString = File.ReadAllText(_filePath);
                _tasks = JsonSerializer.Deserialize<List<TaskItem>>(jsonString) ?? new List<TaskItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Error interno] No se pudo cargar el archivo JSON: {ex.Message}");
            }
        }
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

            //guardamos la lista actualizada en el Json
            SaveTasks();
            Console.WriteLine($"\n[Éxito] Tarea '{title}' creada con el ID #{newId}.");
        }

        // Listar tareas
        public List<TaskItem> GetTasks(TaskStatus? filter = null)
        {
            List<TaskItem> result = new List<TaskItem>();

            if (filter.HasValue)
            {
                foreach (TaskItem task in _tasks)
                {
                    if (task.Status == filter.Value)
                    {
                        result.Add(task);
                    }
                }
            }
            else
            {
                foreach (TaskItem task in _tasks)
                {
                    result.Add(task);
                }
            }

            return result;
        }

        // Cambiar estado
        public bool UpdateTaskStatus(int id, TaskStatus newStatus)
        {
            TaskItem taskFound = null;

            foreach (TaskItem task in _tasks)
            {
                if (task.Id == id)
                {
                    taskFound = task;
                    break;
                }
            }

            if (taskFound != null)
            {
                taskFound.Status = newStatus;
                taskFound.UpdatedAt = DateTime.Now;
                SaveTasks();
                return true;
            }
            else
            {
                return false;
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
                SaveTasks(); // Persistir el cambio en el JSON
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
                _tasks.Remove(task);
                SaveTasks();
                return true;
            }
            return false;
        }
    }
}