using System;
using System.Collections.Generic;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class TaskService
    {
        
        private List<TaskItem> _tasks = new List<TaskItem>();

        //crear tarea
        public void CreateTask(string title, string description, string responsible)
        {
           
        }

        //listar tareas
        public List<TaskItem> GetTasks(TaskStatus? filter = null)
        {
            
            return new List<TaskItem>();
        }

        //cambair estado
        public bool UpdateTaskStatus(int id, TaskStatus newStatus)
        {
            
            return false;
        }
        //validar si hay tareas
        public bool HasTasks()
        {
            
            return false;
        }

        //cambiar responsable
        public bool UpdateTaskResponsible(int id, string newResponsible)
        {
            
            return false;
        }

        //borrar tarea
        public bool DeleteTask(int id)
        {
            
            return false;
        }
    }
}