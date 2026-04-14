using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.Models;

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
            return new List<TaskItem>(); 
        }

        // Cambiar estado
        public bool UpdateTaskStatus(int id, TaskStatus newStatus) 
        { 
            return false; 
        }

        // Validar si hay tareas
        public bool HasTasks() 
        { 
            return false; 
        }

        // Cambiar responsable
        public bool UpdateTaskResponsible(int id, string newResponsible) 
        { 
            return false; 
        }

        // Borrar tarea
        public bool DeleteTask(int id) 
        { 
            return false; 
        }
    }
}