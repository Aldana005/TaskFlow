using System;
using TaskFlow.Services;
// Asegúrense de que el namespace de su ConsoleHelper esté referenciado aquí

namespace TaskFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instanciamos el servicio una sola vez. 
            // Esta instancia guarda la lista de tareas en memoria mientras el programa esté abierto.
            TaskService taskService = new TaskService();
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n=== NOVA TECH: TASKFLOW ===");
                Console.WriteLine("1. Crear tarea");
                Console.WriteLine("2. Listar tareas");
                Console.WriteLine("3. Actualizar estado");
                Console.WriteLine("4. Salir (Temporal para pruebas)");
                Console.Write("Seleccione una opción: ");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        // Llama a la funcionalidad del Integrante A
                        ConsoleHelper.PromptCreateTask(taskService);
                        break;

                    case "2":
                        // Llama a la funcionalidad del Integrante B
                        ConsoleHelper.PromptListTasks(taskService);
                        break;