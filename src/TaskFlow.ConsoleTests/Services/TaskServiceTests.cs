
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TaskFlow.Services;

namespace TaskFlow.Services.Tests
{
    [TestClass]
    public class TaskServiceTests
    {
        // ==========================================
        // HELPERS: Manejo de archivos temporales
        // ==========================================
        private static (string folder, string file) CreateTempPaths()
        {
            string folder = Path.Combine(Path.GetTempPath(), "TaskFlowTests", Guid.NewGuid().ToString());
            string file = Path.Combine(folder, "tasks.json");
            return (folder, file);
        }

        private void Cleanup(string folder)
        {
            try
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
            catch
            {
                // No lanzar excepción en cleanup para evitar falsos negativos
            }
        }
        // ==========================================
        // TESTS: CreateTask (Crear Tarea)
        // ==========================================
        [TestMethod]
        public void CreateTask_AddsTask_And_SavesFile()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);

                svc.CreateTask("Tarea de prueba", "Descripción", "Responsable");

                // Archivo creado
                Assert.IsTrue(File.Exists(file), "El archivo JSON debe existir después de crear una tarea.");

                // Tarea en memoria
                var tasks = svc.GetTasks();
                Assert.AreEqual(1, tasks.Count, "Debe existir exactamente una tarea.");

                var task = tasks.First();
                Assert.AreEqual(1, task.Id);
                Assert.AreEqual("Tarea de prueba", task.Title);
                Assert.AreEqual("Descripción", task.Description);
                Assert.AreEqual("Responsable", task.Responsible);
                Assert.AreEqual(global::TaskStatus.Pendiente, task.Status);
                Assert.IsTrue((DateTime.Now - task.CreatedAt).TotalSeconds < 5, "CreatedAt debe ser reciente.");
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void CreateTask_SecondTask_IncrementsId()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);

                svc.CreateTask("T1", "D1", "R1");
                svc.CreateTask("T2", "D2", "R2");

                var tasks = svc.GetTasks();
                Assert.AreEqual(2, tasks.Count, "Deben existir dos tareas.");
                Assert.IsTrue(tasks.Any(t => t.Id == 1 && t.Title == "T1"));
                Assert.IsTrue(tasks.Any(t => t.Id == 2 && t.Title == "T2"));
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void CreateTask_WithEmptyTitle_ThrowsArgumentException()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);

                Assert.ThrowsException<ArgumentException>(() => svc.CreateTask("", "d", "r"));
                Assert.ThrowsException<ArgumentException>(() => svc.CreateTask("   ", "d", "r"));
                Assert.ThrowsException<ArgumentException>(() => svc.CreateTask(null, "d", "r"));
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void CreateTask_AllowsNullOptionalFields()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);

                svc.CreateTask("SoloTitulo", null, null);

                var task = svc.GetTasks().First();
                Assert.AreEqual("SoloTitulo", task.Title);
                Assert.IsNull(task.Description);
                Assert.IsNull(task.Responsible);
            }
            finally
            {
                Cleanup(folder);
            }
        }

        // ==========================================
        // TESTS: DeleteTask (Eliminar Tarea)
        // ==========================================

        [TestMethod]
        public void DeleteTask_TaskExists_ReturnsTrueAndRemovesTask()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Tarea 1", "Desc 1", "A");
                svc.CreateTask("Tarea 2", "Desc 2", "B");

                var before = svc.GetTasks();
                var idToDelete = before.First().Id;
                var result = svc.DeleteTask(idToDelete);

                Assert.IsTrue(result, "DeleteTask debe devolver true para una tarea existente.");
                Assert.AreEqual(1, svc.GetTasks().Count, "Debe disminuir el número de tareas.");
                Assert.IsNull(svc.GetTaskById(idToDelete), "La tarea borrada no debe encontrarse por id.");

                var content = File.ReadAllText(file);
                Assert.IsTrue(content.Contains("\"Id\""), "El JSON resultante debe ser válido.");
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void DeleteTask_TaskDoesNotExist_ReturnsFalseAndListUnchanged()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Solo tarea", "Desc", "A");

                var result = svc.DeleteTask(9999); // ID inexistente

                Assert.IsFalse(result);
                Assert.AreEqual(1, svc.GetTasks().Count, "La lista no debe cambiar.");
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void DeleteTask_LoadsFromJsonFileAndDeletes_IdRemovedFromLoadedData()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                Directory.CreateDirectory(folder);
                var list = new List<TaskItem>
                {
                   new TaskItem { Id = 101, Title = "A", Description = "a", Responsible = "R", Status = global::TaskStatus.Pendiente },
                   new TaskItem { Id = 202, Title = "B", Description = "b", Responsible = "R", Status = global::TaskStatus.Pendiente },
                   new TaskItem { Id = 303, Title = "C", Description = "c", Responsible = "R", Status = global::TaskStatus.Pendiente }
                };
                File.WriteAllText(file, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file); // Carga desde archivo

                Assert.IsTrue(svc.DeleteTask(202));
                Assert.IsFalse(File.ReadAllText(file).Contains("\"Id\": 202"));
                Assert.IsNull(svc.GetTaskById(202));
            }
            finally
            {
                Cleanup(folder);
            }
        }

        // ==========================================
        // TESTS: GetTaskById (Buscar por ID)
        // ==========================================

        [TestMethod]
        public void GetTaskById_NoTasks_ReturnsNull()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                var result = svc.GetTaskById(1);

                Assert.IsNull(result);
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void GetTaskById_SingleTask_ReturnsThatTask()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("T1", "D1", "R1");

                var t = svc.GetTaskById(1);

                Assert.IsNotNull(t);
                Assert.AreEqual(1, t.Id);
                Assert.AreEqual("T1", t.Title);
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void GetTaskById_DuplicateIds_File_ReturnsFirstOccurrence()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                Directory.CreateDirectory(folder);
                var tasks = new List<TaskItem>
                {
                    new TaskItem { Id = 1, Title = "First", Description = "x", Responsible = "a" },
                    new TaskItem { Id = 1, Title = "Second", Description = "y", Responsible = "b" }
                };

                File.WriteAllText(file, JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file);
                var t = svc.GetTaskById(1);

                Assert.IsNotNull(t);
                Assert.AreEqual("First", t.Title, "Debe devolver la primera ocurrencia con el mismo Id.");
            }
            finally
            {
                Cleanup(folder);
            }
        }

        // ==========================================
        // TESTS: UpdateTaskResponsible (Actualizar Resp)
        // ==========================================

        [TestMethod]
        public void UpdateTaskResponsible_TaskExists_UpdatesResponsibleAndReturnsTrue()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var service = new TaskService(folder, file);
                service.CreateTask("Tarea 1", "Desc", "Juan");

                var task = service.GetTasks()[0];
                var oldUpdatedAt = task.UpdatedAt;

                var result = service.UpdateTaskResponsible(task.Id, "Maria");
                var updatedTask = service.GetTaskById(task.Id);

                Assert.IsTrue(result);
                Assert.AreEqual("Maria", updatedTask.Responsible);
                Assert.IsNotNull(updatedTask.UpdatedAt);

                if (oldUpdatedAt.HasValue)
                {
                    Assert.IsTrue(updatedTask.UpdatedAt > oldUpdatedAt);
                }
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void UpdateTaskResponsible_TaskDoesNotExist_ReturnsFalse()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var service = new TaskService(folder, file);
                var result = service.UpdateTaskResponsible(999, "Maria");

                Assert.IsFalse(result);
            }
            finally
            {
                Cleanup(folder);
            }
        }

        // ==========================================
        // TESTS: GetTasks (Listar tareas)
        // ==========================================

        [TestMethod]
        public void GetTasks_NoTasks_ReturnsEmptyList()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);

                var result = svc.GetTasks();

                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Count, "Si no hay tareas, la lista debe venir vacía.");
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void GetTasks_WithTasks_ReturnsAllTasks()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Tarea A", "D", "X");
                svc.CreateTask("Tarea B", "D", "Y");

                var list = svc.GetTasks();

                Assert.AreEqual(2, list.Count);
                CollectionAssert.AreEquivalent(new[] { "Tarea A", "Tarea B" }, list.Select(t => t.Title).ToArray());
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void GetTasks_FilterByStatus_ReturnsOnlyMatching()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);
                svc.CreateTask("Pendiente 1", "D", "A");   // Id 1 - Pendiente
                svc.CreateTask("EnProgreso 1", "D", "B"); // Id 2 - Pendiente -> cambiar a EnProgreso
                svc.CreateTask("Completada 1", "D", "C"); // Id 3 - Pendiente -> cambiar a Completada

                // Cambiamos estados
                Assert.IsTrue(svc.UpdateTaskStatus(2, global::TaskStatus.EnProgreso));
                Assert.IsTrue(svc.UpdateTaskStatus(3, global::TaskStatus.Completada));

                var all = svc.GetTasks();
                Assert.AreEqual(3, all.Count, "Debe devolver todas las tareas sin filtro.");

                var enProgreso = svc.GetTasks(global::TaskStatus.EnProgreso);
                Assert.AreEqual(1, enProgreso.Count);
                Assert.AreEqual("EnProgreso 1", enProgreso[0].Title);

                var completadas = svc.GetTasks(global::TaskStatus.Completada);
                Assert.AreEqual(1, completadas.Count);
                Assert.AreEqual("Completada 1", completadas[0].Title);
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void GetTasks_LoadsFromJsonFile_ReturnsDataAndRespectsFilter()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                Directory.CreateDirectory(folder);
                var list = new List<TaskItem>
                {
                    new TaskItem { Id = 10, Title = "T1", Description = "d", Responsible = "R", Status = global::TaskStatus.Pendiente },
                    new TaskItem { Id = 20, Title = "T2", Description = "d", Responsible = "R", Status = global::TaskStatus.EnProgreso },
                    new TaskItem { Id = 30, Title = "T3", Description = "d", Responsible = "R", Status = global::TaskStatus.Completada }
                };
                File.WriteAllText(file, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

                var svc = new TaskService(folder, file); // carga desde archivo

                var all = svc.GetTasks();
                Assert.AreEqual(3, all.Count);

                var enProgreso = svc.GetTasks(global::TaskStatus.EnProgreso);
                Assert.AreEqual(1, enProgreso.Count);
                Assert.AreEqual("T2", enProgreso[0].Title);
            }
            finally
            {
                Cleanup(folder);
            }
        }

        // ==========================================
        // TESTS: UpdateTaskStatus (Actualizar Estado)
        // ==========================================

        [TestMethod]
        public void UpdateTaskStatus_TaskExists_UpdatesStatusAndReturnsTrue()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);

                // 1. Preparamos el terreno: Creamos una tarea (nace como Pendiente)
                svc.CreateTask("Aprender C#", "Desc", "Estudiante");
                var id = svc.GetTasks()[0].Id; // Agarramos su ID

                // 2. ACTUAMOS: Intentamos cambiarle el estado a 'EnProgreso'
                bool result = svc.UpdateTaskStatus(id, global::TaskStatus.EnProgreso);

                // 3. AFIRMAMOS (Asserts): Comprobamos que todo haya salido bien
                Assert.IsTrue(result, "Debe devolver true si la tarea existe.");

                var updatedTask = svc.GetTaskById(id);
                Assert.AreEqual(global::TaskStatus.EnProgreso, updatedTask.Status, "El estado debe haberse actualizado.");
                Assert.IsNotNull(updatedTask.UpdatedAt, "La fecha de actualización (UpdatedAt) debe haberse registrado.");
            }
            finally
            {
                Cleanup(folder);
            }
        }

        [TestMethod]
        public void UpdateTaskStatus_TaskDoesNotExist_ReturnsFalse()
        {
            var (folder, file) = CreateTempPaths();
            try
            {
                var svc = new TaskService(folder, file);

                // ACTUAMOS: Intentamos actualizar un ID que sabemos que no existe (999)
                bool result = svc.UpdateTaskStatus(999, global::TaskStatus.Completada);

                // AFIRMAMOS: El método debe ser inteligente y devolver false
                Assert.IsFalse(result, "Debe devolver false si se le pasa un ID inexistente.");
            }
            finally
            {
                Cleanup(folder);
            }
        }

    }
}