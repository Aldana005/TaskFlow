using System;
using System.Collections.Generic;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Utils
{
    public class ConsoleHelper
    {
       
        public static void PromptCreateTask(TaskService taskService) { }

        
        public static void PromptListTasks(TaskService taskService) { }

        public static void PrintTaskList(List<TaskItem> tasks) { }

        
        public static void PromptUpdateStatus(TaskService taskService) { }

        
        public static void PromptUpdateResponsible(TaskService taskService) { }

        public static void PromptDeleteTask(TaskService taskService) { }
    }
}