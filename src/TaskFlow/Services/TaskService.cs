using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class TaskService
    {
        // Lista temporal en memoria (el Tech Lead luego la conectará con el JSON)
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
                // Si ya hay tareas, buscamos el ID más alto y le sumamos 1
                newId = _tasks.Max(t => t.Id) + 1;
            }
            else
            {
                // Si la lista está vacía, es la primera tarea
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
    }
}